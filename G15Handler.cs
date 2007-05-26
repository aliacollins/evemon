using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using EVEMon.Common;
using EVEMon.LogitechG15;

namespace EVEMon
{
    class G15Handler
    {
        public static EVEMon.LogitechG15.Lcdisplay LCD;
        public string ActiveCharacter
        {
            get { return m_activeCharacter; }
            set { m_activeCharacter = value; }
        }

        private static Thread m_G15Thread = null;
        private static bool m_threadRunning = false;
        private static bool m_startupError = false;
        private static bool m_shouldTerminate = false;
        private static string m_activeCharacter = null;
        private static Dictionary<string, CharacterInfo> m_Chars = new Dictionary<string, CharacterInfo>();

        /// <summary>
        /// Initialises the G15 event handles
        /// </summary>
        public static void Init()
        {
            Program.Settings.UseLogitechG15DisplayChanged += new EventHandler<EventArgs>(UseLogitechG15DisplayChanged);
            // we need to start our LCD-object first so list updates can be send to it.
            Start();
        }

        /// <summary>
        /// This shuts down the lcd with its thread
        /// </summary>
        public static void Shutdown()
        {
            EVEMon.LogitechG15.Lcdisplay.OnCharNameChange -= new CharChangeHandler(OnLCDActiveCharacterChange);
            Stop();
        }

        /// <summary>
        /// Refresh the internal list of characters and un/register all involved events.
        /// </summary>
        public static void CharListUpdate()
        {
            if (Program.Settings.CharacterList.Count == 0)
            {
                return;
            }

            if (LCD == null)
            {
                return;
            }

            List<string> names = new List<string>();
            foreach (CharLoginInfo info in Program.Settings.CharacterList)
            {
                if (m_activeCharacter == null)
                {
                    m_activeCharacter = info.CharacterName;
                }
                if (m_Chars.ContainsKey(info.CharacterName))
                {
                    CharacterInfo i = m_Chars[info.CharacterName];
                }
                else
                {
                    CharacterInfo i = Program.MainWindow.GetGrandCharacterInfo(info.CharacterName);
                    if (i == null)
                    {
                        return;
                    }
                    m_Chars.Add(info.CharacterName, i);
                    i.DownloadAttemptCompleted += new CharacterInfo.DownloadAttemptCompletedHandler(DownloadAttemptCompleted);
                    i.SkillChanged += new SkillChangedHandler(SkillChangedHandler);
                }
                names.Add(info.CharacterName);
            }
            List<string> rem = new List<string>();
            foreach (string n in m_Chars.Keys)
            {
                if (!names.Contains(n))
                {
                    rem.Add(n);
                }
            }
            foreach (string r in rem)
            {
                if (m_activeCharacter == r)
                {
                    m_activeCharacter = null;
                }
                m_Chars[r].DownloadAttemptCompleted -= new CharacterInfo.DownloadAttemptCompletedHandler(DownloadAttemptCompleted);
                m_Chars[r].SkillChanged -= new SkillChangedHandler(SkillChangedHandler);
                m_Chars.Remove(r);
            }

            // we got our advanced list, update the simple in the g15 object
            List<CharacterInfo> mlist = new List<CharacterInfo>(m_Chars.Values);
            string[] temp = new string[mlist.Count];
            int i_n = 0;
            foreach (CharacterInfo sci in mlist)
            {
                if (m_activeCharacter == null)
                {
                    m_activeCharacter = sci.Name;
                }
                temp[i_n] = sci.Name;
                i_n++;
            }
            LCD.charlist = temp;

            // list updated, get the skilltime
            GetLeastTrainingSkill();
        }

