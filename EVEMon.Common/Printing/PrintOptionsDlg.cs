using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Printing;

namespace EVEMon.Printing
{
    public partial class PrintOptionsDlg : Form
    {
        EVEMon.Common.PlanTextOptions m_pto;
        EVEMon.Common.Settings m_settings;

        public PrintOptionsDlg(EVEMon.Common.Settings settings, EVEMon.Common.PlanTextOptions pto,PrintDocument doc)
        {
            InitializeComponent();

            int index ;
            string curPrinter = doc.PrinterSettings.PrinterName;
            
            m_pto = pto;
            m_settings = settings;

            foreach (String printer in PrinterSettings.InstalledPrinters)
            {
                index = comboPrinters.Items.Add(printer);

                doc.PrinterSettings.PrinterName = printer;
                if (doc.PrinterSettings.IsDefaultPrinter)
                    comboPrinters.SelectedIndex = index;
            }

            // if this dialog is cancelled, we dont want the name of the printer to have changed
            doc.PrinterSettings.PrinterName = curPrinter;

            EntryFinishDate = pto.EntryFinishDate;
            EntryNumber = pto.EntryNumber;
            EntryStartDate = pto.EntryStartDate;
            EntryTrainingTimes = pto.EntryTrainingTimes;
            FooterCount = pto.FooterCount;
            FooterDate = pto.FooterDate;
            FooterTotalTime = pto.FooterTotalTime;
            IncludeHeader = pto.IncludeHeader;
            
        }

        public String PrinterName
        {
            get { return comboPrinters.Items[comboPrinters.SelectedIndex].ToString()  ; }
        }

        public bool EntryFinishDate
        {
            get { return checkFinishDate.Checked; }
            set { checkFinishDate.Checked = value; }
        }

        public bool EntryNumber
        {
            get { return checkEntryNumber.Checked; }
            set { checkEntryNumber.Checked = value; }
        }

        public bool EntryStartDate
        {
            get { return checkStartDate.Checked; }
            set { checkStartDate.Checked = value; }
        }

        public bool EntryTrainingTimes
        {
            get { return checkTrainingTimes.Checked; }
            set { checkTrainingTimes.Checked = value; }
        }

        public bool FooterCount
        {
            get { return checkPageNumbers.Checked; }
            set { checkPageNumbers.Checked = value; }
        }

        public bool FooterDate
        {
            get { return checkDateInformation.Checked; }
            set { checkDateInformation.Checked = value; }
        }

        public bool FooterTotalTime
        {
            get { return checkTotalTimes.Checked; }
            set { checkTotalTimes.Checked = value; }
        }

        public bool IncludeHeader
        {
            get { return checkPageHeaders.Checked; }
            set { checkPageHeaders.Checked = value; }
        }

        private void OnSetAsDefaults(object sender, EventArgs e)
        {
            OnAccept(sender,e) ;
            m_settings.SaveImmediate();
        }

        private void OnAccept(object sender, EventArgs e)
        {
            m_pto.EntryFinishDate = EntryFinishDate;
            m_pto.EntryNumber = EntryNumber;
            m_pto.EntryStartDate = EntryStartDate;
            m_pto.EntryTrainingTimes = EntryTrainingTimes;
            m_pto.FooterCount = FooterCount;
            m_pto.FooterDate = FooterDate;
            m_pto.FooterTotalTime = FooterTotalTime;
            m_pto.IncludeHeader = IncludeHeader;
        }
    }
}
