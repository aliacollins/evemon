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
using System.Reflection;
using System.Drawing.Text;

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
            m_cycleCompletionTimeTime = DateTime.Now.AddSeconds(5);
            m_cycle = false;
            m_showTime = false;
            m_cycleCompletionTime = false;
            m_showingCompletionTime = false;
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
        private bool m_cycleCompletionTime;
        private int m_cycleCompletionTimeInterval;
        private DateTime m_cycleCompletionTimeTime;
        private bool m_showingCompletionTime;
        private bool m_showTime;
        private Character m_refreshCharacter;
        private ButtonDelegate m_buttonDelegate;

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

        public bool ShowTime
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
        
        public bool CycleCompletionTime
        {
            get
            {
                return m_cycleCompletionTime;
            }
            set 
            {
                m_cycleCompletionTime = value;
            }
        }

        public int CycleCompletionInterval
        {
            get
            {
                return m_cycleCompletionTimeInterval;
            }
            set
            {
                m_cycleCompletionTimeInterval = value;
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
                    return "No character";
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
                if (m_currentCharacterIndex == -1) m_currentCharacterIndex = 0;
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
            if (test.TotalMilliseconds > 0) return;

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

            if (m_cycleCompletionTime)
            {
                if (TimeSpan.FromTicks(DateTime.Now.Ticks - m_cycleCompletionTimeTime.Ticks).TotalSeconds > m_cycleCompletionTimeInterval)
                {
                    m_cycleCompletionTimeTime = DateTime.Now;
                    m_showingCompletionTime = !m_showingCompletionTime;
                }
            }

            string charLine = String.Format("{0} - {1} queued skill{2}", CurrentCharacter.AdornedName, CurrentCharacter.SkillQueue.Count, CurrentCharacter.SkillQueue.Count == 1 ? "" : "s" );
            m_lcdLines.Add(new TextLine(charLine, m_defaultFont));

            if (CurrentCharacter.IsTraining)
            {
                m_lcdLines.Add(new TextLine(CurrentCharacter.SkillQueue.CurrentlyTraining.SkillName, m_defaultFont));
            }
            else
            {
                m_lcdLines.Add(new TextLine("No Skill Trianing", m_defaultFont));
            }
            
            if (CurrentCharacter.SkillQueue.IsPaused)
            {
                m_lcdLines.Add(new TextLine("Skill Queue Paused", m_defaultFont));
            }
            else if (m_showingCompletionTime)
            {
                m_lcdLines.Add(new TextLine(String.Format("Finishes {0}", DateTime.Now + TimeToComplete), m_defaultFont));
            }
            else
            {
                m_lcdLines.Add(new TextLine(Skill.TimeSpanToDescriptiveText(TimeToComplete, DescriptiveTextOptions.SpaceBetween, true), m_defaultFont));
            }

            m_lcdLines.Add(new ProgressLine(CurrentCharacterTrainingProgression, m_defaultFont));

            RenderLines();
            RenderTime();
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

        private void RenderTime()
        {
            if (m_showTime)
            {
                string curTime = DateTime.Now.ToString("T");
                SizeF size = m_lcdCanvas.MeasureString(curTime, m_defaultFont);
                RectangleF timeLine = new RectangleF(new PointF(160f - size.Width, 0f), size);
                timeLine.Offset(0f, -1f);
                m_lcdCanvas.DrawString(curTime, m_defaultFont, new SolidBrush(Color.Black), timeLine);
            }
        }

        /// <summary>
        /// Paints a message for skill completion.
        /// </summary>
        private void PaintSkillCompletionMessage() 
        {
            if (CurrentCharacter == null)
                return;

            if (CurrentCharacter.SkillQueue.CurrentlyTraining == null)
                return;

            m_lcdLines.Clear();
            m_lcdLines.Add(new TextLine(CurrentCharacter.AdornedName, m_defaultFont));
            m_lcdLines.Add(new TextLine("Has finished training", m_defaultFont));
            m_lcdLines.Add(new TextLine(CurrentCharacter.SkillQueue.CurrentlyTraining.SkillName, m_defaultFont));
            m_lcdLines.Add(new TextLine(String.Format("{0} more skills in queue", CurrentCharacter.SkillQueue.Count), m_defaultFont));

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
                m_lcdLines.Add(new TextLine("No Characters", m_defaultFont));
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
            
            string cycleMsg = String.Format("Cycle Time is: {0}s", m_cycleInterval);
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

                        // Copy the content of the bitmp to our buffers 
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
                if (Characters == null)
                    return 0;

                DateTime timeToComplete = DateTime.MaxValue;
                CCPCharacter firstComplete = null;
                foreach (CCPCharacter character in Characters)
                {
                    if (character.IsTraining && character.SkillQueue.EndTime < timeToComplete)
                    {
                        timeToComplete = character.SkillQueue.EndTime;
                        firstComplete = character;
                    }
                }

                if (firstComplete != null)
                {
                    CurrentCharacter = firstComplete;
                }

                if (AutoCycleChanged != null) AutoCycleChanged(false);
                SwitchState(LcdState.Character);
            }

            // Forces a refresh from CCP
            if ((press & BUTTON3_VALUE) != 0)
            {
                if (m_state == LcdState.Character || m_state == LcdState.CharacterList)
                {
                    m_refreshCharacter = CurrentCharacter;
                    if (APIUpdateRequested != null) APIUpdateRequested(m_refreshCharacter);
                    SwitchState(LcdState.Refreshing);
                }
            }

            // Switch autocycle ON/OFF
            if ((press & BUTTON4_VALUE) != 0)
            {
                // switch autocycle on/off
                SwitchCycle();
                if (AutoCycleChanged != null) AutoCycleChanged(m_cycle);
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
            if (Characters == null) return;

            // Move to next char
            m_currentCharacterIndex++;
            if (m_currentCharacterIndex >= Characters.Length) m_currentCharacterIndex = 0;

            // Requests new data
            if (CurrentCharacterChanged != null) CurrentCharacterChanged(CurrentCharacter);
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
        public void SkillCompleted(Character character, QueuedSkill skill)
        {
            SwitchState(LcdState.SkillComplete);
        }

        /// <summary>
        /// On skill completion, switch to the display of the proper message.
        /// </summary>
        public void SkillCompleted(Character character, int skillCount)
        {
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
