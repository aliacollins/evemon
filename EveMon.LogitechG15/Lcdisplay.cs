using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing.Text;

using lgLcdClassLibrary;

using EVEMon.Common;

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


        private List<ILcdLine> m_lcdLines = new List<ILcdLine>();

        private Lcdisplay() 
        {
            m_defaultFont = FontFactory.GetFont("Microsoft Sans Serif", 13.5f, FontStyle.Regular, GraphicsUnit.Point);

            m_bmpLCD = new Bitmap(G15Constants.G15Width, G15Constants.G15Height, PixelFormat.Format24bppRgb);
            m_bmpLCD.SetResolution(G15Constants.G15DpiX, G15Constants.G15DpiY);
            m_lcdCanvas = Graphics.FromImage(m_bmpLCD);
            m_lcdCanvas.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            m_bmpLCDX = new Bitmap(G15Constants.G15Width, G15Constants.G15Height, PixelFormat.Format24bppRgb);
            m_bmpLCDX.SetResolution(G15Constants.G15DpiX, G15Constants.G15DpiY);
            m_lcdOverlay = Graphics.FromImage(m_bmpLCDX);
            m_lcdOverlay.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            m_lcd = new LCDInterface();
            m_buttonDelegate = new ButtonDelegate(this.OnButtonsPressed);
            m_lcd.AssignButtonDelegate(m_buttonDelegate);
            m_lcd.Open("EVEMon", false);
            m_buttonStateHld = DateTime.Now;
            m_cycleTime = DateTime.Now.AddSeconds(10);
            m_cycleQueueInfoTime = DateTime.Now.AddSeconds(5);
            m_cycle = false;
            m_showTime = false;
            m_cycleCharacterSkillQueueInfo = false;
            m_showingCycledQueueInfo = false;
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
   
        private static Lcdisplay s_singleInstance;
        private LCDInterface m_lcd;
        private Font m_defaultFont;
        private Bitmap m_bmpLCD;
        private Bitmap m_bmpLCDX;
        private Graphics m_lcdCanvas;
        private Graphics m_lcdOverlay;
        private int m_currentCharacterIndex;
        private string m_currentSkillTraining;
        private double m_currentPerc;
        private TimeSpan m_timeToSkillComplete;
        private Character m_firstCharacterToComplete;
        private TimeSpan m_leasttime;
        private uint m_oldButtonState;
        private DateTime m_buttonStateHld;
        private DateTime m_paintTime;
        private DateTime m_holdTime;
        private LcdState m_state;
        private bool m_cycle;
        private int m_cycleInterval;
        private DateTime m_cycleTime;
        private bool m_cycleCharacterSkillQueueInfo;
        private int m_cycleCharacterInfoTimeInterval;
        private DateTime m_cycleQueueInfoTime;
        private bool m_showingCycledQueueInfo;
        private bool m_showTime;
        private bool m_showEVETime;
        private Character m_refreshCharacter;
        private ButtonDelegate m_buttonDelegate;
        private int m_completedSkills;

        public bool Cycle 
        {
            get 
            {
                return m_cycle;
            }
            set
            {
                m_cycle = value;
            }
        }

        public int CycleInterval 
        {
            get
            {
                return m_cycleInterval;
            }
            set
            {
                m_cycleInterval = value;
            }
        }

        public bool ShowSystemTime
        {
            get
            {
                return m_showTime;
            }
            set
            {
                m_showTime = value;
            }
        }

        public bool ShowEVETime
        {
            get
            {
                return m_showEVETime;
            }
            set
            {
                m_showEVETime = value;
            }
        }

        public bool CycleSkillQueueTime
        {
            get
            {
                return m_cycleCharacterSkillQueueInfo;
            }
            set 
            {
                m_cycleCharacterSkillQueueInfo = value;
            }
        }

        public int CycleCompletionInterval
        {
            get
            {
                return m_cycleCharacterInfoTimeInterval;
            }
            set
            {
                m_cycleCharacterInfoTimeInterval = value;
            }
        }

        public Character RefreshCharacter
        {
            get
            {
                return m_refreshCharacter;
            }
            set
            {
                m_refreshCharacter = value;
            }
        }

        public int CurrentCharacterIndex
        {
            get
            {
                return m_currentCharacterIndex;
            }
        }

        public string CurrentCharacterName 
        {
            get 
            {
                if (m_currentCharacterIndex >= Characters.Length)
                {
                    m_currentCharacterIndex = Characters.Length - 1;
                }

                if (m_currentCharacterIndex < 0 || Characters == null)
                {
                    return "No CCP Character";
                }
                return Characters[m_currentCharacterIndex].AdornedName;
            }
        }

        public CCPCharacter CurrentCharacter
        {
            get
            {
                if (m_currentCharacterIndex < 0 || Characters == null || m_currentCharacterIndex >= Characters.Length)
                {
                    return null;
                }
                return Characters[m_currentCharacterIndex];
            }
            set
            {
                m_currentCharacterIndex = Array.IndexOf(Characters, value);
                if (m_currentCharacterIndex == -1)
                    m_currentCharacterIndex = 0;
            }
        }

        public string CurrentSkillTrainingText 
        {
            get
            {
                return m_currentSkillTraining;
            }
            set
            {
                m_currentSkillTraining = value;
            }
        }

        public TimeSpan TimeToComplete 
        {
            get
            {
                return m_timeToSkillComplete;
            }
            set
            {
                m_timeToSkillComplete = value;
            }
        }

        public Character FirstCharacterToCompleteSkill 
        {
            get
            {
                return m_firstCharacterToComplete;
            }
            set
            {
                m_firstCharacterToComplete = value;
            }
        }

        public TimeSpan FirstSkillCompletionRemaingTime
        {
            get
            {
                return m_leasttime;
            }
            set
            {
                m_leasttime = value;
            }
        }
        public double CurrentCharacterTrainingProgression 
        {
            get
            {
                return m_currentPerc;
            }
            set
            {
                m_currentPerc = value;
            }
        }

        public CCPCharacter[] Characters
        {
            get;
            set;
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
            if (test.TotalMilliseconds > 0)
                return;

            TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks - m_holdTime.Ticks);
            if (m_state == LcdState.SplashScreen && now.TotalSeconds > 4)
            {
                SwitchState(LcdState.Character);
            }
            if (m_state == LcdState.CharacterList && now.TotalMilliseconds > 2000)
            {
                SwitchState(LcdState.Character);
            }
            if (m_state == LcdState.SkillComplete && now.TotalSeconds > 14)
            {
                SwitchState(LcdState.Character);
            }
            if ((m_state == LcdState.CycleSettings || m_state == LcdState.Refreshing) && now.TotalSeconds > 2)
            {
                SwitchState(LcdState.Character);
            }
            
            switch (m_state) 
            {
                case LcdState.SplashScreen:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintSplash();
                    return;
                case LcdState.Character:
                    if (m_cycle)
                    {
                        if (TimeSpan.FromTicks(DateTime.Now.Ticks-m_cycleTime.Ticks).TotalSeconds > m_cycleInterval)
                        {
                            m_cycleTime = DateTime.Now;
                            MoveToNextChar();
                            
                            // When moving to next character
                            // we reset the queue info timer
                            m_cycleQueueInfoTime = DateTime.Now;
                            m_showingCycledQueueInfo = false;
                        }
                    }
                    m_paintTime = m_paintTime.AddSeconds(1);
                    PaintsCharacter();
                    return;
                case LcdState.CharacterList:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintCharactersList();
                    return;
                case LcdState.SkillComplete:
                    m_paintTime = m_paintTime.AddMilliseconds(800);
                    PaintSkillCompletionMessage();
                    return;
                case LcdState.CycleSettings:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintCycleSettings();
                    return;
                case LcdState.Refreshing:
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
            m_lcdLines.Clear();

            if (CurrentCharacter == null)
            {
                m_lcdLines.Add(new TextLine("No CCP Characters To Display", m_defaultFont));
                RenderLines();
                UpdateLcdDisplay();
                return;
            }
            
            if (m_cycleCharacterSkillQueueInfo)
            {
                if (TimeSpan.FromTicks(DateTime.Now.Ticks - m_cycleQueueInfoTime.Ticks).TotalSeconds > m_cycleCharacterInfoTimeInterval)
                {
                    m_cycleQueueInfoTime = DateTime.Now;
                    m_showingCycledQueueInfo = !m_showingCycledQueueInfo;
                }
            }

            m_lcdLines.Add(new TextLine(CurrentCharacter.AdornedName, m_defaultFont));

            if (CurrentCharacter.SkillQueue.IsTraining)
            {
                if (m_showingCycledQueueInfo)
                {
                    bool freeTime = CurrentCharacter.SkillQueue.EndTime < DateTime.UtcNow.AddHours(24);

                    if (freeTime)
                    {
                        // Place holder for skill queue free room rendering
                        m_lcdLines.Add(new TextLine(" ", m_defaultFont));
                    }
                    else
                    {
                        var time = Skill.TimeSpanToDescriptiveText(CurrentCharacter.SkillQueue.EndTime - DateTime.UtcNow, DescriptiveTextOptions.SpaceBetween, true);
                        m_lcdLines.Add(new TextLine(String.Format(CultureConstants.DefaultCulture, "Queue populated for: {0}", time), m_defaultFont));
                    }
                }
                else
                {
                    var skill = CurrentCharacter.SkillQueue.CurrentlyTraining;
                    m_lcdLines.Add(new TextLine(skill.ToString(), m_defaultFont));
                }

                m_lcdLines.Add(new TextLine(Skill.TimeSpanToDescriptiveText(TimeToComplete, DescriptiveTextOptions.SpaceBetween, true).TrimStart(' '), m_defaultFont));
            }
            else
            {
                if (CurrentCharacter.SkillQueue.IsPaused)
                {
                    var skill = CurrentCharacter.SkillQueue.CurrentlyTraining;
                    m_lcdLines.Add(new TextLine(skill.ToString(), m_defaultFont));
                    m_lcdLines.Add(new TextLine("Skill Training Is Paused", m_defaultFont));
                }
                else
                {
                    m_lcdLines.Add(new TextLine("No Skill In Training", m_defaultFont));
                    m_lcdLines.Add(new TextLine("Skill Queue Is Empty", m_defaultFont));
                }
            }
            
            m_lcdLines.Add(new ProgressLine(CurrentCharacterTrainingProgression, m_defaultFont));

            RenderLines();
            RenderWalletBalance();
            RenderSkillQueueInfo();
            RenderCompletionTime();
            RenderEVETime();
            RenderSystemTime();

            UpdateLcdDisplay();
        }

        private void RenderLines()
        {
            ClearGraphics();

            float offset = 0;

            foreach (var lcdLine in m_lcdLines)
            {
                lcdLine.Render(m_lcdCanvas, m_lcdOverlay, offset);
                offset += lcdLine.Height;
            }
        }

        private void RenderWalletBalance()
        {
            decimal balance = CurrentCharacter.Balance;
            string walletBalance = String.Format(CultureConstants.DefaultCulture, "{0} ISK", balance.ToString("#,##0.#0"));
            SizeF size = m_lcdCanvas.MeasureString(walletBalance, m_defaultFont);
            SizeF charNameSize = m_lcdCanvas.MeasureString(CurrentCharacter.AdornedName, m_defaultFont);
            float availableWidth = (G15Constants.G15Width - charNameSize.Width);

            if (availableWidth < size.Width)
            {
                walletBalance = AbbreviationFormat(balance, availableWidth);
                size = m_lcdCanvas.MeasureString(walletBalance, m_defaultFont);
            }

            RectangleF line = new RectangleF(new PointF(G15Constants.G15Width - size.Width, 0f), size);
            line.Offset(0f, 0f);
            m_lcdCanvas.DrawString(walletBalance, m_defaultFont, new SolidBrush(Color.Black), line);
        }

        private void RenderSkillQueueInfo()
        {
            bool freeTime = CurrentCharacter.SkillQueue.EndTime < DateTime.UtcNow.AddHours(24);

            if (CurrentCharacter.IsTraining && m_showingCycledQueueInfo && freeTime)
            {
                UpdateSkillQueueFreeRoom();
            }
        }

        private void RenderCompletionTime()
        {
            if (CurrentCharacter.IsTraining)
            {
                string completionDateTime = String.Format(CultureConstants.DefaultCulture, "{0}  {1}",(DateTime.Now + TimeToComplete).ToShortDateString(), TimeExtensions.GetShortTimeString((DateTime.Now + TimeToComplete)));
                SizeF size = m_lcdCanvas.MeasureString(completionDateTime, m_defaultFont);
                RectangleF timeLine = new RectangleF(new PointF(G15Constants.G15Width - size.Width, 0f), size);
                timeLine.Offset(0f, 22f);
                m_lcdCanvas.DrawString(completionDateTime, m_defaultFont, new SolidBrush(Color.Black), timeLine);
            }
        }

        private void RenderEVETime()
        {
            if (m_showEVETime)
            {
                string curEVETime = DateTime.UtcNow.ToString("HH:mm");
                SizeF size = m_lcdCanvas.MeasureString(curEVETime, m_defaultFont);
                RectangleF timeLine = new RectangleF(new PointF(0f, 0f), size);
                timeLine.Offset(0f, 32f);
                m_lcdCanvas.DrawString(curEVETime, m_defaultFont, new SolidBrush(Color.Black), timeLine);
            }
        }

        private void RenderSystemTime()
        {
            if (m_showTime)
            {
                string curTime = TimeExtensions.GetShortTimeString(DateTime.Now);
                SizeF size = m_lcdCanvas.MeasureString(curTime, m_defaultFont);
                RectangleF timeLine = new RectangleF(new PointF(G15Constants.G15Width - size.Width, 0f), size);
                timeLine.Offset(0f, 32f);
                m_lcdCanvas.DrawString(curTime, m_defaultFont, new SolidBrush(Color.Black), timeLine);
            }
        }

        /// <summary>
        /// Paints a message for skill completion.
        /// </summary>
        private void PaintSkillCompletionMessage() 
        {

            if (CurrentCharacter.SkillQueue.LastCompleted == null)
                return;

            m_lcdLines.Clear();

            if (CurrentCharacter == null)
            {
                m_lcdLines.Add(new TextLine("No CCP Characters To Display", m_defaultFont));
                RenderLines();
                UpdateLcdDisplay();
                return;
            }

            m_lcdLines.Add(new TextLine(CurrentCharacter.AdornedName, m_defaultFont));
            m_lcdLines.Add(new TextLine("has finished training", m_defaultFont));

            if (m_completedSkills > 1)
            {
                m_lcdLines.Add(new TextLine(String.Format(CultureConstants.DefaultCulture, "{0} skills", m_completedSkills), m_defaultFont)); 
            }
            else
            {
                m_lcdLines.Add(new TextLine(CurrentCharacter.SkillQueue.LastCompleted.ToString(), m_defaultFont));
            }

            var skillCount = CurrentCharacter.SkillQueue.Count;

            if (skillCount == 0)
            {
                m_lcdLines.Add(new TextLine("NO SKILLS IN QUEUE", m_defaultFont)); 
            }
            else
            {
                m_lcdLines.Add(new TextLine(String.Format(CultureConstants.DefaultCulture, "{0} more skill{1} in queue", skillCount, skillCount == 1 ? String.Empty : "s"), m_defaultFont));
            }

            RenderLines();
            UpdateLcdDisplay(LcdisplayPriority.Alert);
        }

        /// <summary>
        /// Paints the characters list.
        /// </summary>
        private void PaintCharactersList() 
        {
            m_lcdLines.Clear();
            
            if (Characters.Length == 0)
            {
                m_lcdLines.Add(new TextLine("No CCP Characters To Display", m_defaultFont));
                RenderLines();
                UpdateLcdDisplay();
                return;
            }

            // Creates a reordered list with the selected character on top
            List<CCPCharacter> charList = new List<CCPCharacter>();

            for (int i = m_currentCharacterIndex; i < Characters.Length; i++)
            {
                charList.Add(Characters[i]);
            }
            for (int i = 0; i < m_currentCharacterIndex; i++)
            {
                charList.Add(Characters[i]);
            }
                        
            // Perform the painting
            ClearGraphics();
            
            foreach (var character in charList)
            {
                m_lcdLines.Add(new TextLine(character.AdornedName, m_defaultFont));
            }

            RenderLines();
            RenderSelector();
            UpdateLcdDisplay();
        }

        private void RenderSelector()
        {
            m_lcdOverlay.FillRectangle(new SolidBrush(Color.Black), 0, 0, G15Constants.G15Width, 11);
        }

        /// <summary>
        /// Paints the cycling settings on the screen.
        /// </summary>
        private void PaintCycleSettings()
        {
            ClearGraphics();
            m_lcdLines.Clear();
            
            string status = m_cycle ? "on" : "off";
            string statusMsg = String.Format(CultureConstants.DefaultCulture, "Autocycle is now {0}", status);
            m_lcdLines.Add(new TextLine(statusMsg, m_defaultFont));
            
            string cycleMsg = String.Format(CultureConstants.DefaultCulture, "Cycle Time is: {0}s", m_cycleInterval);
            m_lcdLines.Add(new TextLine(cycleMsg, m_defaultFont));

            RenderLines();
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints a waiting message while we're updating the characters.
        /// </summary>
        private void PaintRefreshingMessage()
        {
            ClearGraphics();
            m_lcdLines.Clear();

            m_lcdLines.Add(new TextLine("Refreshing Character Information", m_defaultFont));
            m_lcdLines.Add(new TextLine("of", m_defaultFont));
            m_lcdLines.Add(new TextLine(m_refreshCharacter.AdornedName, m_defaultFont));

            RenderLines();
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints the EVEMon icon at the initialization of the LCD screen.
        /// </summary>
        private void PaintSplash() 
        {
            // Load the icon from the assembly
            Bitmap splashLogo;
            using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("EVEMon.LogitechG15.LCDSplash.bmp"))
            {
                splashLogo = new Bitmap(strm);
            }

            // Clear the graphics
            ClearGraphics();
            
            // Display the splash logo
            int left = (int)((G15Constants.G15Width / 2) - (splashLogo.Width / 2));
            int top = (int)((G15Constants.G15Height /2) - (splashLogo.Height / 2));
            m_lcdCanvas.DrawImage(splashLogo, new Rectangle(left, top, splashLogo.Width, splashLogo.Height));
            UpdateLcdDisplay(LcdisplayPriority.Alert);
        }

        private void ClearGraphics()
        {
            m_lcdCanvas.Clear(Color.White);
            m_lcdOverlay.Clear(Color.White);
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
                byte[] buffer = new byte[6880];
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

                        // Copy the content of the bitmap to our buffers 
                        // Unsafe code removes the boundaries checks - a lot faster.
                        fixed (byte* buffer0 = buffer)
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
                m_lcd.DisplayBitmap(ref buffer[0], (int)priority);
            }
        }

        /// <summary>
        /// Updates the skill queue free room info.
        /// </summary>
        private void UpdateSkillQueueFreeRoom()
        {
            TimeSpan timeLeft = DateTime.UtcNow.AddHours(24) - CurrentCharacter.SkillQueue.EndTime;
            string timeLeftText;

            // Prevents the "(none)" text from being displayed
            if (timeLeft < TimeSpan.FromSeconds(1))
                return;

            // Less than minute ? Display seconds
            if (timeLeft < TimeSpan.FromMinutes(1))
            {
                timeLeftText = Skill.TimeSpanToDescriptiveText(timeLeft, DescriptiveTextOptions.IncludeCommas);
            }
            // Display time without seconds
            else
            {
                timeLeftText = Skill.TimeSpanToDescriptiveText(timeLeft, DescriptiveTextOptions.IncludeCommas, false);
            }

            string skillQueueFreemRoom = String.Format(String.Format(CultureConstants.DefaultCulture, "{0} free room in skill queue", timeLeftText));
            SizeF size = m_lcdCanvas.MeasureString(skillQueueFreemRoom, m_defaultFont);
            RectangleF line = new RectangleF(new PointF(0f, 0f), size);
            line.Offset(0f, 11f);
            m_lcdCanvas.FillRectangle(new SolidBrush(Color.Black), 0, 13, 160, 10);
            m_lcdOverlay.DrawString(skillQueueFreemRoom, m_defaultFont, new SolidBrush(Color.Black), line);
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Formats the wallet balance value in an abbreviation form
        /// </summary>
        /// <param name="value"></param>
        /// <param name="width"></param>
        /// <returns>Abbriaviated balance value</returns>
        private string AbbreviationFormat(decimal value, float width)
        {
            string balance;
            int suffixIndex = 0;
            float newWidth;

            do
            {
                value /= 1000;
                suffixIndex++;

                switch (suffixIndex)
                {
                    case 1:
                        balance = String.Format(CultureConstants.DefaultCulture, "{0} K ISK", value.ToString("#,###.#0"));
                        break;
                    case 2:
                        balance = String.Format(CultureConstants.DefaultCulture, "{0} M ISK", value.ToString("#,###.#0"));
                        break;
                    case 3:
                        balance = String.Format(CultureConstants.DefaultCulture, "{0} B ISK", value.ToString("#,###.#0"));
                        break;
                    // We have no room to show the wallet balance
                    default:
                        balance = String.Empty;
                        break;
                }

                SizeF size = m_lcdCanvas.MeasureString(balance, m_defaultFont);
                newWidth = size.Width;
            }
            while (width < newWidth);

            return balance;
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
            if (m_oldButtonState == dwButtons)
                return 0;

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
                if (Characters == null)
                    return 0;
                
                CurrentCharacter = FirstCharacterToCompleteSkill as CCPCharacter;

                if (AutoCycleChanged != null)
                    AutoCycleChanged(false);

                SwitchState(LcdState.Character);
            }

            // Forces a refresh from CCP
            if ((press & BUTTON3_VALUE) != 0)
            {
                if (m_state == LcdState.Character || m_state == LcdState.CharacterList)
                {
                    m_refreshCharacter = CurrentCharacter;
                    if (APIUpdateRequested != null)
                        APIUpdateRequested(m_refreshCharacter);

                    SwitchState(LcdState.Refreshing);
                }
            }

            // Switch autocycle ON/OFF
            if ((press & BUTTON4_VALUE) != 0)
            {
                // switch autocycle on/off
                SwitchCycle();
                if (AutoCycleChanged != null)
                    AutoCycleChanged(m_cycle);

                SwitchState(LcdState.CycleSettings);
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
            if (Characters == null)
                return;

            // Move to next char
            m_currentCharacterIndex++;
            if (m_currentCharacterIndex >= Characters.Length)
                m_currentCharacterIndex = 0;

            // Requests new data
            if (CurrentCharacterChanged != null)
                CurrentCharacterChanged(CurrentCharacter);
        }

        /// <summary>
        /// Swutches the state and updates some of the internal times variables.
        /// </summary>
        /// <param name="state"></param>
        public void SwitchState(LcdState newState)
        {
            m_state = newState;
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

            SwitchState(LcdState.CharacterList);
        }

        /// <summary>
        /// On skill completion, switch to the display of the proper message.
        /// </summary>
        public void SkillCompleted(Character character)
        {
            CurrentCharacter = character as CCPCharacter;
            SwitchState(LcdState.SkillComplete);
        }

        /// <summary>
        /// On skill completion, switch to the display of the proper message.
        /// </summary>
        public void SkillCompleted(Character character, int skillCount)
        {
            CurrentCharacter = character as CCPCharacter;
            m_completedSkills = skillCount;
            SwitchState(LcdState.SkillComplete);
        }

        /// <summary>
        /// Switches the cycling setting.
        /// </summary>
        public void SwitchCycle()
        {
            m_cycle = !m_cycle;
        }
        #endregion
    }
}
