using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EVEMon.Common;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Text;

namespace EVEMon
{
    /// <summary>
    /// Displays a Windows-style tooltip
    /// </summary>
    public partial class TrayTooltipWindow : Form
    {
        private CharacterCollection m_characters;
        private String m_tooltip;
        private bool m_autoRefresh;

        public TrayTooltipWindow() : this(null) {}

        public TrayTooltipWindow(List<CharacterMonitor> characters)
        {
            InitializeComponent();
            m_characters = new CharacterCollection();
            if (characters != null) m_characters.AddRange(characters);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Look and Feel
            this.Font = new Font(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
            string tooltipFormat = Settings.GetInstance().TooltipString;
            // Construct the initial array of tooltip strings
            m_autoRefresh = false;
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(tooltipFormat))
                sb.Append("You can configure this tooltip in the options/general panel");
            else if (m_characters.CharactersTraining.Count == 0)
                sb.Append("No Characters in training!");
            else
            {
                m_autoRefresh = true;
                foreach (CharacterMonitor cm in m_characters.CharactersTraining)
                {
                    if (sb.Length != 0) sb.Append("\n");
                    sb.Append(FormatTooltipText(tooltipFormat, cm.GrandCharacterInfo));
                }
            }
            m_tooltip = sb.ToString();
            UpdateForm();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (m_autoRefresh) displayTimer.Start();
            // Equivalent to setting TopMost = true, except don't activate the window.
            NativeMethods.SetWindowPos(this.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
                NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
            // Show the window without activating it.
            NativeMethods.ShowWindow(this.Handle, NativeMethods.SW_SHOWNOACTIVATE);
        }

        protected override void OnClosed(EventArgs e)
        {
            displayTimer.Stop();
            base.OnClosed(e);
        }
        private void UpdateForm()
        {
            this.SuspendLayout();
            string toolTip = m_tooltip;
            foreach (CharacterMonitor cm in m_characters.CharactersTraining)
            {
                Skill trainingSkill = cm.GrandCharacterInfo.CurrentlyTrainingSkill;
                TimeSpan ts = trainingSkill.EstimatedCompletion - DateTime.Now;
                toolTip = Regex.Replace(toolTip, '%' + cm.GrandCharacterInfo.CharacterId.ToString() + 'r',
                    Skill.TimeSpanToDescriptiveText(ts, DescriptiveTextOptions.IncludeCommas), RegexOptions.Compiled);
            }
            lblToolTip.Text = toolTip;
            this.ResumeLayout();
            TrayIcon.SetToolTipLocation(this);
        }

        private string FormatTooltipText(string toolTipFormat, CharacterInfo character)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Regex.Replace(toolTipFormat, "%([nbsdr]|[ct][ir])", new MatchEvaluator(delegate(Match m)
            {
                string value = String.Empty;
                char capture = m.Groups[1].Value[0];

                if (capture == 'n')
                {
                    value = character.Name;
                }
                else if (capture == 'b')
                {
                    value = character.Balance.ToString("#,##0.00");
                }
                else if (capture == 's')
                {
                    value = character.CurrentlyTrainingSkill.Name;
                }
                else if (capture == 'd')
                {
                    value = character.CurrentlyTrainingSkill.EstimatedCompletion.ToString("g");
                }
                else if (capture == 'r')
                {
                    value = '%' + character.CharacterId.ToString() + 'r';
                }
                else
                {
                    int level = -1;
                    if (capture == 'c')
                    {
                        level = character.CurrentlyTrainingSkill.Level;
                    }
                    else if (capture == 't')
                    {
                        level = character.CurrentlyTrainingSkill.TrainingToLevel;
                    }

                    if (m.Groups[1].Value.Length > 1 && level >= 0)
                    {
                        capture = m.Groups[1].Value[1];

                        if (capture == 'i')
                        {
                            value = level.ToString();
                        }
                        else if (capture == 'r')
                        {
                            value = Skill.GetRomanForInt(level);
                        }
                    }
                }

                return value;
            }), RegexOptions.Compiled));
            return sb.ToString();
        }

        private class CharacterCollection : List<CharacterMonitor>
        {
            public CharacterCollection CharactersTraining
            {
                get
                {
                    CharacterCollection selectedChars = new CharacterCollection();
                    foreach (CharacterMonitor cm in this)
                    {
                        if (cm.GrandCharacterInfo.IsTraining)
                            selectedChars.Add(cm);
                    }
                    selectedChars.Sort(new CompletionTimeComparer());
                    return selectedChars;
                }
            }
            
            private class CompletionTimeComparer : IComparer<CharacterMonitor>
            {
                public int Compare(CharacterMonitor x, CharacterMonitor y)
                {
                    Skill skillX = x.GrandCharacterInfo.CurrentlyTrainingSkill;
                    Skill skillY = y.GrandCharacterInfo.CurrentlyTrainingSkill;
                    if (skillX == null && skillY == null)
                    {
                        return x.CharacterName.CompareTo(y.CharacterName);
                    }
                    else if (skillX == null && skillY != null)
                        return -1;
                    else if (skillX != null && skillY == null)
                        return 1;
                    else
                    {
                        if (skillX.EstimatedCompletion < skillY.EstimatedCompletion)
                            return -1;
                        else if (skillY.EstimatedCompletion == skillY.EstimatedCompletion)
                            return 0;
                        else
                            return 1;
                    }
                }
            }
        }
        #region Native Stuff
        internal class NativeMethods
        {
            public const Int32 HWND_TOPMOST = -1;
            public const Int32 SWP_NOACTIVATE = 0x0010;
            public const Int32 SWP_NOSIZE = 0x0001;
            public const Int32 SWP_NOMOVE = 0x0002;
            public const Int32 SW_SHOWNOACTIVATE = 4;

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, Int32 flags);
            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd,
                Int32 hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, uint uFlags);

        }
        #endregion

        private void displayTimer_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

    }
}

