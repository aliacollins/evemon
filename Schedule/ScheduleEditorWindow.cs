using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Schedule;
using System.Drawing;

namespace EVEMon.Schedule
{
    public partial class ScheduleEditorWindow : EVEMonForm
    {
        private ToolTip ttToolTip = null;
        private List<ScheduleEntry> lbEntriesData = null;

        public ScheduleEditorWindow()
        {
            InitializeComponent();

            ttToolTip = new ToolTip();
            ttToolTip.IsBalloon = true;
            ttToolTip.UseAnimation = true;
            ttToolTip.UseFading = true;
            ttToolTip.AutoPopDelay = 10000;
            ttToolTip.ReshowDelay = 100;
            ttToolTip.InitialDelay = 500;
            ttToolTip.ToolTipIcon = ToolTipIcon.Info;
        }

        void calControl_DayClicked(DateTime datetime, MouseEventArgs mouse, Point location)
        {
            // Show Bubble with all Events on left click, context menu on right click
            if (mouse.Button == MouseButtons.Left)
            {
                // How can you only localize the date?
                string title = "Entries for " + datetime.ToString("d");

                string content = String.Empty;
                foreach (ScheduleEntry entry in m_settings.Schedule)
                {
                    if (entry.IsToday(datetime))
                    {
                        DateTime from = datetime;
                        DateTime to = datetime;

                        if (entry is SimpleScheduleEntry)
                        {
                            SimpleScheduleEntry simple = (SimpleScheduleEntry)entry;

                            from = simple.StartDateTime;
                            to = simple.EndDateTime;
                        }
                        else
                        {                          
                            RecurringScheduleEntry recurring = (RecurringScheduleEntry)entry;

                            // Does this always have one entry?
                            IEnumerable<ScheduleDateTimeRange> ranges = recurring.GetRangesInPeriod(new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0), new DateTime(datetime.Year, datetime.Month, datetime.Day, 23, 59, 59));                            
                            IEnumerator<ScheduleDateTimeRange> enumranges = ranges.GetEnumerator();
                            while (enumranges.MoveNext())
                            {
                                ScheduleDateTimeRange r = enumranges.Current;
                                from = r.From;
                                to = r.To;
                            }
                        }

                        // If the "from" date is before the selected date
                        if (!(from.Year == datetime.Year && from.Month == datetime.Month && from.Day == datetime.Day))
                        {
                            // Set date to midnight today
                            from = new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0);
                        }

                        // If the "to" date is after the selected date
                        if (!(to.Year == datetime.Year && to.Month == datetime.Month && to.Day == datetime.Day))
                        {
                            // Set date to last second before tomorrows midnight
                            to = new DateTime(datetime.Year, datetime.Month, datetime.Day, 23, 59, 59);
                        }

                        content += entry.Title;
                        content += " [ ";
                        content += from.ToString("HH:mm") + " - " + to.ToString("HH:mm");
                        content += " ] ";
                        content += "\n";
                    }
                }

