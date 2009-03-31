using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using lgLcdClassLibrary;

namespace EVEMon.LogitechG15
{
    public delegate void CharChangeHandler(string CharName);
    public delegate void CharRefreshHandler(string CharName);
    public delegate void CharAutoCycleHandler(bool Cycle);

    public partial class Lcdisplay : IDisposable
    {
        public static event CharChangeHandler OnCharNameChange;
        public static event CharRefreshHandler OnCharRefresh;
        public static event CharAutoCycleHandler OnAutoCycleChange;

        private Lcdisplay() 
        {
            _defaultFont = new Font("Microsoft Sans Serif", 13.5f, FontStyle.Regular, GraphicsUnit.Point);
            _bufferOut = new byte[6880];

            // using standard brushes in a multithreaded app is bad mkay ?
            _defBrush = new SolidBrush(Color.Black);
            _backColor = Color.White;
            _defPen = new Pen(_defBrush);

            _bmpLCD = new Bitmap(160, 43);
            _bmpLCD.SetResolution(46f, 46f);
            _lcdGraphics = Graphics.FromImage(_bmpLCD);
            _lcdGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            _bmpLCDX = new Bitmap(160, 43);
            _bmpLCDX.SetResolution(46f, 46f);
            _lcdGraphicsX = Graphics.FromImage(_bmpLCDX);
            _lcdGraphicsX.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            _lines = new string[3] { "", "", "" };
            _formatStringConverted = FORMAT_CHAR_SKILL_TIME_STRING.Replace("[c]", "{0}").Replace("[s]", "{1}").Replace("[t]", "{2}");

            using (Stream strm = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("EVEMon.LogitechG15.EVEMon-all.ico"))
            {
                _eveMonSplash = new Icon(strm);
            }

            _LCD = new LCDInterface();
            _buttonDelegate = new ButtonDelegate(this.ButtonsPressed);
            _LCD.AssignButtonDelegate(_buttonDelegate);
            _LCD.Open("EVEMon", false);
            _btnstatehld = DateTime.Now;
            _cycletime = DateTime.Now.AddSeconds(10);
            _cyclecompletiontimetime = DateTime.Now.AddSeconds(5);
            _cycle = false;
            _showtime = false;
            _cyclecompletiontime = false;
            _showingcompletiontime = false;
            _COMPLETESTR = "Hotaru Nakayoshi\nhas finished learning skill\nHull Upgrades V";
            _leasttime = TimeSpan.FromTicks(DateTime.Now.Ticks);
        }

        #region Cleanup

        private bool _disposed = false;
        ~Lcdisplay()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool bDisposing)
        {
            if (!this._disposed) {
                if (bDisposing || _LCD != null) {
                    _LCD.Close();
                    _LCD = null;
                }
                _singleInstance = null;
            }
            _disposed = true;
        }
        #endregion

        public const uint BUTTON1_VALUE = 1;
        public const uint BUTTON2_VALUE = 2;
        public const uint BUTTON3_VALUE = 4;
        public const uint BUTTON4_VALUE = 8;

        const int STATE_SPLASH = 1;
        const int STATE_CLIST = 2;
        const int STATE_SKILLC = 3;
        const int STATE_CHARS = 4;
        const int STATE_S_CYCLE = 5;
        const int STATE_S_REFRESH = 6;

        /// <summary>
        /// Format for Character / Skill / Time mode. 
        /// Use [c] for character, [s] for skill name, and [t] for Time String
        /// </summary>
        const string FORMAT_CHAR_SKILL_TIME_STRING = "[c]:\n[s]\n[t]";     
   
