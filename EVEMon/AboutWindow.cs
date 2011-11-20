using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon
{
    /// <summary>
    /// Displays the About Window (Help -&gt; About) containing contrib,
    /// legal and version information about the application.
    /// </summary>
    public partial class AboutWindow : EVEMonForm
    {
        private readonly SortedList m_headers;
        private readonly SortedList m_developers;

        /// <summary>
        /// Setup the list of developers and the standard font
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
            devsList.SelectedIndexChanged += devsList_SelectedIndexChanged;
            EVEMonLabel.Font = FontFactory.GetDefaultFont(8.25F, FontStyle.Bold);
            DevContribLabel.Font = FontFactory.GetDefaultFont(8.25F, FontStyle.Bold);
            CredentialsLabels.Font = FontFactory.GetDefaultFont(8.25F, FontStyle.Bold);

            // list of headings
            m_headers = new SortedList
                            {
                                { "01", "Guru" },
                                { "02", "Guru (Retired)" },
                                { "03", "Developers" },
                                { "04", "Developers (Retired)" },
                                { "05", "Contributors" }
                            };

            // list of developers by heading
            m_developers = new SortedList
                               {
                                   // EVEMon Guru
                                   { "Jimi", "01" },
                                   // Guru (Retired)
                                   { "Araan Sunn", "02" },
                                   { "Six Anari", "02" },
                                   { "Anders Chydenius", "02" },
                                   { "Brad Stone", "02" },
                                   { "Eewec Ourbyni", "02" },
                                   { "Richard Slater", "02" },
                                   { "Vehlin", "02" },
                                   // Developers
                                   { "MrCue", "03" },
                                   { "Nericus Demeeny", "03" },
                                   { "Tonto Auri", "03" },
                                   // Developers (Retired)
                                   { "Collin Grady", "04" },
                                   { "DCShadow", "04" },
                                   { "DonQuiche", "04" },
                                   { "Grauw", "04" },
                                   { "Jalon Mevek", "04" },
                                   { "Labogh", "04" },
                                   { "romanl", "04" },
                                   { "Safrax", "04" },
                                   { "Stevil Knevil", "04" },
                                   { "TheBelgarion", "04" },
                                   // Contributors
                                   { "Abomb", "05" },
                                   { "Adam Butt", "05" },
                                   { "Aethlyn", "05" },
                                   { "Aevum Decessus", "05" },
                                   { "aliceturing", "05" },
                                   { "aMUSiC", "05" },
                                   { "Arengor", "05" },
                                   { "ATGardner", "05" },
                                   { "Barend", "05" },
                                   { "bugusnot", "05" },
                                   { "Candle", "05" },
                                   { "coeus", "05" },
                                   { "CrazyMahone", "05" },
                                   { "CyberTech", "05" },
                                   { "Dariana", "05" },
                                   { "Eviro", "05" },
                                   { "exi", "05" },
                                   { "FangVV", "05" },
                                   { "Femaref", "05" },
                                   { "Flash", "05" },
                                   { "Galideeth", "05" },
                                   { "gareth", "05" },
                                   { "gavinl", "05" },
                                   { "GoneWacko", "05" },
                                   { "happyslinky", "05" },
                                   { "jdread", "05" },
                                   { "Jeff Zellner", "05" },
                                   { "jthiesen", "05" },
                                   { "justinian", "05" },
                                   { "Kelos Pelmand", "05" },
                                   { "Kingdud", "05" },
                                   { "Kw4h", "05" },
                                   { "lerthe61", "05" },
                                   { "Lexiica", "05" },
                                   { "Master of Dice", "05" },
                                   { "Maximilian Kernbach", "05" },
                                   { "MaZ", "05" },
                                   { "mexx24", "05" },
                                   { "Michayel Lyon", "05" },
                                   { "mintoko", "05" },
                                   { "misterilla", "05" },
                                   { "Moq", "05" },
                                   { "morgangreenacre", "05" },
                                   { "Namistai", "05" },
                                   { "Nascent Nimbus", "05" },
                                   { "NetMage", "05" },
                                   { "Nagapito", "05" },
                                   { "Nilyen", "05" },
                                   { "Nimrel", "05" },
                                   { "Niom", "05" },
                                   { "Pharazon", "05" },
                                   { "Phoenix Flames", "05" },
                                   { "phorge", "05" },
                                   { "Optica", "05" },
                                   { "Risako", "05" },
                                   { "Ruldar", "05" },
                                   { "Safarian Lanar", "05" },
                                   { "scoobyrich", "05" },
                                   { "Sertan Deras", "05" },
                                   { "shaver", "05" },
                                   { "Shocky", "05" },
                                   { "Shwehan Juanis", "05" },
                                   { "skolima", "05" },
                                   { "Spiff Nutter", "05" },
                                   { "Subkahnshus", "05" },
                                   { "The_Assimilator", "05" },
                                   { "TheConstructor", "05" },
                                   { "Trin", "05" },
                                   { "vardoj", "05" },
                                   { "Waste Land", "05" },
                                   { "wrok", "05" },
                                   { "xNomeda", "05" },
                                   { "ykoehler", "05" },
                                   { "Zarra Kri", "05" },
                                   { "Zofu", "05" }
                               };
        }

        /// <summary>
        /// Prevents the user to select an item in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void devsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (devsList.SelectedItems.Count != 0)
                devsList.SelectedItems.Clear();
        }

        /// <summary>
        /// Populates and adds links to the various labels and list
        /// boxes on the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutWindow_Load(object sender, EventArgs e)
        {
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            VersionLabel.Text = String.Format(CultureConstants.DefaultCulture, VersionLabel.Text, currentVersion);

            AddDevelopersToListView();

            if (EveMonClient.IsDebugBuild)
                AddDebugTag();

            AddLinkToLabel(ccpGamesLinkLabel, "CCP Games", "http://www.ccpgames.com/");
            AddLinkToLabel(battleclinicLinkLabel, "BattleClinic", "http://www.battleclinic.com/");
            AddLinkToLabel(eveCentralLinkLabel, "Eve-central", "http://www.eve-central.com/");
            AddLinkToLabel(eveDevLinkLabel, "EVEDev", "http://www.eve-dev.net/");
            AddLinkToLabel(googleDataLinkLabel, "Google.Data", "http://code.google.com/apis/gdata/client-cs.html");
            AddLinkToLabel(lironLeviLinkLabel, "Liron Levi", "http://www.codeproject.com/KB/cs/multipanelcontrol.aspx");
            AddLinkToLabel(stackOverflowLinkLabel, "Stack Overflow", "http://stackoverflow.com");
        }

        /// <summary>
        /// Little function to allow us to add links to a link label
        /// after the contents has been set, purely by the contained
        /// text
        /// </summary>
        /// <remarks>
        /// At present this function only works on the first instance
        /// of a string within the text property of the link label,
        /// further insances will be ignored.
        /// </remarks>
        /// <param name="label">LinkLabel to act upon</param>
        /// <param name="linkText">text to make a link</param>
        /// <param name="url">URL for the link to point to</param>
        private static void AddLinkToLabel(LinkLabel label, String linkText, String url)
        {
            int start = label.Text.IndexOf(linkText, StringComparison.Ordinal);
            int length = linkText.Length;

            label.Links.Add(start, length, url);
        }

        /// <summary>
        /// Adds " (Debug)" to the verison number if the build is in DEBUG.
        /// </summary>
        private void AddDebugTag()
        {
            VersionLabel.Text = String.Format(CultureConstants.DefaultCulture, "{0} (Debug)", VersionLabel.Text);
        }

        /// <summary>
        /// Loops through the list of headings and developers and adds
        /// them to the list box.
        /// </summary>
        private void AddDevelopersToListView()
        {
            devsList.Columns.Add(new ColumnHeader());

            // Set up the list of developers.
            for (int i = 0; i < m_headers.Count; i++)
            {
                ListViewGroup group = new ListViewGroup(m_headers.GetByIndex(i).ToString());
                devsList.Groups.Add(group);

                for (int j = 0; j < m_developers.Count; j++)
                {
                    if (m_headers.GetKey(i) != m_developers.GetByIndex(j))
                        continue;
                    ListViewItem item = new ListViewItem(m_developers.GetKey(j).ToString(), group);
                    devsList.Items.Add(item);
                }
            }

            devsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the LinkClicked event of the llHomePage LinkLabel.
        /// Navigates to the EVEMon website in a browser.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void llHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(new Uri(NetworkConstants.EVEMonMainPage));
        }

        /// <summary>
        /// Handles the LinkClicked event of the IconSourceLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData.GetType() != typeof(String))
                return;

            try
            {
                Uri linkUri = new Uri(e.Link.LinkData.ToString());
                Util.OpenURL(linkUri);
            }
            catch (UriFormatException ex)
            {
                // uri is malformed, never mind just ignore it
                ExceptionHandler.LogException(ex, true);
            }
        }
    }
}