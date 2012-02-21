using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.About
{
    /// <summary>
    /// Displays the About Window (Help -&gt; About) containing contrib,
    /// legal and version information about the application.
    /// </summary>
    public partial class AboutWindow : EVEMonForm
    {
        private readonly SortedList m_headers;
        private readonly SortedList m_developersList;

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
                                { 1, "Guru" },
                                { 2, "Guru (Retired)" },
                                { 3, "Developers" },
                                { 4, "Developers (Retired)" },
                                { 5, "Contributors" }
                            };

            // list of developers by heading
            m_developersList = new SortedList
                               {
                                   // EVEMon Guru
                                   { "Jimi", 1 },
                                   // Guru (Retired)
                                   { "Araan Sunn", 2 },
                                   { "Six Anari", 2 },
                                   { "Anders Chydenius", 2 },
                                   { "Brad Stone", 2 },
                                   { "Eewec Ourbyni", 2 },
                                   { "Richard Slater", 2 },
                                   { "Vehlin", 2 },
                                   // Developers
                                   { "MrCue", 3 },
                                   { "Nericus Demeeny", 3 },
                                   { "Tonto Auri", 3 },
                                   // Developers (Retired)
                                   { "Collin Grady", 4 },
                                   { "DCShadow", 4 },
                                   { "DonQuiche", 4 },
                                   { "Grauw", 4 },
                                   { "Jalon Mevek", 4 },
                                   { "Labogh", 4 },
                                   { "romanl", 4 },
                                   { "Safrax", 4 },
                                   { "Stevil Knevil", 4 },
                                   { "TheBelgarion", 4 },
                                   // Contributors
                                   { "Abomb", 5 },
                                   { "Adam Butt", 5 },
                                   { "Aethlyn", 5 },
                                   { "Aevum Decessus", 5 },
                                   { "aliceturing", 5 },
                                   { "aMUSiC", 5 },
                                   { "Arengor", 5 },
                                   { "ATGardner", 5 },
                                   { "Barend", 5 },
                                   { "bugusnot", 5 },
                                   { "Candle", 5 },
                                   { "coeus", 5 },
                                   { "CrazyMahone", 5 },
                                   { "CyberTech", 5 },
                                   { "Dariana", 5 },
                                   { "Eviro", 5 },
                                   { "exi", 5 },
                                   { "FangVV", 5 },
                                   { "Femaref", 5 },
                                   { "Flash", 5 },
                                   { "Galideeth", 5 },
                                   { "gareth", 5 },
                                   { "gavinl", 5 },
                                   { "GoneWacko", 5 },
                                   { "happyslinky", 5 },
                                   { "jdread", 5 },
                                   { "Jeff Zellner", 5 },
                                   { "jthiesen", 5 },
                                   { "justinian", 5 },
                                   { "Kelos Pelmand", 5 },
                                   { "Kingdud", 5 },
                                   { "Kw4h", 5 },
                                   { "lerthe61", 5 },
                                   { "Lexiica", 5 },
                                   { "Master of Dice", 5 },
                                   { "Maximilian Kernbach", 5 },
                                   { "MaZ", 5 },
                                   { "mexx24", 5 },
                                   { "Michayel Lyon", 5 },
                                   { "mintoko", 5 },
                                   { "misterilla", 5 },
                                   { "Moq", 5 },
                                   { "morgangreenacre", 5 },
                                   { "Namistai", 5 },
                                   { "Nascent Nimbus", 5 },
                                   { "NetMage", 5 },
                                   { "Nagapito", 5 },
                                   { "Nilyen", 5 },
                                   { "Nimrel", 5 },
                                   { "Niom", 5 },
                                   { "Pharazon", 5 },
                                   { "Phoenix Flames", 5 },
                                   { "phorge", 5 },
                                   { "Optica", 5 },
                                   { "Risako", 5 },
                                   { "Ruldar", 5 },
                                   { "Safarian Lanar", 5 },
                                   { "scoobyrich", 5 },
                                   { "Sertan Deras", 5 },
                                   { "shaver", 5 },
                                   { "Shocky", 5 },
                                   { "Shwehan Juanis", 5 },
                                   { "skolima", 5 },
                                   { "Spiff Nutter", 5 },
                                   { "Subkahnshus", 5 },
                                   { "The_Assimilator", 5 },
                                   { "TheConstructor", 5 },
                                   { "Trin", 5 },
                                   { "vardoj", 5 },
                                   { "Waste Land", 5 },
                                   { "wrok", 5 },
                                   { "xNomeda", 5 },
                                   { "ykoehler", 5 },
                                   { "Zarra Kri", 5 },
                                   { "Zofu", 5 }
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

            // Set up the list of developers
            for (int i = 0; i < m_headers.Count; i++)
            {
                ListViewGroup group = new ListViewGroup(m_headers.GetByIndex(i).ToString());
                devsList.Groups.Add(group);

                for (int j = 0; j < m_developersList.Count; j++)
                {
                    if (!m_headers.GetKey(i).Equals(m_developersList.GetByIndex(j)))
                        continue;

                    ListViewItem item = new ListViewItem(m_developersList.GetKey(j).ToString(), group);
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