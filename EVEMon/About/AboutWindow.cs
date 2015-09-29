using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;

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
                                { 3, "Consultants"},
                                { 4, "Developers" },
                                { 5, "Developers (Retired)" },
                                { 6, "Contributors" }
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
                                   // Consultants
                                   { "MrCue", 3 },
                                   { "Nericus Demeeny", 3 },
                                   { "Tonto Auri", 3 },
                                   // Developers (Retired)
                                   { "Collin Grady", 5 },
                                   { "DCShadow", 5 },
                                   { "DonQuiche", 5 },
                                   { "Grauw", 5 },
                                   { "Jalon Mevek", 5 },
                                   { "Labogh", 5 },
                                   { "romanl", 5 },
                                   { "Safrax", 5 },
                                   { "Stevil Knevil", 5 },
                                   { "TheBelgarion", 5 },
                                   // Contributors
                                   { "Abomb", 6 },
                                   { "Adam Butt", 6 },
                                   { "Aethlyn", 6 },
                                   { "Aevum Decessus", 6 },
                                   { "aliceturing", 6 },
                                   { "aMUSiC", 6 },
                                   { "Arengor", 6 },
                                   { "ATGardner", 6 },
                                   { "Barend", 6 },
                                   { "berin", 6 },
                                   { "bugusnot", 6 },
                                   { "Candle", 6 },
                                   { "coeus", 6 },
                                   { "CrazyMahone", 6 },
                                   { "CyberTech", 6 },
                                   { "Derath Ellecon", 6 },
                                   { "Dariana", 6 },
                                   { "Eviro", 6 },
                                   { "exi", 6 },
                                   { "FangVV", 6 },
                                   { "Femaref", 6 },
                                   { "Flash", 6 },
                                   { "Galideeth", 6 },
                                   { "gareth", 6 },
                                   { "gavinl", 6 },
                                   { "GoneWacko", 6 },
                                   { "Good speed", 6 },
                                   { "happyslinky", 6 },
                                   { "Jazzy_Josh", 6 },
                                   { "jdread", 6 },
                                   { "Jeff Zellner", 6 },
                                   { "jthiesen", 6 },
                                   { "justinian", 6 },
                                   { "Kelos Pelmand", 6 },
                                   { "Kingdud", 6 },
                                   { "Kw4h", 6 },
                                   { "Kunnis Niam", 6 },
                                   { "lerthe61", 6 },
                                   { "Lexiica", 6 },
                                   { "Master of Dice", 6 },
                                   { "Maximilian Kernbach", 6 },
                                   { "MaZ", 6 },
                                   { "mexx24", 6 },
                                   { "Michayel Lyon", 6 },
                                   { "mintoko", 6 },
                                   { "misterilla", 6 },
                                   { "Moq", 6 },
                                   { "morgangreenacre", 6 },
                                   { "Namistai", 6 },
                                   { "Nascent Nimbus", 6 },
                                   { "NetMage", 6 },
                                   { "Nagapito", 6 },
                                   { "Nilyen", 6 },
                                   { "Nimrel", 6 },
                                   { "Niom", 6 },
                                   { "Pharazon", 6 },
                                   { "Phoenix Flames", 6 },
                                   { "phorge", 6 },
                                   { "Protag", 6 },
                                   { "Optica", 6 },
                                   { "Quantix Blackstar", 6 },
                                   { "Risako", 6 },
                                   { "Ruldar", 6 },
                                   { "Safarian Lanar", 6 },
                                   { "scoobyrich", 6 },
                                   { "Sertan Deras", 6 },
                                   { "shaver", 6 },
                                   { "Shocky", 6 },
                                   { "Shwehan Juanis", 6 },
                                   { "skolima", 6 },
                                   { "Spiff Nutter", 6 },
                                   { "Subkahnshus", 6 },
                                   { "SyndicateAexeron", 6 },
                                   { "The_Assimilator", 6 },
                                   { "TheConstructor", 6 },
                                   { "Travis Puderbaugh", 6 },
                                   { "Trin", 6 },
                                   { "vardoj", 6 },
                                   { "Waste Land", 6 },
                                   { "wrok", 6 },
                                   { "xNomeda", 6 },
                                   { "ykoehler", 6 },
                                   { "Zarra Kri", 6 },
                                   { "Zofu", 6 }
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
            CopyrightLabel.Text = String.Format(CultureConstants.DefaultCulture, CopyrightLabel.Text, DateTime.UtcNow.Year);
            VersionLabel.Text = GetVersionText();

            AddDevelopersToListView();

            AddLinkToLabel(ccpGamesLinkLabel, "CCP Games", "http://www.ccpgames.com/");
            AddLinkToLabel(battleclinicLinkLabel, "BattleClinic", "http://www.battleclinic.com/");
            AddLinkToLabel(eveCentralLinkLabel, "EVE-Central", "http://www.eve-central.com/");
            AddLinkToLabel(eveDevLinkLabel, "EVEDev", "http://wiki.eve-id.net/Main_Page");
            AddLinkToLabel(googleDataLinkLabel, "Google.Data", "http://code.google.com/apis/gdata/client-cs.html");
            AddLinkToLabel(lironLeviLinkLabel, "Liron Levi", "http://www.codeproject.com/KB/cs/multipanelcontrol.aspx");
            AddLinkToLabel(stackOverflowLinkLabel, "Stack Overflow", "http://stackoverflow.com");
        }

        /// <summary>
        /// Gets the version text.
        /// </summary>
        /// <returns></returns>
        private string GetVersionText()
        {
            // Adds environment process info
            VersionLabel.Text += String.Format(CultureConstants.InvariantCulture, " ({0} bit)",
                Environment.Is64BitProcess ? "64" : "32");

            // Returns the product version if the build is in SNAPSHOT
            if (EveMonClient.IsSnapshotBuild)
                return String.Format(CultureConstants.DefaultCulture, VersionLabel.Text, Application.ProductVersion);
            
            // Adds " (Debug)" to the version number if the build is in DEBUG
            if (EveMonClient.IsDebugBuild)
                return String.Format(CultureConstants.DefaultCulture, VersionLabel.Text + " (Debug)", Application.ProductVersion);

            // Returns only the Major, Minor and Build of the version number
            return String.Format(CultureConstants.DefaultCulture, VersionLabel.Text,
                Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf(".", StringComparison.Ordinal)));
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