                ttToolTip.ToolTipTitle = title;
                ttToolTip.SetToolTip(calControl, content);
                ttToolTip.Active = true;
            }
            else if (mouse.Button == MouseButtons.Right)
            {
                // Remove old Entries
                while(calContext.Items.Count > 2) {
                    calContext.Items.RemoveAt(2);
                }

                // Set Date Tag to new entry
                calContext.Items[0].Tag = datetime;

                foreach (ScheduleEntry entry in m_settings.Schedule)
                {
                    if (entry.IsToday(datetime))
                    {
                        ToolStripItem item = new ToolStripMenuItem();
                        item.Text = "Edit \"" + entry.Title + "\"...";
                        item.Tag = entry;
                        item.Click += new EventHandler(contextItem_Click);

                        calContext.Items.Add(item);
                    }
                }

                calContext.Show(calControl, location);
            }
        }

        private void contextItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                object tag = ((ToolStripMenuItem)sender).Tag;

                if(tag is DateTime) {

                    DateTime datetime = (DateTime)tag;

                    using (EditScheduleEntryWindow f = new EditScheduleEntryWindow(datetime))
                    {
                        DialogResult dr = f.ShowDialog();
                        if (dr == DialogResult.Cancel)
                        {
                            return;
                        }
                        m_settings.ScheduleAdd(f.ScheduleEntry);
                        lbEntries.Items.Add(f.ScheduleEntry.Title);
                        m_settings.Save();

                        UpdateEntries();
                    }
                }
                else if (tag is ScheduleEntry)
                {
                    ScheduleEntry temp = (ScheduleEntry)tag;
                    using (EditScheduleEntryWindow f = new EditScheduleEntryWindow())
                    {
                        f.ScheduleEntry = temp;
                        DialogResult dr = f.ShowDialog();
                        if (dr == DialogResult.Cancel)
                        {
                            return;
                        }
                        // need to put something in here to test to see if the item has actually been changed, and if not, simply return
                        int i = -1;
                        for (int x = 0; x < m_settings.Schedule.Count && i == -1; x++)
                        {
                            if (m_settings.Schedule[x].Equals(temp))
                            {
                                i = x;
                            }
                        }
                        m_settings.Schedule.RemoveAt(i);
                        m_settings.ScheduleAdd(f.ScheduleEntry);
                        m_settings.Save();

                        UpdateEntries();
                    }
                }
            }
        }

        private void calControl_DayDoubleClicked(DateTime datetime, MouseEventArgs mouse, Point loc)
        {
            using (EditScheduleEntryWindow f = new EditScheduleEntryWindow(datetime))
            {
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
                m_settings.ScheduleAdd(f.ScheduleEntry);
                lbEntries.Items.Add(f.ScheduleEntry.Title);
                m_settings.Save();

                UpdateEntries();
            }
        }

        public ScheduleEditorWindow(Settings s)
            : this()
        {
            m_settings = s;
            UpdateEntries();
        }

        private void UpdateEntries()
        {
            UpdateListBoxEntries();
            UpdateCalendarEntries();
        }

        private void UpdateCalendarEntries()
        {
            calControl.Entries.Clear();
            foreach (ScheduleEntry temp in m_settings.Schedule)
            {
                calControl.Entries.Add(temp);
            }
            calControl.Invalidate();
        }

        private void UpdateListBoxEntries()
        {
            lbEntriesData = null;
            lbEntriesData = new List<ScheduleEntry>(m_settings.Schedule);

            lbEntriesData.Sort(new ScheduleEntryComparisonTitle());

            lbEntries.Items.Clear();
            foreach (ScheduleEntry temp in lbEntriesData)
            {
                lbEntries.Items.Add(temp.Title);
            }
        }

        private Settings m_settings;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            using (EditScheduleEntryWindow f = new EditScheduleEntryWindow())
            {
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
                m_settings.ScheduleAdd(f.ScheduleEntry);
                lbEntries.Items.Add(f.ScheduleEntry.Title);
                m_settings.Save();

                UpdateEntries();
            }
        }

        private DateTime currentdate = DateTime.Now;

        private void ScheduleEditorWindow_Load(object sender, EventArgs e)
        {
            nudMonth.Items.Clear();
            string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            nudMonth.Items.Add(monthNames[0]);
            for (int i = 1; i <= CultureInfo.CurrentCulture.Calendar.GetMonthsInYear(currentdate.Year); i++)
            {
                nudMonth.Items.Add(
                    monthNames[((CultureInfo.CurrentCulture.Calendar.GetMonthsInYear(currentdate.Year)) - i)]);
            }
            nudMonth.Items.Add(monthNames[CultureInfo.CurrentCulture.Calendar.GetMonthsInYear(currentdate.Year) - 1]);
            nudYear.Value = currentdate.Year;
            nudMonth.SelectedIndex = ((nudMonth.Items.Count - 1) - currentdate.Month);
            nudDay.Maximum = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month) + 1;
            nudDay.Value = currentdate.Day;
            calControl.Date = currentdate;
        }

        private void changedYear(object sender, EventArgs e)
        {
            int oldyearnum = currentdate.Year;
            currentdate = currentdate.AddYears((int)nudYear.Value - currentdate.Year);
            if (currentdate.Month == 2 &&
                (CultureInfo.CurrentCulture.Calendar.IsLeapYear(currentdate.Year) ||
                 CultureInfo.CurrentCulture.Calendar.IsLeapYear(oldyearnum)))
            {
                nudDay.Maximum =
                    CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month) + 1;
                if (CultureInfo.CurrentCulture.Calendar.IsLeapYear(oldyearnum) && nudDay.Value > nudDay.Maximum)
                {
                    nudDay.Value = nudDay.Maximum;
                }
            }
            calControl.Date = currentdate;
        }

        private void changedDay(object sender, EventArgs e)
        {
            bool donex = false;
            bool doney = false;
            if (nudDay.Value == 0)
            {
                if (nudMonth.SelectedIndex == (nudMonth.Items.Count - 2) && nudYear.Value == nudYear.Minimum)
                {
                    nudDay.Value = 1;
                }
                currentdate = currentdate.AddDays((int)nudDay.Value - currentdate.Day);
                nudDay.Maximum =
                    CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month) + 1;
                if (nudDay.Value == 0)
                {
                    nudDay.Value =
                        CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month);
                }
                donex = true;
            }
            else if (!donex && nudDay.Value == nudDay.Maximum)
            {
                if (nudMonth.SelectedIndex == 1 && nudYear.Value == nudYear.Maximum)
                {
                    nudDay.Value =
                        CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month);
                    doney = true;
                }
                else if (!doney)
                {
                    currentdate = currentdate.AddDays((int)nudDay.Value - currentdate.Day);
                    nudDay.Value = 1;
                    nudDay.Maximum =
                        CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month) + 1;
                }
                donex = true;
            }
            else if (!donex)
            {
                currentdate = currentdate.AddDays((int)nudDay.Value - currentdate.Day);
            }
            calControl.Date = currentdate;
            nudYear.Value = currentdate.Year;
            nudMonth.SelectedIndex = (CultureInfo.CurrentCulture.Calendar.GetMonthsInYear(currentdate.Year) -
                                      currentdate.Month) + 1;
        }

        private void changedMonth(object sender, EventArgs e)
        {
            if (nudMonth.SelectedIndex == nudMonth.Items.Count - 1 && nudYear.Value == nudYear.Minimum)
            {
                nudMonth.SelectedIndex = nudMonth.Items.Count - 2;
            }
            if (nudMonth.SelectedIndex == 0 && nudYear.Value == nudYear.Maximum)
            {
                nudMonth.SelectedIndex = 1;
            }
            currentdate =
                currentdate.AddMonths((((nudMonth.Items.Count - 1) - nudMonth.SelectedIndex) - currentdate.Month));
            nudMonth.SelectedIndex = ((nudMonth.SelectedIndex + (nudMonth.Items.Count - 3)) % (nudMonth.Items.Count - 2)) +
                                     1;
            if (nudDay.Value > CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month))
            {
                nudDay.Value = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month);
            }
            nudDay.Maximum = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(currentdate.Year, currentdate.Month) + 1;
            calControl.Date = currentdate;
            nudYear.Value = currentdate.Year;
        }

        private void tsbDeleteEntry_Click(object sender, EventArgs e)
        {
            if (lbEntries.SelectedIndex != -1)
            {
                int i = -1;
                for (int x = 0; x < m_settings.Schedule.Count && i == -1; x++)
                {
                    if (m_settings.Schedule[x].Title == lbEntries.Items[lbEntries.SelectedIndex].ToString())
                    {
                        i = x;
                    }
                }
                lbEntries.Items.RemoveAt(lbEntries.SelectedIndex);
                m_settings.ScheduleRemoveAt(i);
                m_settings.Save();

                UpdateEntries(); 
            }
        }

        private void tsbClearExpired_Click(object sender, EventArgs e)
        {
            List<ScheduleEntry> unexpiredEntries = new List<ScheduleEntry>();
            lbEntries.Items.Clear();
            foreach (ScheduleEntry se in m_settings.Schedule)
            {
                if (!se.Expired)
                {
                    lbEntries.Items.Add(se.Title);
                    unexpiredEntries.Add(se);
                }
            }
            m_settings.Schedule = unexpiredEntries;
            m_settings.Save();

            UpdateEntries();   
        }

        private void lbEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbEntries.SelectedIndex != -1)
            {
                ScheduleEntry temp = lbEntriesData[lbEntries.SelectedIndex];
                string label_text = "Title: " + temp.Title;
                if (temp.GetType() == typeof(SimpleScheduleEntry))
                {
                    SimpleScheduleEntry x = (SimpleScheduleEntry)temp;
                    label_text = label_text + "\nOne Off Entry\n Start: " + x.StartDateTime + "\n End: " + x.EndDateTime + "\n Expired: " + x.Expired;
                    label_text += "\n\n Options\n  Blocking: " + ((x.ScheduleEntryOptions & ScheduleEntryOptions.Blocking) != 0);
                    label_text += "\n  Silent: " + ((x.ScheduleEntryOptions & ScheduleEntryOptions.Quiet) != 0);
                    label_text += "\n  Uses Eve Time: " + ((x.ScheduleEntryOptions & ScheduleEntryOptions.EVETime) != 0);
                }
                else if (temp.GetType() == typeof(RecurringScheduleEntry))
                {
                    RecurringScheduleEntry x = (RecurringScheduleEntry)temp;
                    label_text = label_text + "\nRecurring Entry:\n Start: " + x.RecurStart + "\n End: " + x.RecurEnd + "\n Frequency: " + x.RecurFrequency;
                    if (x.RecurFrequency == RecurFrequency.Monthly)
                    {
                        label_text = label_text + "\n  Day of Month: " + x.RecurDayOfMonth + "\n  On Overflow: " + x.OverflowResolution;
                    }
                    else if (x.RecurFrequency == RecurFrequency.Weekly)
                    {
                        DateTime nowish = DateTime.Now.Date;
                        DateTime Initial = x.RecurStart.AddDays((x.RecurDayOfWeek - x.RecurStart.DayOfWeek + 7) % 7);
                        Double datediff = ((7 * x.nWeekly) - (nowish.Subtract(Initial).Days % (7 * x.nWeekly))) % (7 * x.nWeekly);
                        if (((nowish.AddDays(datediff)).Add(TimeSpan.FromSeconds(x.StartSecond))) < DateTime.Now)
                            datediff = datediff + (7 * x.nWeekly);
                        label_text = label_text + "\n  Day of Week: " + x.RecurDayOfWeek + "\n  Every: " + x.nWeekly + " weeks\n  Next: " + (nowish.AddDays(datediff)).Add(TimeSpan.FromSeconds(x.StartSecond));
                    }
                    label_text = label_text + "\n Start Time: " + TimeSpan.FromSeconds(x.StartSecond).ToString() + "\n End Time: " + TimeSpan.FromSeconds(x.EndSecond).ToString() + "\n Expired: " + x.Expired;
                    label_text += "\n Options\n  Blocking: " + ((x.ScheduleEntryOptions & ScheduleEntryOptions.Blocking) != 0);
                    label_text += "\n  Silent: " + ((x.ScheduleEntryOptions & ScheduleEntryOptions.Quiet) != 0);
                    label_text += "\n  Uses Eve Time: " + ((x.ScheduleEntryOptions & ScheduleEntryOptions.EVETime) != 0);
                }
                else
                {
                    // ?? Wha...
                    label_text = "What the Smeg is this?";
                }
                lblEntryDescription.Text = label_text;
            }
            else
            {
                lblEntryDescription.Text = "";
            }
        }

        private void lbEntries_DoubleClick(object sender, EventArgs e)
        {
            if (lbEntries.SelectedIndex != -1)
            {
                ScheduleEntry temp = lbEntriesData[lbEntries.SelectedIndex];
                using (EditScheduleEntryWindow f = new EditScheduleEntryWindow())
                {
                    f.ScheduleEntry = temp;
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.Cancel)
                    {
                        return;
                    }
                    // need to put something in here to test to see if the item has actually been changed, and if not, simply return
                    int i = -1;
                    for (int x = 0; x < m_settings.Schedule.Count && i == -1; x++)
                    {
                        if (m_settings.Schedule[x].Equals(temp))
                        {
                            i = x;
                        }
                    }
                    lbEntries.Items.RemoveAt(lbEntries.SelectedIndex);
                    m_settings.Schedule.RemoveAt(i);
                    m_settings.ScheduleAdd(f.ScheduleEntry);
                    lbEntries.Items.Add(f.ScheduleEntry.Title);
                    m_settings.Save();

                    UpdateEntries();
                }
            }
        }

        private void calControl_MouseLeave(object sender, EventArgs e)
        {
            ttToolTip.Active = false;
        }

        private void calControl_MouseEnter(object sender, EventArgs e)
        {
            ttToolTip.Active = false;
        }
    }
}