        /// <summary>
        /// Updates the necessary data into the LCD every second.
        /// </summary>
        private static void GetActiveCharData()
        {
            if (m_activeCharacter == null || m_activeCharacter == "" || LCD == null)
            {
                return;
            }

            if (!m_Chars.ContainsKey(m_activeCharacter))
            {
                // looks like we have a glitch here
                // call the charlist update to either get the needed data into the array
                // or delete the current charname
                CharListUpdate();
                return;
            }

            CharacterInfo currchar = m_Chars[m_activeCharacter];
            Skill s = currchar.CurrentlyTrainingSkill;
            if (s != null)
            {
                LCD.curperc = s.GetPercentDone();
                LCD.CharacterName = m_activeCharacter;
                LCD.CurrentSkillTrainingText = s.Name + " " + Skill.GetRomanForInt(s.TrainingToLevel);

                // we might want to refactor this so it's not using the settings quite so often then add an event to update the local value when the setting changes.
                //LCD.TimeToComplete = s.EstimatedCompletion.AddSeconds(-Settings.GetInstance().NotificationOffset) - DateTime.Now;
                LCD.TimeToComplete = s.EstimatedCompletion - DateTime.Now;
            }
            else
            {
                LCD.CharacterName = m_activeCharacter;
                LCD.CurrentSkillTrainingText = "";
                LCD.TimeToComplete = TimeSpan.FromTicks(0);
                LCD.curperc = 1;
            }

        }

        /// <summary>
        /// This recalculates the skill with least time left. Called on startup and SkillChanged.
        /// </summary>
        private static void GetLeastTrainingSkill()
        {
            LCD.leasttime = TimeSpan.FromTicks(DateTime.Now.Ticks);

            foreach (CharacterInfo gci in m_Chars.Values)
            {
                Skill gs = gci.CurrentlyTrainingSkill;
                if (gs == null)
                {
                    continue;
                }

                long tmp = LCD.leasttime.Ticks - (gs.EstimatedCompletion.Ticks - DateTime.Now.Ticks);
                if (tmp > 0)
                {
                    LCD.leasttime = gs.EstimatedCompletion - DateTime.Now;
                    LCD.leastchar = gci.Name;
                }
            }
        }

        /// <summary>
        /// The handler which is called when a skill has been changed.
        /// </summary>
        private static void SkillChangedHandler(object sender, SkillChangedEventArgs e) { SkillChangedHandler(); }
        private static void SkillChangedHandler()
        {
            CharListUpdate();
        }

        /// <summary>
        /// Updates the ActiveCharacter because next update to the LCD would send the old one again.
        /// </summary>
        private static void OnLCDActiveCharacterChange(string CharName)
        {
            m_activeCharacter = CharName;
            GetActiveCharData();
        }

        /// <summary>
        /// Activates an update request in the CharacterMonitor of that character.
        /// </summary>
        private static void OnLCDRefreshChange(string CharName)
        {
            if (CharName == null)
            {
                return;
            }

            CharacterMonitor cm = Program.MainWindow.GetCharacterMonitor(CharName);
            if (cm == null)
            {
                return;
            }

            cm.UpdateCharacterInfo();
        }

        /// <summary>
        /// Sets the current autocycle state into the settings
        /// </summary>
        private static void OnLCDAutoCycleChange(bool Cycle)
        {
            Program.Settings.G15ACycle = Cycle;
        }

        /// <summary>
        /// The handler which is called when a skill has been completed.
        /// </summary>
        private static void DownloadAttemptCompleted(object sender, CharacterInfo.DownloadAttemptCompletedEventArgs e)
        {
            if (!e.Complete)
            {
                return;
            }

            string skillLevelString = Program.MainWindow.GetGrandCharacterInfo(e.CharacterName).GetSkill(e.SkillName).RomanLevel;
            LCD._COMPLETESTR = e.CharacterName + "\nhas finished learning skill\n" + e.SkillName + " " + skillLevelString;
            LCD.SkillCompleted();
        }