        private static Lcdisplay _singleInstance;
        private LCDInterface _LCD;
        private Font _defaultFont;
        private Brush _defBrush;
        private Pen _defPen;
        private Bitmap _bmpLCD;
        private Bitmap _bmpLCDX;
        private Graphics _lcdGraphics;
        private Graphics _lcdGraphicsX;
        private Color _backColor;
        private string _formatStringConverted;
        private DateTime _lastRepaint;
        private Icon _eveMonSplash;
        private string _characterName;
        private string _curSkillTraining;
        private double _curperc;
        private TimeSpan _timeToSkillComplete;
        private string _leastchar;
        private TimeSpan _leasttime;
        private string[] _lines;
        private Byte[] _bufferOut;
        private string[] _charlist;
        private uint _oldbuttonstate;
        private DateTime _btnstatehld;
        private DateTime _painttime;
        private DateTime _holdtime;
        private int _state;
        private bool _cycle;
        private int _cycleint;
        private DateTime _cycletime;
        private bool _cyclecompletiontime;
        private int _cyclecompletiontimeint;
        private DateTime _cyclecompletiontimetime;
        private bool _showingcompletiontime;
        private bool _showtime;
        private string _refreshchar;
        private bool _skillcchg;
        private ButtonDelegate _buttonDelegate;

        public bool cycle 
        {
            get { return _cycle; }
            set { _cycle = value; }
        }
        public int cycleint 
        {
            get { return _cycleint; }
            set { _cycleint = value; }
        }
        public bool showtime
        {
            get { return _showtime; }
            set { _showtime = value; }
        }
        public bool cyclecompletiontime
        {
            get { return _cyclecompletiontime; }
            set { _cyclecompletiontime = value; }
        }
        public int cyclecompletiontimeint
        {
            get { return _cyclecompletiontimeint; }
            set { _cyclecompletiontimeint = value; }
        }
        public string refreshchar
        {
            get { return _refreshchar; }
            set { _refreshchar = value; }
        }
        public string CharacterName 
        {
            get { return _characterName; }
            set { _characterName = value; }
        }
        public string CurrentSkillTrainingText 
        {
            get { return _curSkillTraining; }
            set { _curSkillTraining = value; }
        }
        public TimeSpan TimeToComplete 
        {
            get { return _timeToSkillComplete; }
            set { _timeToSkillComplete = value; }
        }
        public string leastchar 
        {
            get { return _leastchar; }
            set { _leastchar = value; }
        }
        public TimeSpan leasttime
        {
            get { return _leasttime; }
            set { _leasttime = value; }
        }
        public double curperc 
        {
            get { return _curperc; }
            set { _curperc = value; }
        }
        public string[] charlist 
        {
            get { return _charlist; }
            set { _charlist = value; }
        }
        public string _COMPLETESTR;

        public static Lcdisplay Instance() 
        {
            if (_singleInstance == null) 
            {
                _singleInstance = new Lcdisplay();
            }
            return _singleInstance;
        }
        public void SkillCompleted()
        {
            switchState(STATE_SKILLC);
        }
        public void switchCycle()
        {
            if (_cycle == true)
                _cycle = false;
            else
                _cycle = true;
        }
        public void displayChars()
        {
            bool nextchar = false;
            TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks - _btnstatehld.Ticks);
            if (now.TotalMilliseconds < 2000)
            {
                nextchar = true;
                _cycle = false;
            }
            _btnstatehld = DateTime.Now;

