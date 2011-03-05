using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Threading;
using EVEMon.LogitechG15;

namespace EVEMon
{
    /// <summary>
    /// This class takes care to drive the <see cref="Lcdisplay"/> according to the settings, the characters, the global events, etc. 
    /// <para>It malaxes and fetches data to the <see cref="Lcdisplay"/> while this last one holds the responsibility to cycle between chars, 
    /// format the display, and update to the device.</para>
    /// </summary>
    public static class G15Handler
    {
        private static Lcdisplay m_lcd;
        private static bool m_running;
        private static bool m_startupError;

        private static Object m_syncLock = new Object();

        #region Initialize
        /// <summary>
        /// Initialises the G15 event handles
        /// </summary>
        public static void Initialize()
        {
            EveClient.TimerTick += new EventHandler(EveClient_TimerTick);
            EveClient.QueuedSkillsCompleted += new EventHandler<QueuedSkillsEventArgs>(EveClient_QueuedSkillsCompleted);

            // Subscribe to events which occur of G15 buttons pressed
            Lcdisplay.APIUpdateRequested += new Lcdisplay.CharacterHandler(Lcdisplay_APIUpdateRequested);
            Lcdisplay.AutoCycleChanged += new Lcdisplay.CharAutoCycleHandler(Lcdisplay_AutoCycleChanged);
            Lcdisplay.CurrentCharacterChanged += new Lcdisplay.CharacterHandler(Lcdisplay_CurrentCharacterChanged);
        }       
        #endregion


        #region LCD Updater
        /// <summary>
        /// Update on every second (and when some of the G15 buttons are pressed)
        /// </summary>
        private static void UpdateOnTimerTick()
        {
            // No G15 keyboard connected
            if (m_startupError)
                return;

            // Did the state changed ?
            if (Settings.G15.Enabled != m_running)
            {
                if (!m_running)
                {
                    Start();
                    if (m_startupError)
                        return;
                }
                else
                {
                    Stop();
                }
            }

            // Run
            if (m_running)
            {
                UpdateG15Data();
                m_lcd.Paint();
            }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Starts the LCD display.
        /// </summary>
        private static void Start()
        {
            try
            {
                m_lcd = Lcdisplay.Instance();
                m_lcd.SwitchState(LcdState.SplashScreen);
                m_running = true;
            }
            catch(Exception ex)
            {
                EveClient.Trace(ex.Message);
                m_startupError = true;
                m_running = false;
            }
        }

        /// <summary>
        /// Stop the LCD display.
        /// </summary>
        private static void Stop()
        {
            try
            {
                m_lcd.Dispose();
            }
            catch (Exception ex)
            {
                EveClient.Trace(ex.Message);
            }
            finally
            {
                m_lcd = null;
                m_running = false;
            }
        }

        /// <summary>
        /// Update the display once every second
        /// </summary>
        private static void UpdateG15Data()
        {
            // Settings
            m_lcd.Cycle = Settings.G15.UseCharactersCycle;
            m_lcd.CycleInterval = Settings.G15.CharactersCycleInterval;
            m_lcd.CycleSkillQueueTime = Settings.G15.UseTimeFormatsCycle;
            m_lcd.CycleCompletionInterval = Settings.G15.TimeFormatsCycleInterval;
            m_lcd.ShowSystemTime = Settings.G15.ShowSystemTime;
            m_lcd.ShowEVETime = Settings.G15.ShowEVETime;

            // Characters names
            List<CCPCharacter> lcdCharacters = new List<CCPCharacter>();

            foreach (Character character in EveClient.MonitoredCharacters.Where(x => x is CCPCharacter))
            {
                lcdCharacters.Add(character as CCPCharacter);
            }

            m_lcd.Characters = lcdCharacters.ToArray();

            // First character to complete a skill
            var nextChar = EveClient.MonitoredCharacters.Where(x => x.IsTraining).ToArray().OrderBy(x => x.CurrentlyTrainingSkill.EndTime).FirstOrDefault();
            if (nextChar != null)
            {
                m_lcd.FirstSkillCompletionRemaingTime = nextChar.CurrentlyTrainingSkill.RemainingTime;
                m_lcd.FirstCharacterToCompleteSkill = nextChar;
            }

            // Updates the character name
            var currentCharacter = m_lcd.CurrentCharacter;
            if (currentCharacter != null)
            {
                if (currentCharacter.IsTraining)
                {
                    var skill = currentCharacter.CurrentlyTrainingSkill;

                    m_lcd.CurrentCharacter = currentCharacter;
                    m_lcd.CurrentCharacterTrainingProgression = skill.FractionCompleted;
                    m_lcd.CurrentSkillTrainingText = skill.ToString();
                    m_lcd.TimeToComplete = skill.RemainingTime;
                }
                else
                {
                    m_lcd.CurrentSkillTrainingText = String.Empty;
                    m_lcd.TimeToComplete = TimeSpan.Zero;
                    m_lcd.CurrentCharacterTrainingProgression = 0;
                }
            }
        }
        
        #endregion


        #region Event Handlers
        /// <summary>
        /// On every second, we check whether we should start ot stop the LCD display, updated its data, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void EveClient_TimerTick(object sender, EventArgs e)
        {
            UpdateOnTimerTick();
        }

        /// <summary>
        /// When skills are completed, we display a special message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void EveClient_QueuedSkillsCompleted(object sender, QueuedSkillsEventArgs e)
        {
            if (m_running)
            {
                if (e.CompletedSkills.Count == 1)
                {
                    m_lcd.SkillCompleted(e.Character);
                }
                else
                {
                    m_lcd.SkillCompleted(e.Character, e.CompletedSkills.Count);
                }
            }
        }
        #endregion


        #region Events triggered by the G15 buttons
        /// <summary>
        /// Occurs whenever the current character changed (because of a button press or cycling).
        /// </summary>
        static void Lcdisplay_CurrentCharacterChanged(Character character)
        {
            Dispatcher.Invoke(() =>
                {
                    UpdateOnTimerTick();
                });
        }

        /// <summary>
        /// Occurs whenever a G15 button has been pressed which requires EVEMon to requery the API for the specified character.
        /// </summary>
        static void Lcdisplay_APIUpdateRequested(Character character)
        {
            Dispatcher.Invoke(() =>
                {
                    var ccpCharacter = character as CCPCharacter;
                    if (ccpCharacter != null)
                        ccpCharacter.QueryMonitors.QueryEverything();
                });
        }

        /// <summary>
        /// Occurs whenever the auto cycle should change (because of a button press).
        /// </summary>
        static void Lcdisplay_AutoCycleChanged(bool cycle)
        {
            Settings.G15.UseCharactersCycle = cycle;
        }
        #endregion
    }
}
