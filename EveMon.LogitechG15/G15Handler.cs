using System;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Threading;

namespace EVEMon.LogitechG15
{
    /// <summary>
    /// This class takes care to drive the <see cref="LcdDisplay"/> according to the settings, the characters, the global events, etc. 
    /// <para>It malaxes and fetches data to the <see cref="LcdDisplay"/> while this last one holds the responsibility to cycle between chars, 
    /// format the display, and update to the device.</para>
    /// </summary>
    public static class G15Handler
    {
        private static LcdDisplay s_lcd;
        private static bool s_running;
        private static bool s_startupError;


        #region Initialize

        /// <summary>
        /// Initialises the G15 event handles
        /// </summary>
        public static void Initialize()
        {
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.QueuedSkillsCompleted += EveMonClient_QueuedSkillsCompleted;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;

            // Subscribe to events which occur of G15 buttons pressed
            LcdDisplay.ApiUpdateRequested += LcdDisplay_APIUpdateRequested;
            LcdDisplay.AutoCycleChanged += LcdDisplay_AutoCycleChanged;
            LcdDisplay.CurrentCharacterChanged += LcdDisplay_CurrentCharacterChanged;
        }

        #endregion


        #region LCD Updater

        /// <summary>
        /// Update on every second (and when some of the G15 buttons are pressed)
        /// </summary>
        private static void UpdateOnTimerTick()
        {
            // No G15 keyboard connected
            if (s_startupError)
                return;

            // Did the state changed ?
            if (Settings.G15.Enabled != s_running)
            {
                if (!s_running)
                {
                    Start();
                    if (s_startupError)
                        return;
                }
                else
                    Stop();
            }

            // Run
            if (!s_running)
                return;

            UpdateG15Data();
            s_lcd.Paint();
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
                s_lcd = LcdDisplay.Instance();
                s_lcd.SwitchState(LcdState.SplashScreen);
                s_running = true;
                UpdateFromSettings();

                // Initialize the current character
                s_lcd.CurrentCharacter = EveMonClient.MonitoredCharacters.OfType<CCPCharacter>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                EveMonClient.Trace(ex.Message);
                s_startupError = true;
                s_running = false;
            }
        }

        /// <summary>
        /// Stop the LCD display.
        /// </summary>
        private static void Stop()
        {
            try
            {
                s_lcd.Dispose();
            }
            catch (Exception ex)
            {
                EveMonClient.Trace(ex.Message);
            }
            finally
            {
                s_lcd = null;
                s_running = false;
            }
        }

        /// <summary>
        /// Updates preferences from settings.
        /// </summary>
        private static void UpdateFromSettings()
        {
            if (s_lcd == null || !s_running)
                return;

            s_lcd.Cycle = Settings.G15.UseCharactersCycle;
            s_lcd.CycleInterval = Settings.G15.CharactersCycleInterval;
            s_lcd.CycleSkillQueueTime = Settings.G15.UseTimeFormatsCycle;
            s_lcd.CycleCompletionInterval = Settings.G15.TimeFormatsCycleInterval;
            s_lcd.ShowSystemTime = Settings.G15.ShowSystemTime;
            s_lcd.ShowEVETime = Settings.G15.ShowEVETime;
        }

        /// <summary>
        /// Update the display once every second.
        /// </summary>
        private static void UpdateG15Data()
        {
            // First character to complete a skill
            CCPCharacter nextChar = EveMonClient.MonitoredCharacters.OfType<CCPCharacter>().Where(
                x => x.IsTraining).OrderBy(x => x.CurrentlyTrainingSkill.EndTime).FirstOrDefault();

            if (nextChar != null)
                s_lcd.FirstCharacterToCompleteSkill = nextChar;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// On every second, we check whether we should start ot stop the LCD display, updated its data, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            UpdateOnTimerTick();
        }

        /// <summary>
        /// When skills are completed, we display a special message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void EveMonClient_QueuedSkillsCompleted(object sender, QueuedSkillsEventArgs e)
        {
            if (!s_running)
                return;

            s_lcd.SkillCompleted(e.Character, e.CompletedSkills.Count);
        }

        /// <summary>
        /// Update the preferences when the settings change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateFromSettings();
        }

        #endregion


        #region Events triggered by the G15 buttons

        /// <summary>
        /// Occurs whenever the current character changed (because of a button press or cycling).
        /// </summary>
        private static void LcdDisplay_CurrentCharacterChanged(object sender, CharacterChangedEventArgs e)
        {
            Dispatcher.Invoke(UpdateOnTimerTick);
        }

        /// <summary>
        /// Occurs whenever a G15 button has been pressed which requires EVEMon to requery the API for the specified character.
        /// </summary>
        private static void LcdDisplay_APIUpdateRequested(object sender, CharacterChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
                                  {
                                      CCPCharacter ccpCharacter = e.Character as CCPCharacter;
                                      if (ccpCharacter != null)
                                      {
                                          ccpCharacter.QueryMonitors.Query(new Enum[]
                                                                               {
                                                                                   APICharacterMethods.CharacterSheet,
                                                                                   APICharacterMethods.SkillQueue
                                                                               });
                                      }
                                  });
        }

        /// <summary>
        /// Occurs whenever the auto cycle should change (because of a button press).
        /// </summary>
        private static void LcdDisplay_AutoCycleChanged(object sender, CycleEventArgs e)
        {
            Settings.G15.UseCharactersCycle = e.Cycle;
        }

        #endregion
    }
}