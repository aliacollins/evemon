using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common;
using System.Text;
using System.Collections;
using System.Drawing;

namespace EVEMon
{
    public partial class AboutWindow : EVEMonForm
    {
        SortedList slPriority;
        SortedList slDevelopers;
        SortedList slOutput;
        public Font myFont;
        public Font myFontbold;

        public AboutWindow()
        {
            InitializeComponent();

            myFont = new System.Drawing.Font("Tahoma", 8);
            myFontbold = new System.Drawing.Font("Tahoma", 8, FontStyle.Bold);

            slPriority = new SortedList();
            slPriority.Add("01", "EVEMon Guru:");
            slPriority.Add("02", "Guru (Retired):");
            slPriority.Add("03", "Developers:");
            slPriority.Add("04", "Developers (Retired):");
            slPriority.Add("05", "Contributors:");

            slDevelopers = new SortedList();
            slDevelopers.Add("Brad Stone", "01");

            slDevelopers.Add("Six Anari", "02");
            slDevelopers.Add("Anders Chydenius", "02");
            slDevelopers.Add("Eewec Ourbyni", "02");

            slDevelopers.Add("Araan Sunn", "03");
            slDevelopers.Add("mrcue", "03");
            slDevelopers.Add("Collin Grady", "03");
            slDevelopers.Add("Grauw", "03");

            slDevelopers.Add("DCShadow", "04");
            slDevelopers.Add("Jalon Mevek", "04");
            slDevelopers.Add("Labogh", "04");
            slDevelopers.Add("romanl", "04");
            slDevelopers.Add("Safrax", "04");
            slDevelopers.Add("Stevil Knevil", "04");
            slDevelopers.Add("TheBelgarion", "04");

            slDevelopers.Add("Adam Butt", "05");
            slDevelopers.Add("bugusnot", "05");
            slDevelopers.Add("Candle","05");
            slDevelopers.Add("cybertech", "05");
            slDevelopers.Add("Dariana", "05");
            slDevelopers.Add("exi", "05");
            slDevelopers.Add("FangVV", "05");
            slDevelopers.Add("Femaref", "05");
            slDevelopers.Add("Flash", "05");
            slDevelopers.Add("Galideeth", "05");
            slDevelopers.Add("gareth", "05");
            slDevelopers.Add("gavinl", "05");
            slDevelopers.Add("GoneWacko", "05");
            slDevelopers.Add("happyslinky", "05");
            slDevelopers.Add("justinian", "05");
            slDevelopers.Add("jdread", "05");
            slDevelopers.Add("Kw4h", "05");
            slDevelopers.Add("Lexiica", "05");
            slDevelopers.Add("Maximilian Kernbach", "05");
            slDevelopers.Add("Michayel Lyon", "05");
            slDevelopers.Add("mintoko", "05");
            slDevelopers.Add("Namistai", "05");
            slDevelopers.Add("Nascent Nimbus", "05");
            slDevelopers.Add("NetMage", "05");
            slDevelopers.Add("Nilyen", "05");
            slDevelopers.Add("Nimrel", "05");
            slDevelopers.Add("PeteWilson", "05");
            slDevelopers.Add("Phoenix Flames", "05");
            slDevelopers.Add("phorge", "05");
            slDevelopers.Add("Richard Slater", "05");
            slDevelopers.Add("Ruldar", "05");
            slDevelopers.Add("scoobyrich", "05");
            slDevelopers.Add("Sertan Deras", "05");
            slDevelopers.Add("shaver", "05");
            slDevelopers.Add("Shocky", "05");
            slDevelopers.Add("Shwehan Juanis", "05");
            slDevelopers.Add("skolima", "05");
            slDevelopers.Add("Spiff Nutter", "05");
            slDevelopers.Add("The_Assimilator", "05");
            slDevelopers.Add("vardoj", "05");
            slDevelopers.Add("xNomeda", "05");

            slOutput = new SortedList();
        }

        private void AboutWindow_Load(object sender, EventArgs e)
        {
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            lblVersion.Text = String.Format(lblVersion.Text, currentVersion.ToString());
            AddDevelopersToListBox();
        }

        private void AddDevelopersToListBox()
        {
            // Set up the list of developers.
            int iOrder = 0;
            bool bAddGroup;
            for (int i = 0; i < slPriority.Count; i++)
            {
                bAddGroup = true;
                for (int j = 0; j < slDevelopers.Count; j++)
                {
                    if (slPriority.GetKey(i) == slDevelopers.GetByIndex(j))
                    {
                        if (bAddGroup)
                        {
                            slOutput.Add(iOrder++, slPriority.GetByIndex(i));
                            bAddGroup = false;
                        }
                        slOutput.Add(iOrder++, slDevelopers.GetKey(j));
                    }
                }
            }
            for (int i = 0; i < slOutput.Count; i++)
            {
                lstDevelopers.Items.Add(slOutput.GetByIndex(i));
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void llHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EveSession.BrowserLinkClicked("http://evemon.battleclinic.com/");
        }

        private void lstDevelopers_DrawItem(object sender,
        System.Windows.Forms.DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                string sLine = slOutput.GetByIndex(e.Index).ToString();
                if (sLine.Contains(":"))
                {
                    //sLine = sLine + ":";
                    e.Graphics.DrawString(sLine, myFontbold, Brushes.Black, new Point(e.Bounds.X, e.Bounds.Y));
                }
                else
                {
                    sLine = "  " + sLine;
                    e.Graphics.DrawString(sLine, myFont, Brushes.Black, new Point(e.Bounds.X, e.Bounds.Y));
                }
            }
        }
    }
}