            if (nextchar)
                reorderList(nextchar);
            switchState(STATE_CLIST);
        }

        public void switchState(int state) 
        {
            _state = state;
            _painttime = DateTime.Now;
            _holdtime = DateTime.Now;
        }
        public void DoRepaint() 
        {
            TimeSpan test = TimeSpan.FromTicks(_painttime.Ticks - DateTime.Now.Ticks);
            if (test.TotalMilliseconds > 0)
                return;
            TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks - _holdtime.Ticks);
            if (_state == STATE_SPLASH && now.TotalSeconds > 4)
            {
                switchState(STATE_CHARS);
            }
            if (_state == STATE_CLIST && now.TotalMilliseconds > 2000)
            {
                switchState(STATE_CHARS);
            }
            if (_state == STATE_SKILLC && now.TotalSeconds > 14)
            {
                switchState(STATE_CHARS);
            }
            if ((_state == STATE_S_CYCLE || _state == STATE_S_REFRESH) && now.TotalSeconds > 2)
            {
                switchState(STATE_CHARS);
            }
            
            switch (_state) 
            {
                case STATE_SPLASH:
                    _painttime = _painttime.AddSeconds(2);
                    ShowSplash();
                    return;
                case STATE_CHARS:
                    if (_cycle)
                    {
                        if (TimeSpan.FromTicks(DateTime.Now.Ticks-_cycletime.Ticks).TotalSeconds > _cycleint)
                        {
                            _cycletime = DateTime.Now;
                            reorderList(true);
                        }
                    }
                    _painttime = _painttime.AddSeconds(1);
                    DoRepaint_CharSkillTimeMode();
                    return;
                case STATE_CLIST:
                    _painttime = _painttime.AddSeconds(2);
                    DoRepaint_charlist();
                    return;
                case STATE_SKILLC:
                    _painttime = _painttime.AddMilliseconds(800);
                    DoRepaint_skillcomplete();
                    return;
                case STATE_S_CYCLE:
                    _painttime = _painttime.AddSeconds(2);
                    DoRepaint_s_cycle();
                    return;
                case STATE_S_REFRESH:
                    _painttime = _painttime.AddSeconds(2);
                    DoRepaint_s_refresh();
                    return;
                default:
                    return;
            }
        }

        private void DoRepaint_CharSkillTimeMode() 
        {
            if (_characterName == null || _characterName == "")
                _characterName = "No character";
            if (_curSkillTraining == null || _curSkillTraining == "")
                _curSkillTraining = "No skill in training";

            string tmpTxt = "";
            if (_cyclecompletiontime)
            {
                if (TimeSpan.FromTicks(DateTime.Now.Ticks - _cyclecompletiontimetime.Ticks).TotalSeconds > _cyclecompletiontimeint)
                {
                    _cyclecompletiontimetime = DateTime.Now;
                    _showingcompletiontime = !_showingcompletiontime;
                }
            }
            if (_showingcompletiontime)
            {
                tmpTxt = string.Format(_formatStringConverted, _characterName, _curSkillTraining, String.Format("Finishes {0}", DateTime.Now + _timeToSkillComplete));
            }
            else
            {
                tmpTxt = string.Format(_formatStringConverted, _characterName, _curSkillTraining, FormatTimeSpan(_timeToSkillComplete));
            }

            string[] tmpLines = tmpTxt.Split("\n".ToCharArray());
            if (tmpLines.Length == 3) 
            {
                _lines[0] = tmpLines[0];
                _lines[1] = tmpLines[1];
                _lines[2] = tmpLines[2];
            }
            _lcdGraphics.Clear(_backColor);
            _lcdGraphicsX.Clear(_backColor);

            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[0], _defaultFont));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[1], _defaultFont));
            RectangleF recLine3 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[2], _defaultFont));
            RectangleF recLine4 = new RectangleF(new PointF(0f, 0f), new SizeF(40f, 10f));
            recLine1.Offset(0f, -1f);
            recLine2.Offset(0f, recLine1.Bottom);
            recLine3.Offset(0f, recLine2.Bottom);

            int len = Convert.ToInt16(_curperc * 157);
            float offset = 66f;
            if (_timeToSkillComplete.Ticks == 0)
            {
                _curperc = 1;
                offset = 61f;
            }
            recLine4.Offset(offset, recLine3.Bottom - 1);
            string perc = _curperc.ToString("P2");
            _lcdGraphics.DrawString(_lines[0], _defaultFont, _defBrush, recLine1);
            _lcdGraphics.DrawString(_lines[1], _defaultFont, _defBrush, recLine2);
            _lcdGraphics.DrawString(_lines[2], _defaultFont, _defBrush, recLine3);
            _lcdGraphicsX.DrawString(perc, _defaultFont, _defBrush, recLine4);

            if (_showtime)
            {
                string curTime = DateTime.Now.ToString("T");
                SizeF size = _lcdGraphics.MeasureString(curTime, _defaultFont);
                RectangleF timeLine = new RectangleF(new PointF(160f - size.Width, 0f), size);
                timeLine.Offset(0f, -1f);
                _lcdGraphics.DrawString(curTime, _defaultFont, _defBrush, timeLine);
            }

            //Pen test = ;
            //_lcdGraphics.DrawRectangle(test, 0, 0, 159, 10);
            _lcdGraphics.DrawRectangle(_defPen, 1, (recLine3.Bottom + 1), 158, 8);
            _lcdGraphics.FillRectangle(_defBrush, 2, (recLine3.Bottom + 2), len, 7);
            DoRepaint_RawBitMapMode();
        }
        private void DoRepaint_skillcomplete() 
        {
            string[] tmpLines = _COMPLETESTR.Split("\n".ToCharArray());
            if (tmpLines.Length > 1)
            {
                _lines[0] = tmpLines[0];
                _lines[1] = tmpLines[1];
                _lines[2] = tmpLines[2];
            }
            _lcdGraphics.Clear(_backColor);
            _lcdGraphicsX.Clear(_backColor);
            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[0], _defaultFont));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[1], _defaultFont));
            RectangleF recLine3 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[2], _defaultFont));
            
            float offset = (159 - recLine1.Width) / 2;
            recLine1.Offset(offset, -1f);
            offset = (159 - recLine2.Width) / 2;
            recLine2.Offset(offset, recLine1.Bottom);
            offset = (159 - recLine3.Width) / 2;
            recLine3.Offset(offset, recLine2.Bottom);

            _lcdGraphics.DrawString(_lines[0], _defaultFont, _defBrush, recLine1);
            _lcdGraphics.DrawString(_lines[1], _defaultFont, _defBrush, recLine2);
            _lcdGraphics.DrawString(_lines[2], _defaultFont, _defBrush, recLine3);

             if (_skillcchg)
            {
                double tmp = 1;
                string perc = tmp.ToString("P2");
                RectangleF recLine4 = new RectangleF(new PointF(0f, 0f), new SizeF(40f, 10f));

                recLine4.Offset(61f, recLine3.Bottom - 1);
                _lcdGraphics.DrawRectangle(_defPen, 1, (recLine3.Bottom + 1), 158, 8);
                _lcdGraphics.FillRectangle(_defBrush, 2, (recLine3.Bottom + 2), 157, 7);
                _lcdGraphicsX.DrawString(perc, _defaultFont, _defBrush, recLine4);
                _skillcchg = false;
            }
            else
            {
                _skillcchg = true;
            }
            DoRepaint_RawBitMapMode(LcdisplayPriority.Alert);
        }
        private void DoRepaint_charlist() 
        {
            _lcdGraphics.Clear(_backColor);
            _lcdGraphicsX.Clear(_backColor);
            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), new SizeF(158f, 11f));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), new SizeF(158f, 11f));
            RectangleF recLine3 = new RectangleF(new PointF(0f, 0f), new SizeF(158f, 11f));
            RectangleF recLine4 = new RectangleF(new PointF(0f, 0f), new SizeF(158f, 11f));
            recLine1.Offset(0f, -1f);
            recLine2.Offset(0f, recLine1.Bottom);
            recLine3.Offset(0f, recLine2.Bottom);
            recLine4.Offset(0f, recLine3.Bottom);

            _lcdGraphicsX.DrawString(_charlist[0], _defaultFont, _defBrush, recLine1);
            if (_charlist.Length > 1)
                _lcdGraphics.DrawString(_charlist[1], _defaultFont, _defBrush, recLine2);
            if (_charlist.Length > 2)
                _lcdGraphics.DrawString(_charlist[2], _defaultFont, _defBrush, recLine3);
            if (_charlist.Length > 3)
                _lcdGraphics.DrawString(_charlist[3], _defaultFont, _defBrush, recLine4);

            //_lcdGraphics.DrawRectangle(test, 0, 0, 159, 10);
            //_lcdGraphics.DrawRectangle(test, 0, 0, 159, 10);
            _lcdGraphics.FillRectangle(_defBrush, 0, 0, 159, 11);
            DoRepaint_RawBitMapMode();
        }
        private void DoRepaint_s_cycle()
        {
            _lcdGraphics.Clear(_backColor);
            _lcdGraphicsX.Clear(_backColor);
            string[] _line = new string[2];

            _line[0] = "Autocycle is now ";
            if (_cycle)
                _line[0] += "on";
            else
                _line[0] += "off";

            _line[1] = "Cycle Time is: " + _cycleint + "s";

            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_line[0], _defaultFont));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_line[1], _defaultFont));

            float offset = (159 - recLine1.Width) / 2;
            recLine1.Offset(offset, -1f);
            offset = (159 - recLine2.Width) / 2;
            recLine2.Offset(offset, recLine1.Bottom);

            _lcdGraphics.DrawString(_line[0], _defaultFont, _defBrush, recLine1);
            _lcdGraphics.DrawString(_line[1], _defaultFont, _defBrush, recLine2);
            DoRepaint_RawBitMapMode();
        }
        private void DoRepaint_s_refresh()
        {
            _lcdGraphics.Clear(_backColor);
            _lcdGraphicsX.Clear(_backColor);

            _lines[0] = "Refreshing Character Information";
            _lines[1] = "of";
            _lines[2] = _refreshchar;

            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[0], _defaultFont));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[1], _defaultFont));
            RectangleF recLine3 = new RectangleF(new PointF(0f, 0f), _lcdGraphics.MeasureString(_lines[2], _defaultFont));

            float offset = (159 - recLine1.Width) / 2;
            recLine1.Offset(offset, -1f);
            offset = (159 - recLine2.Width) / 2;
            recLine2.Offset(offset, recLine1.Bottom);
            offset = (159 - recLine3.Width) / 2;
            recLine3.Offset(offset, recLine2.Bottom);
            _lcdGraphics.DrawString(_lines[0], _defaultFont, _defBrush, recLine1);
            _lcdGraphics.DrawString(_lines[1], _defaultFont, _defBrush, recLine2);
            _lcdGraphics.DrawString(_lines[2], _defaultFont, _defBrush, recLine3);


            DoRepaint_RawBitMapMode();
        }
        private void ShowSplash() 
        {
            _lcdGraphics.Clear(_backColor);
            _lcdGraphicsX.Clear(_backColor);
            _lcdGraphics.DrawIcon(_eveMonSplash, new Rectangle(64, 5, 32, 32));
            DoRepaint_RawBitMapMode(LcdisplayPriority.Alert);
        }

        private void DoRepaint_RawBitMapMode() { DoRepaint_RawBitMapMode(LcdisplayPriority.Normal); }
        private void DoRepaint_RawBitMapMode(LcdisplayPriority priority)
        {
            // locking should not be necesseray but i'll keep it here
            lock (_bmpLCD)
            {
                BitmapData bmData = _bmpLCD.LockBits(
                    new Rectangle(0, 0, _bmpLCD.Width, _bmpLCD.Height),
                    ImageLockMode.ReadOnly, _bmpLCD.PixelFormat);
                try
                {
                    byte[] rawData = new byte[bmData.Stride * _bmpLCD.Height];
                    byte[] rawDataX = new byte[bmData.Stride * _bmpLCD.Height];
                    System.Runtime.InteropServices.Marshal.Copy(bmData.Scan0, rawData, 0, rawData.Length);

                    BitmapData bmDataX = _bmpLCDX.LockBits(
                    new Rectangle(0, 0, _bmpLCDX.Width, _bmpLCDX.Height),
                    ImageLockMode.ReadOnly, _bmpLCDX.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(bmDataX.Scan0, rawDataX, 0, rawDataX.Length);
                    _bmpLCDX.UnlockBits(bmDataX);

                    int bpp = bmData.Stride / _bmpLCD.Width;

                    int currPos = 0;
                    int rowWidth = _bmpLCD.Width * bpp;
                    int height = _bmpLCD.Height;
                    int stride = bmData.Stride;
                    for (int row = 0; row < height; row++)
                    {
                        int rowStart = row * stride;
                        for (int pixel = 0; pixel < rowWidth; pixel += bpp)
                        {
                            _bufferOut[currPos] = rawData[rowStart + pixel + 2];
                            _bufferOut[currPos] ^= rawDataX[rowStart + pixel + 2];
                            currPos++;
                        }
                    }

                    _LCD.DisplayBitmap(ref _bufferOut[0], (int)priority);
                }
                catch
                {
                }
                finally
                {
                    _bmpLCD.UnlockBits(bmData);
                }
                _lastRepaint = DateTime.Now;
            }
        }

        private void reorderList(bool nextchar)
        {
            if (_charlist == null) return;
            string[] nlist = new string[_charlist.Length];
            int nlisti = 0;
            bool enlist = false;
            for (int i = 0; i < _charlist.Length; i++)
            {
                if (_charlist[i] == _characterName)
                {
                    if (nextchar)
                    {
                        if (_charlist.Length > i + 1)
                        {
                            _characterName = _charlist[i + 1];
                        }
                        else
                        {
                            _characterName = _charlist[0];
                        }
                        OnCharNameChange(_characterName);
                        nextchar = false;
                    }
                    else
                    {
                        nlist[nlisti] = _charlist[i];
                        nlisti++;
                        enlist = true;
                    }
                }
                else if (enlist)
                {
                    nlist[nlisti] = charlist[i];
                    nlisti++;
                }
            }
            for (int i = 0; i < _charlist.Length; i++)
            {
                if (_charlist[i] == _characterName)
                {
                    break;
                }
                nlist[nlisti] = _charlist[i];
                nlisti++;
            }

            if (nlist[0] != null)
            {
                _charlist = nlist;
            }
        }
        private int ButtonsPressed(int device, uint dwButtons, IntPtr pContext)
        {
            if (_oldbuttonstate != dwButtons)
            {
                // get all buttons who havent been pressed last time
                uint press = (_oldbuttonstate ^ dwButtons) & dwButtons;
                if ((press & BUTTON1_VALUE) != 0)
                {
                    displayChars();
                }
                if ((press & BUTTON2_VALUE) != 0)
                {
                    // select next skill ready char
                    if (_leastchar != null)
                    {
                        OnCharNameChange(_leastchar);
                        reorderList(false);
                        _cycle = false;
                        OnAutoCycleChange(_cycle);
                        switchState(STATE_CHARS);
                    }
                }
                if ((press & BUTTON3_VALUE) != 0)
                {
                    // update charinfo
                    if (_state == STATE_CHARS || _state == STATE_CLIST)
                    {
                        _refreshchar = _characterName;
                        OnCharRefresh(_characterName);
                        switchState(STATE_S_REFRESH);
                    }
                }
                if ((press & BUTTON4_VALUE) != 0)
                {
                    // switch autocycle on/off
                    switchCycle();
                    OnAutoCycleChange(_cycle);
                    switchState(STATE_S_CYCLE);
                    _cycletime = DateTime.Now;
                }

                _oldbuttonstate = dwButtons;
            }
            return 0;
        }

        static string FormatTimeSpan(TimeSpan timespan) 
        {
            string rtn = "";
            if (timespan.TotalDays >= 1) 
                rtn += timespan.Days.ToString() + "d ";

            if (timespan.Hours < 10)
                rtn += "0";
            rtn += timespan.Hours.ToString() + "h ";
            if (timespan.Minutes < 10)
                rtn += "0";
            rtn += timespan.Minutes.ToString() + "m ";
            if (timespan.Seconds < 10)
                rtn += "0";
            rtn += timespan.Seconds.ToString() + "s";
            if (timespan.Ticks == 0)
                rtn = "Done";
            return rtn;
        }
    }

    public enum LcdisplayPriority
    {
        Alert = lgLcdClassLibrary.LCDInterface.lglcd_PRIORITY_ALERT,
        Normal = lgLcdClassLibrary.LCDInterface.lglcd_PRIORITY_NORMAL,
        Background = lgLcdClassLibrary.LCDInterface.lglcd_PRIORITY_BACKGROUND,
        Idle = lgLcdClassLibrary.LCDInterface.lglcd_PRIORITY_IDLE_NO_SHOW
    }
}
