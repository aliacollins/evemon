using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using lgLcdClassLibrary;
using EVEMon.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EVEMon.LogitechG15
{
    public delegate void CharacterHandler(Character character);
    public delegate void CharRefreshHandler(Character character);
    public delegate void CharAutoCycleHandler(bool cycle);

    public partial class Lcdisplay : IDisposable
    {
        /// <summary>
        /// Fired whenever a button has been pressed which require EVEMon to requery the API for the specified character.
        /// </summary>
        public static event CharacterHandler APIUpdateRequested;

        /// <summary>
        /// Fired whenever the current character changed (because of a button press).
        /// </summary>
        public static event CharacterHandler CurrentCharacterChanged;

        /// <summary>
        /// Fired whenever the auto cycle should change (because of a button press).
        /// </summary>
        public static event CharAutoCycleHandler AutoCycleChanged;


        private Lcdisplay() 
        {
            m_defaultFont = FontFactory.GetFont("Microsoft Sans Serif", 13.5f, FontStyle.Regular, GraphicsUnit.Point);
            m_bufferOut = new byte[6880];

            // using standard brushes in a multithreaded app is bad mkay ?
            m_defBrush = new SolidBrush(Color.Black);
            m_backColor = Color.White;
            m_defPen = new Pen(m_defBrush);

            m_bmpLCD = new Bitmap(160, 43, PixelFormat.Format24bppRgb);
            m_bmpLCD.SetResolution(46f, 46f);
            m_lcdGraphics = Graphics.FromImage(m_bmpLCD);
            m_lcdGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            m_bmpLCDX = new Bitmap(160, 43, PixelFormat.Format24bppRgb);
            m_bmpLCDX.SetResolution(46f, 46f);
            m_lcdGraphicsX = Graphics.FromImage(m_bmpLCDX);
            m_lcdGraphicsX.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            m_lines = new string[3] { "", "", "" };
            m_formatStringConverted = FORMAT_CHAR_SKILL_TIME_STRING.Replace("[c]", "{0}").Replace("[s]", "{1}").Replace("[t]", "{2}");

            using (Stream strm = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("EVEMon.LogitechG15.EVEMon-all.ico"))
            {
                m_EVEMonSplash = new Icon(strm);
            }

            m_lcd = new LCDInterface();
            m_buttonDelegate = new ButtonDelegate(this.OnButtonsPressed);
            m_lcd.AssignButtonDelegate(m_buttonDelegate);
            m_lcd.Open("EVEMon", false);
            m_buttonStateHld = DateTime.Now;
            m_cycleTime = DateTime.Now.AddSeconds(10);
            m_cycleCompletionTimeTime = DateTime.Now.AddSeconds(5);
            m_cycle = false;
            m_showTime = false;
            m_cycleCompletionTime = false;
            m_showingCompletionTime = false;
            m_completionString = "Hotaru Nakayoshi\nhas finished learning skill\nHull Upgrades V";
            m_leasttime = TimeSpan.FromTicks(DateTime.Now.Ticks);
        }

        #region Cleanup

        private bool m_disposed = false;
        ~Lcdisplay()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool isDisposing)
        {
            if (!this.m_disposed)
            {
                if (isDisposing || m_lcd != null) 
                {
                    m_lcd.Close();
                    m_lcd = null;
                }
                s_singleInstance = null;
            }
            m_disposed = true;
        }
        #endregion

        public const uint BUTTON1_VALUE = 1;
        public const uint BUTTON2_VALUE = 2;
        public const uint BUTTON3_VALUE = 4;
        public const uint BUTTON4_VALUE = 8;

        public const int STATE_SPLASH = 1;
        public const int STATE_CLIST = 2;
        public const int STATE_SKILLC = 3;
        public const int STATE_CHARS = 4;
        public const int STATE_S_CYCLE = 5;
        public const int STATE_S_REFRESH = 6;

        /// <summary>
        /// Format for Character / Skill / Time mode. 
        /// Use [c] for character, [s] for skill name, and [t] for Time String
        /// </summary>
        const string FORMAT_CHAR_SKILL_TIME_STRING = "[c]:\n[s]\n[t]";     
   
        private static Lcdisplay s_singleInstance;
        private LCDInterface m_lcd;
        private Font m_defaultFont;
        private Brush m_defBrush;
        private Pen m_defPen;
        private Bitmap m_bmpLCD;
        private Bitmap m_bmpLCDX;
        private Graphics m_lcdGraphics;
        private Graphics m_lcdGraphicsX;
        private Color m_backColor;
        private string m_formatStringConverted;
        private Icon m_EVEMonSplash;
        private int m_currentCharacterIndex;
        private string m_currentSkillTraining;
        private double m_currentPerc;
        private TimeSpan m_timeToSkillComplete;
        private Character m_firstCharacterToComplete;
        private TimeSpan m_leasttime;
        private string[] m_lines;
        private Byte[] m_bufferOut;
        private Character[] m_characters;
        private uint m_oldButtonState;
        private DateTime m_buttonStateHld;
        private DateTime m_paintTime;
        private DateTime m_holdTime;
        private int m_state;
        private bool m_cycle;
        private int m_cycleInterval;
        private DateTime m_cycleTime;
        private bool m_cycleCompletionTime;
        private int m_cycleCompletionTimeInterval;
        private DateTime m_cycleCompletionTimeTime;
        private bool m_showingCompletionTime;
        private bool m_showTime;
        private Character m_refreshCharacter;
        private bool m_skillCchg;
        private ButtonDelegate m_buttonDelegate;
        private string m_completionString;

        public string CompletionString
        {
            get { return m_completionString; }
            set { m_completionString = value; }
        }

        public bool Cycle 
        {
            get { return m_cycle; }
            set { m_cycle = value; }
        }
        public int CycleInterval 
        {
            get { return m_cycleInterval; }
            set { m_cycleInterval = value; }
        }
        public bool ShowTime
        {
            get { return m_showTime; }
            set { m_showTime = value; }
        }
        public bool CycleCompletionTime
        {
            get { return m_cycleCompletionTime; }
            set { m_cycleCompletionTime = value; }
        }
        public int CycleCompletionInterval
        {
            get { return m_cycleCompletionTimeInterval; }
            set { m_cycleCompletionTimeInterval = value; }
        }
        public Character RefreshCharacter
        {
            get { return m_refreshCharacter; }
            set { m_refreshCharacter = value; }
        }

        public int CurrentCharacterIndex
        {
            get { return m_currentCharacterIndex; }
        }

        public string CurrentCharacterName 
        {
            get 
            {
                if (m_currentCharacterIndex < 0 || m_characters == null || m_currentCharacterIndex >= m_characters.Length)
                {
                    return "No character";
                }
                return m_characters[m_currentCharacterIndex].AdornedName;
            }
        }

        public Character CurrentCharacter
        {
            get
            {
                if (m_currentCharacterIndex < 0 || m_characters == null || m_currentCharacterIndex >= m_characters.Length)
                {
                    return null;
                }
                return m_characters[m_currentCharacterIndex];
            }
            set
            {
                m_currentCharacterIndex = Array.IndexOf(m_characters, value);
                if (m_currentCharacterIndex == -1) m_currentCharacterIndex = 0;
            }
        }

        public string CurrentSkillTrainingText 
        {
            get { return m_currentSkillTraining; }
            set { m_currentSkillTraining = value; }
        }
        public TimeSpan TimeToComplete 
        {
            get { return m_timeToSkillComplete; }
            set { m_timeToSkillComplete = value; }
        }
        public Character FirstCharacterToCompleteSkill 
        {
            get { return m_firstCharacterToComplete; }
            set { m_firstCharacterToComplete = value; }
        }
        public TimeSpan FirstSkillCompletionRemaingTime
        {
            get { return m_leasttime; }
            set { m_leasttime = value; }
        }
        public double CurrentCharacterTrainingProgression 
        {
            get { return m_currentPerc; }
            set { m_currentPerc = value; }
        }
        public Character[] Characters 
        {
            get { return m_characters; }
            set { m_characters = value; }
        }

        public static Lcdisplay Instance() 
        {
            if (s_singleInstance == null) 
            {
                s_singleInstance = new Lcdisplay();
            }
            return s_singleInstance;
        }

        #region Painting
        /// <summary>
        /// Performs the repainting of the screen.
        /// </summary>
        public void Paint() 
        {
            TimeSpan test = TimeSpan.FromTicks(m_paintTime.Ticks - DateTime.Now.Ticks);
            if (test.TotalMilliseconds > 0) return;

            TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks - m_holdTime.Ticks);
            if (m_state == STATE_SPLASH && now.TotalSeconds > 4)
            {
                SwitchState(STATE_CHARS);
            }
            if (m_state == STATE_CLIST && now.TotalMilliseconds > 2000)
            {
                SwitchState(STATE_CHARS);
            }
            if (m_state == STATE_SKILLC && now.TotalSeconds > 14)
            {
                SwitchState(STATE_CHARS);
            }
            if ((m_state == STATE_S_CYCLE || m_state == STATE_S_REFRESH) && now.TotalSeconds > 2)
            {
                SwitchState(STATE_CHARS);
            }
            
            switch (m_state) 
            {
                case STATE_SPLASH:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintSplash();
                    return;
                case STATE_CHARS:
                    if (m_cycle)
                    {
                        if (TimeSpan.FromTicks(DateTime.Now.Ticks-m_cycleTime.Ticks).TotalSeconds > m_cycleInterval)
                        {
                            m_cycleTime = DateTime.Now;
                            MoveToNextChar();
                        }
                    }
                    m_paintTime = m_paintTime.AddSeconds(1);
                    PaintsCharacter();
                    return;
                case STATE_CLIST:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintCharactersList();
                    return;
                case STATE_SKILLC:
                    m_paintTime = m_paintTime.AddMilliseconds(800);
                    PaintSkillCompletionMessage();
                    return;
                case STATE_S_CYCLE:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintCycleSettings();
                    return;
                case STATE_S_REFRESH:
                    if (m_refreshCharacter != null)
                    {
                        m_paintTime = m_paintTime.AddSeconds(2);
                        PaintRefreshingMessage();
                    }
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// Paints the current character's training informations, this is the regular painting operation.
        /// </summary>
        private void PaintsCharacter() 
        {
            if (String.IsNullOrEmpty(m_currentSkillTraining)) m_currentSkillTraining = "No skill in training";

            string tmpTxt = "";
            if (m_cycleCompletionTime)
            {
                if (TimeSpan.FromTicks(DateTime.Now.Ticks - m_cycleCompletionTimeTime.Ticks).TotalSeconds > m_cycleCompletionTimeInterval)
                {
                    m_cycleCompletionTimeTime = DateTime.Now;
                    m_showingCompletionTime = !m_showingCompletionTime;
                }
            }
            if (m_showingCompletionTime)
            {
                tmpTxt = string.Format(m_formatStringConverted, CurrentCharacterName, m_currentSkillTraining, String.Format("Finishes {0}", DateTime.Now + m_timeToSkillComplete));
            }
            else
            {
                tmpTxt = string.Format(m_formatStringConverted, CurrentCharacterName, m_currentSkillTraining, FormatTimeSpan(m_timeToSkillComplete));
            }

            string[] tmpLines = tmpTxt.Split("\n".ToCharArray());
            if (tmpLines.Length == 3) 
            {
                m_lines[0] = tmpLines[0];
                m_lines[1] = tmpLines[1];
                m_lines[2] = tmpLines[2];
            }
            m_lcdGraphics.Clear(m_backColor);
            m_lcdGraphicsX.Clear(m_backColor);

            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[0], m_defaultFont));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[1], m_defaultFont));
            RectangleF recLine3 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[2], m_defaultFont));
            RectangleF recLine4 = new RectangleF(new PointF(0f, 0f), new SizeF(40f, 10f));
            recLine1.Offset(0f, -1f);
            recLine2.Offset(0f, recLine1.Bottom);
            recLine3.Offset(0f, recLine2.Bottom);

            int len = Convert.ToInt16(m_currentPerc * 157);
            float offset = 66f;
            if (m_timeToSkillComplete.Ticks == 0)
            {
                m_currentPerc = 1;
                offset = 61f;
            }
            recLine4.Offset(offset, recLine3.Bottom - 1);
            string perc = m_currentPerc.ToString("P2");
            m_lcdGraphics.DrawString(m_lines[0], m_defaultFont, m_defBrush, recLine1);
            m_lcdGraphics.DrawString(m_lines[1], m_defaultFont, m_defBrush, recLine2);
            m_lcdGraphics.DrawString(m_lines[2], m_defaultFont, m_defBrush, recLine3);
            m_lcdGraphicsX.DrawString(perc, m_defaultFont, m_defBrush, recLine4);

            if (m_showTime)
            {
                string curTime = DateTime.Now.ToString("T");
                SizeF size = m_lcdGraphics.MeasureString(curTime, m_defaultFont);
                RectangleF timeLine = new RectangleF(new PointF(160f - size.Width, 0f), size);
                timeLine.Offset(0f, -1f);
                m_lcdGraphics.DrawString(curTime, m_defaultFont, m_defBrush, timeLine);
            }

            //Pen test = ;
            //_lcdGraphics.DrawRectangle(test, 0, 0, 159, 10);
            m_lcdGraphics.DrawRectangle(m_defPen, 1, (recLine3.Bottom + 1), 158, 8);
            m_lcdGraphics.FillRectangle(m_defBrush, 2, (recLine3.Bottom + 2), len, 7);
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints a message for skill completion.
        /// </summary>
        private void PaintSkillCompletionMessage() 
        {
            string[] tmpLines = m_completionString.Split("\n".ToCharArray());
            if (tmpLines.Length == 3)
            {
                m_lines[0] = tmpLines[0];
                m_lines[1] = tmpLines[1];
                m_lines[2] = tmpLines[2];
            }
            m_lcdGraphics.Clear(m_backColor);
            m_lcdGraphicsX.Clear(m_backColor);
            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[0], m_defaultFont));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[1], m_defaultFont));
            RectangleF recLine3 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[2], m_defaultFont));
            
            float offset = (159 - recLine1.Width) / 2;
            recLine1.Offset(offset, -1f);
            offset = (159 - recLine2.Width) / 2;
            recLine2.Offset(offset, recLine1.Bottom);
            offset = (159 - recLine3.Width) / 2;
            recLine3.Offset(offset, recLine2.Bottom);

            m_lcdGraphics.DrawString(m_lines[0], m_defaultFont, m_defBrush, recLine1);
            m_lcdGraphics.DrawString(m_lines[1], m_defaultFont, m_defBrush, recLine2);
            m_lcdGraphics.DrawString(m_lines[2], m_defaultFont, m_defBrush, recLine3);

             if (m_skillCchg)
            {
                double tmp = 1;
                string perc = tmp.ToString("P2");
                RectangleF recLine4 = new RectangleF(new PointF(0f, 0f), new SizeF(40f, 10f));

                recLine4.Offset(61f, recLine3.Bottom - 1);
                m_lcdGraphics.DrawRectangle(m_defPen, 1, (recLine3.Bottom + 1), 158, 8);
                m_lcdGraphics.FillRectangle(m_defBrush, 2, (recLine3.Bottom + 2), 157, 7);
                m_lcdGraphicsX.DrawString(perc, m_defaultFont, m_defBrush, recLine4);
                m_skillCchg = false;
            }
            else
            {
                m_skillCchg = true;
            }
            UpdateLcdDisplay(LcdisplayPriority.Alert);
        }

        /// <summary>
        /// Paints the characters list.
        /// </summary>
        private void PaintCharactersList() 
        {
            // Creates a reordered list with the selected character on top
            List<Character> nlist = new List<Character>();

            for (int i = m_currentCharacterIndex; i < m_characters.Length; i++)
            {
                nlist.Add(m_characters[i]);
            }
            for (int i = 0; i < m_currentCharacterIndex; i++)
            {
                nlist.Add(m_characters[i]);
            }

            // Creates the four lines rectangles
            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), new SizeF(158f, 11f));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), new SizeF(158f, 11f));
            RectangleF recLine3 = new RectangleF(new PointF(0f, 0f), new SizeF(158f, 11f));
            RectangleF recLine4 = new RectangleF(new PointF(0f, 0f), new SizeF(158f, 11f));
            recLine1.Offset(0f, -1f);
            recLine2.Offset(0f, recLine1.Bottom);
            recLine3.Offset(0f, recLine2.Bottom);
            recLine4.Offset(0f, recLine3.Bottom);

            // Perform the painting
            m_lcdGraphics.Clear(m_backColor);
            m_lcdGraphicsX.Clear(m_backColor);

            if (nlist.Count == 0)
                m_lcdGraphicsX.DrawString("No character", m_defaultFont, m_defBrush, recLine1);
            if (nlist.Count > 0)
                m_lcdGraphicsX.DrawString(nlist[0].AdornedName, m_defaultFont, m_defBrush, recLine1);
            if (nlist.Count > 1)
                m_lcdGraphics.DrawString(nlist[1].AdornedName, m_defaultFont, m_defBrush, recLine2);
            if (nlist.Count > 2)
                m_lcdGraphics.DrawString(nlist[2].AdornedName, m_defaultFont, m_defBrush, recLine3);
            if (nlist.Count > 3)
                m_lcdGraphics.DrawString(nlist[3].AdornedName, m_defaultFont, m_defBrush, recLine4);

            m_lcdGraphics.FillRectangle(m_defBrush, 0, 0, 159, 11);
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints the cycling settings on the screen.
        /// </summary>
        private void PaintCycleSettings()
        {
            m_lcdGraphics.Clear(m_backColor);
            m_lcdGraphicsX.Clear(m_backColor);
            string[] _line = new string[2];

            _line[0] = "Autocycle is now ";
            if (m_cycle)
                _line[0] += "on";
            else
                _line[0] += "off";

            _line[1] = "Cycle Time is: " + m_cycleInterval + "s";

            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(_line[0], m_defaultFont));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(_line[1], m_defaultFont));

            float offset = (159 - recLine1.Width) / 2;
            recLine1.Offset(offset, -1f);
            offset = (159 - recLine2.Width) / 2;
            recLine2.Offset(offset, recLine1.Bottom);

            m_lcdGraphics.DrawString(_line[0], m_defaultFont, m_defBrush, recLine1);
            m_lcdGraphics.DrawString(_line[1], m_defaultFont, m_defBrush, recLine2);
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints a waiting message while we're updating the characters.
        /// </summary>
        private void PaintRefreshingMessage()
        {
            m_lcdGraphics.Clear(m_backColor);
            m_lcdGraphicsX.Clear(m_backColor);

            m_lines[0] = "Refreshing Character Information";
            m_lines[1] = "of";
            m_lines[2] = m_refreshCharacter.AdornedName;

            RectangleF recLine1 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[0], m_defaultFont));
            RectangleF recLine2 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[1], m_defaultFont));
            RectangleF recLine3 = new RectangleF(new PointF(0f, 0f), m_lcdGraphics.MeasureString(m_lines[2], m_defaultFont));

            float offset = (159 - recLine1.Width) / 2;
            recLine1.Offset(offset, -1f);
            offset = (159 - recLine2.Width) / 2;
            recLine2.Offset(offset, recLine1.Bottom);
            offset = (159 - recLine3.Width) / 2;
            recLine3.Offset(offset, recLine2.Bottom);
            m_lcdGraphics.DrawString(m_lines[0], m_defaultFont, m_defBrush, recLine1);
            m_lcdGraphics.DrawString(m_lines[1], m_defaultFont, m_defBrush, recLine2);
            m_lcdGraphics.DrawString(m_lines[2], m_defaultFont, m_defBrush, recLine3);

            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints the EVEMon icon at the initialization of the LCD screen.
        /// </summary>
        private void PaintSplash() 
        {
            m_lcdGraphics.Clear(m_backColor);
            m_lcdGraphicsX.Clear(m_backColor);
            m_lcdGraphics.DrawIcon(m_EVEMonSplash, new Rectangle(64, 5, 32, 32));
            UpdateLcdDisplay(LcdisplayPriority.Alert);
        }

        /// <summary>
        /// Fetches the content of the <see cref="Graphics"/> object to the G15 screen with a <see cref="LcdisplayPriority.Normal"/> priority.
        /// </summary>
        private void UpdateLcdDisplay() 
        { 
            UpdateLcdDisplay(LcdisplayPriority.Normal); 
        }

        /// <summary>
        /// Fetches the content of the <see cref="Graphics"/> object to the G15 screen.
        /// </summary>
        /// <param name="priority"></param>
        private unsafe void UpdateLcdDisplay(LcdisplayPriority priority)
        {
            // locking should not be necesseray but i'll keep it here
            lock (m_bmpLCD)
            {
                int width = m_bmpLCD.Width;
                int height = m_bmpLCD.Height;
                Rectangle rect = new Rectangle(0, 0, width, height);

                BitmapData bmData = m_bmpLCD.LockBits(rect, ImageLockMode.ReadOnly, m_bmpLCD.PixelFormat);
                try
                {
                    BitmapData bmDataX = m_bmpLCDX.LockBits(rect, ImageLockMode.ReadOnly, m_bmpLCDX.PixelFormat);
                    try
                    {
                        // Extract bits per pixel and length infos
                        int stride = bmData.Stride;
                        int bpp = stride / width;
                        int length = stride * height;

                        // Copy the content of the bitmp to our buffers 
                        // Unsafe code removes the boundaries checks - a lot faster.
                        fixed (byte* buffer0 = m_bufferOut)
                        {
                            byte* output = buffer0;
                            byte* inputX = (byte*)bmDataX.Scan0.ToPointer();
                            byte* input = (byte*)bmData.Scan0.ToPointer();;

                            for (int i = 0; i < height; i++)
                            {
                                for (int j = 0; j < width; j ++)
                                {
                                    *output = (byte)((*input) ^ (*inputX));
                                    inputX += bpp;
                                    input += bpp;
                                    output++;
                                }
                            }
                        }
                    }
                    finally
                    {
                        m_bmpLCDX.UnlockBits(bmDataX);
                    }
                }
                finally
                {
                    m_bmpLCD.UnlockBits(bmData);
                }

                // Fetches the buffer to the LCD screen
                m_lcd.DisplayBitmap(ref m_bufferOut[0], (int)priority);
            }
        }
        #endregion


        #region Controlling logic
        /// <summary>
        /// Occurs when some of the G15 screen buttons are pressed.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="dwButtons"></param>
        /// <param name="pContext"></param>
        /// <returns></returns>
        private int OnButtonsPressed(int device, uint dwButtons, IntPtr pContext)
        {
            if (m_oldButtonState == dwButtons) return 0;

            // Gets all buttons who havent been pressed last time
            uint press = (m_oldButtonState ^ dwButtons) & dwButtons;

            // Displays the characters' list or move to the next char if the list is already displayed.
            if ((press & BUTTON1_VALUE) != 0)
            {
                DisplayCharactersList();
            }

            // Move to the first character to complete his training
            if ((press & BUTTON2_VALUE) != 0)
            {
                // Select next skill ready char
                if (m_firstCharacterToComplete != null)
                {
                    m_currentCharacterIndex = Array.IndexOf(m_characters, m_firstCharacterToComplete);
                    if (CurrentCharacterChanged != null) CurrentCharacterChanged(m_firstCharacterToComplete);
                    m_cycle = false;

                    if (AutoCycleChanged != null) AutoCycleChanged(m_cycle);
                    SwitchState(STATE_CHARS);
                }
            }

            // Forces a refresh from CCP
            if ((press & BUTTON3_VALUE) != 0)
            {
                if (m_state == STATE_CHARS || m_state == STATE_CLIST)
                {
                    m_refreshCharacter = CurrentCharacter;
                    if (APIUpdateRequested != null) APIUpdateRequested(m_refreshCharacter);
                    SwitchState(STATE_S_REFRESH);
                }
            }

            // Switch autocycle ON/OFF
            if ((press & BUTTON4_VALUE) != 0)
            {
                // switch autocycle on/off
                SwitchCycle();
                if (AutoCycleChanged != null) AutoCycleChanged(m_cycle);
                SwitchState(STATE_S_CYCLE);
                m_cycleTime = DateTime.Now;
            }

            m_oldButtonState = dwButtons;
            return 0;
        }

        /// <summary>
        /// Moves the selection to the next character.
        /// </summary>
        /// <param name="nextchar"></param>
        private void MoveToNextChar()
        {
            if (m_characters == null) return;

            // Move to next char
            m_currentCharacterIndex++;
            if (m_currentCharacterIndex >= m_characters.Length) m_currentCharacterIndex = 0;

            // Requests new data
            if (CurrentCharacterChanged != null) CurrentCharacterChanged(CurrentCharacter);
        }

        /// <summary>
        /// Swutches the state and updates some of the internal times variables.
        /// </summary>
        /// <param name="state"></param>
        public void SwitchState(int state)
        {
            m_state = state;
            m_paintTime = DateTime.Now;
            m_holdTime = DateTime.Now;
        }

        /// <summary>
        /// Updates the characters' list. First call displays the list, the second one moves the selection
        /// </summary>
        public void DisplayCharactersList()
        {
            TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks - m_buttonStateHld.Ticks);
            m_buttonStateHld = DateTime.Now;

            if (now.TotalMilliseconds < 2000)
            {
                m_cycle = false;
                MoveToNextChar();
            }

            SwitchState(STATE_CLIST);
        }

        /// <summary>
        /// On skill completion, switch to the display of the proper message.
        /// </summary>
        public void SkillCompleted()
        {
            SwitchState(STATE_SKILLC);
        }

        /// <summary>
        /// Switches the cycling setting.
        /// </summary>
        public void SwitchCycle()
        {
            m_cycle = !m_cycle;
        }
        #endregion


        /// <summary>
        /// Formats the tumespan to formats such as "3h 21m 4s", "4d 3h 21m 4s", etc...
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
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