        /// <summary>
        /// Handler when the settings have been changed so we have to en/disable the LCD.
        /// </summary>
        private static void UseLogitechG15DisplayChanged(object sender, EventArgs e)
        {
            if (m_startupError)
            {
                return;
            }

            if (Program.Settings.UseLogitechG15Display)
            {
                Start();
                CharListUpdate();
            }
            else
            {
                Stop();
            }
        }

        private static void Start()
        {
            if (Program.Settings.UseLogitechG15Display && m_threadRunning == false && LCD == null)
            {
                try
                {
                    LCD = EVEMon.LogitechG15.Lcdisplay.Instance();
                    m_G15Thread = new Thread(ThreadStart);
                    m_G15Thread.Name = "LCD Thread";
                    m_G15Thread.Start();
                    m_threadRunning = true;
                }
                catch
                {
                    m_threadRunning = false;
                    m_startupError = true;
                    Stop();
                }

                // dont register the event handler if something has gone wrong
                if (!m_threadRunning)
                {
                    return;
                }

                // register the events
                EVEMon.LogitechG15.Lcdisplay.OnCharNameChange += new CharChangeHandler(OnLCDActiveCharacterChange);
                EVEMon.LogitechG15.Lcdisplay.OnCharRefresh += new CharRefreshHandler(OnLCDRefreshChange);
                EVEMon.LogitechG15.Lcdisplay.OnAutoCycleChange += new CharAutoCycleHandler(OnLCDAutoCycleChange);
                //EVEMon.LogitechG15.Lcdisplay.OnButtonPress += new OnButtonPressHandler(OnLCDButtonPress);

                // load the cycle settings
                LCD.cycle = Program.Settings.G15ACycle;
                LCD.cycleint = Program.Settings.G15ACycleint;

                // update our internal list
                CharListUpdate();
            }
        }

        private static void Stop()
        {
            // unregister the events
            if (LCD != null)
            {
                EVEMon.LogitechG15.Lcdisplay.OnCharNameChange -= new CharChangeHandler(OnLCDActiveCharacterChange);
                EVEMon.LogitechG15.Lcdisplay.OnCharRefresh -= new CharRefreshHandler(OnLCDRefreshChange);
                EVEMon.LogitechG15.Lcdisplay.OnAutoCycleChange -= new CharAutoCycleHandler(OnLCDAutoCycleChange);
                //EVEMon.LogitechG15.Lcdisplay.OnButtonPress -= new OnButtonPressHandler(OnLCDButtonPress);
            }

            foreach (CharacterInfo gci in m_Chars.Values)
            {
                gci.DownloadAttemptCompleted -= new CharacterInfo.DownloadAttemptCompletedHandler(DownloadAttemptCompleted);
                gci.SkillChanged -= new SkillChangedHandler(SkillChangedHandler);
            }

            m_Chars = new Dictionary<string, CharacterInfo>();
            if (m_G15Thread != null && m_G15Thread.IsAlive && LCD != null)
            {
                ThreadStop();
            }
            LCD = null;
        }

        private static void ThreadStart()
        {
            m_shouldTerminate = false;
            DateTime timer = DateTime.Now.AddSeconds(1);
            TimeSpan test = TimeSpan.FromTicks(timer.Ticks - DateTime.Now.Ticks);

            LCD.switchState(1);

            while (!m_shouldTerminate)
            {
                // every second copy the active char data into the LCD
                test = TimeSpan.FromTicks(timer.Ticks - DateTime.Now.Ticks);
                if (test.TotalMilliseconds < 0)
                {
                    GetActiveCharData();
                    timer = DateTime.Now.AddSeconds(1);
                }

                // issue the repaint every 50ms (its checked first, no resources wasted!)
                LCD.DoRepaint();
                Thread.Sleep(50);
            }
            LCD.Dispose();
        }

        private static void ThreadStop()
        {
            if (m_threadRunning)
            {
                m_shouldTerminate = true;
                m_G15Thread.Join();
                m_G15Thread.Abort();
                m_threadRunning = false;
                m_G15Thread = null;
            }
        }
    }
}
