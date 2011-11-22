using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Serialization.BattleClinic;

namespace EVEMon.PatchXmlCreator
{
    public partial class PatchXmlCreatorWindow : EVEMonForm
    {
        #region Fields

        private static readonly Dictionary<Control, String> s_listOfInitMessages = new Dictionary<Control, String>();
        private static readonly List<Datafile> s_datafiles = new List<Datafile>();
        private static readonly CultureInfo s_enUsCulture = new CultureInfo("en-US");

        internal const string Caption = "Patch Xml File Creator";
        internal const string EVEMonExecFilename = "EVEMon.exe";
        internal const string EVEMonExecDir = @"..\..\..\..\..\EVEMon\bin\x86\Release";

        private const string DateTimeFormat = "dd MMMM yyyy";
        private const string DatafilesMessageFormat = "{0} {1} ({2}) {3} data file by the EVEMon Development Team";
        internal const string InstallerDir = @"..\..\..\..\..\EVEMon\bin\x86\Installbuilder\Installer";
        internal const string InstallerFilename = "EVEMon-install-{0}.exe";

        private const string PatchFilename = "patch.xml";
        private const string PatchDir = @"..\..\..\..\Website";
        private const string DatafileDir = @"..\..\..\..\..\EVEMon.Common\Resources";
        private const string DatafileHeader = "eve-";
        private const string DatafileTail = "-en-US.xml.gz";

        private const string InstallerArgs = "/S /AUTORUN /SKIPDOTNET";
        private const string AdditionalArgs = "/D=%EVEMON_EXECUTABLE_PATH%";

        private readonly bool m_newRelease;

        private Control m_activeTextBox;
        private string m_text;
        private bool m_init;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private PatchXmlCreatorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PatchXmlCreatorWindow(bool newRelease)
            : this()
        {
            m_newRelease = newRelease;
        }


        #endregion


        #region Properties

        /// <summary>
        /// Get EVEMon's assembly version.
        /// </summary>
        internal static string AssemblyVersion
        {
            get { return AssemblyName.GetAssemblyName(Path.Combine(EVEMonExecDir, EVEMonExecFilename)).Version.ToString(); }
        }

        #endregion


        #region Events

        /// <summary>
        /// On load we create the window and update the controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitDatafiles();
            CustomLayout();
            StoreInitMessage();
            UpdateReleaseInfo();
            UpdateDatafilesControls();
            UpdateCreateButtonEnabled();

            MinimumSize = new Size(Width, Height);

            m_init = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClick(object sender, EventArgs e)
        {
            ((Control)sender).Focus();
            btnCreate.Focus();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Updates the info in the release section.
        /// </summary>
        private void UpdateReleaseInfo()
        {
            if (!m_newRelease)
            {
                LoadReleaseInfoFromFile();
                return;
            }

            string installerFile = String.Format(CultureConstants.InvariantCulture, InstallerFilename, AssemblyVersion);
            string installerPath = String.Format(CultureConstants.InvariantCulture, "{1}{0}{2}", Path.DirectorySeparatorChar,
                                                 InstallerDir, installerFile);
            FileInfo installerFileInfo = new FileInfo(installerPath);

            // Assign info
            lblEVEMonVersion.Text = AssemblyVersion;
            dtpRelease.Value = (File.Exists(installerPath) ? installerFileInfo.LastWriteTime : DateTime.Now);
            lblMD5Sum.Text = Util.CreateMD5From(installerPath);
        }

        /// <summary>
        /// Creates a list of the datafiles found in the release folder.
        /// </summary>
        private static void InitDatafiles()
        {
            DirectoryInfo di = new DirectoryInfo(DatafileDir);
            FileInfo[] directoryFiles = di.GetFiles(String.Format(CultureConstants.InvariantCulture, "{0}*{1}",
                                                                  DatafileHeader, DatafileTail));
            foreach (FileInfo datafile in directoryFiles)
            {
                s_datafiles.Add(new Datafile(datafile.Name));
            }
        }

        /// <summary>
        /// Adds a control for each data file.
        /// </summary>
        private void CustomLayout()
        {
            int startLocation = 70;
            const int Pad = 5;

            gbDatafiles.Controls.Remove(datafileControl);
            Height -= datafileControl.Height;

            SuspendLayout();
            try
            {
                foreach (Datafile datafile in s_datafiles.OrderBy(x => x.Filename))
                {
                    // Add a new datafile control
                    DatafileControl newDatafileControl = new DatafileControl();
                    gbDatafiles.Controls.Add(newDatafileControl);

                    // Control info
                    UpdateDatafileInfo(newDatafileControl, datafile);

                    // Set Properties
                    newDatafileControl.Location = new Point(9, startLocation);
                    newDatafileControl.Font = new Font(Font, FontStyle.Regular);
                    newDatafileControl.Anchor |= AnchorStyles.Right;
                    newDatafileControl.Size = new Size(gbDatafiles.Width - (Pad * 3), newDatafileControl.Height);

                    // Calculate window height and next control point
                    Height += datafileControl.Height + Pad;
                    startLocation += datafileControl.Height + Pad;

                    // Subscribe Events
                    newDatafileControl.rtbDatafileMessage.Enter += Control_Enter;
                    newDatafileControl.rtbDatafileMessage.Leave += Control_Leave;
                    newDatafileControl.rtbDatafileMessage.DoubleClick += Control_DoubleClick;
                }
            }
            finally
            {
                // Update the message of each data file control
                UpdateDatafilesMessage();

                ResumeLayout();
            }

            CenterToScreen();
        }

        /// <summary>
        /// Updates the info in the data files section.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="datafile"></param>
        private static void UpdateDatafileInfo(DatafileControl control, Datafile datafile)
        {
            // Data file info
            FileInfo fileInfo = new FileInfo(Path.Combine(DatafileDir, datafile.Filename));

            // Assign info
            control.gbDatafile.Text = datafile.Filename;
            control.lblMD5Sum.Text = datafile.MD5Sum;
            control.dtpDatafiles.Value = fileInfo.LastWriteTime;
        }

        /// <summary>
        /// Updates the data file's header message according to the info provided.
        /// </summary>
        private void UpdateDatafilesMessage()
        {
            foreach (Datafile datafile in s_datafiles)
            {
                foreach (DatafileControl dfControl in gbDatafiles.Controls.OfType<DatafileControl>().Where(
                    x => x != null && x.gbDatafile.Text == datafile.Filename))
                {
                    dfControl.rtbDatafileMessage.BackColor = SystemColors.Window;
                    dfControl.rtbDatafileMessage.ForeColor = SystemColors.WindowText;

                    if (m_init)
                        EnsureHeaderMessage(dfControl.rtbDatafileMessage);
                    else
                    {
                        dfControl.rtbDatafileMessage.Text = String.Format(
                            s_enUsCulture, DatafilesMessageFormat, tbExpansion.Text, tbExpVersion.Text, tbExpRevision.Text,
                            datafile.Filename.Replace(DatafileHeader, String.Empty).Replace(DatafileTail, String.Empty));
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that each data file message has an appropriate message header.
        /// </summary>
        /// <param name="control"></param>
        private void EnsureHeaderMessage(Control control)
        {
            StringBuilder sb = new StringBuilder();

            // Remove any existing header and text that is before the header
            control.Text = (control.Text.Contains("\n")
                                ? control.Text.Remove(0, control.Text.IndexOf("\n", StringComparison.OrdinalIgnoreCase) + 1)
                                : control.Text.Remove(0, control.Text.LastIndexOf("m", StringComparison.Ordinal) + 1));

            // Create the new header text
            string headerText = String.Format(s_enUsCulture, DatafilesMessageFormat, tbExpansion.Text, tbExpVersion.Text,
                                              tbExpRevision.Text,
                                              control.Parent.Text.Replace(DatafileHeader, String.Empty).Replace(DatafileTail,
                                                                                                                String.Empty));

            // Check if the new header text is already present and remove it
            if (control.Text.Contains(headerText))
                control.Text = control.Text.Replace(headerText, String.Empty);

            // Assing the remaining text to a new variable
            string newText = control.Text;

            // Add the new header text to string builder
            sb.Append(headerText);

            // Check the remaining text and add it accordingly
            if (!String.IsNullOrEmpty(newText))
            {
                if (!newText.StartsWith("\n", StringComparison.OrdinalIgnoreCase))
                    sb.AppendLine();

                sb.Append(newText);
            }

            // Assing the new text to control text
            control.Text = sb.ToString();
        }

        /// <summary>
        /// Stores the initial texts in the text control for use in the create button enabling.
        /// </summary>
        private void StoreInitMessage()
        {
            // Store the texts from the release section excluding the update url
            foreach (RichTextBox control in gbRelease.Controls.OfType<RichTextBox>().Where(x => x != null && x != rtbReleaseUrl))
            {
                s_listOfInitMessages.Add(control, control.Text);
            }

            // Store the text from the datafiles section excluding the update url
            foreach (Control control in gbDatafiles.Controls.Cast<Control>().Where(
                x => x != rtbDatafileUrl))
            {
                if (control is TextBox || control is RichTextBox)
                    s_listOfInitMessages.Add(control, control.Text);

                DatafileControl dfControl = control as DatafileControl;
                if (dfControl != null)
                    s_listOfInitMessages.Add(control, dfControl.rtbDatafileMessage.Text);
            }
        }

        /// <summary>
        /// Checks if all conditions are met to update the data file's message header.
        /// </summary>
        private void UpdateDatafilesControls()
        {
            bool updateDatafilesText = true;

            // Look into datafiles controls
            foreach (TextBox control in gbDatafiles.Controls.OfType<TextBox>().Where(x => x != null))
            {
                control.BackColor = SystemColors.Window;
                control.ForeColor = SystemColors.WindowText;

                if ((s_listOfInitMessages.FirstOrDefault(x => x.Key == control).Value == control.Text))
                {
                    control.ForeColor = SystemColors.Highlight;
                    updateDatafilesText = false;
                }

                if (!String.IsNullOrEmpty(control.Text))
                    continue;

                control.BackColor = SystemColors.Highlight;
                updateDatafilesText = false;
            }

            // If all conditions are met update the messages
            if (updateDatafilesText)
                UpdateDatafilesMessage();
        }

        /// <summary>
        /// Checks if all conditions are met to enable the create button.
        /// </summary>
        private void UpdateCreateButtonEnabled()
        {
            // Look into release controls, datafiles controls and datafileControl controls
            bool buttonEnable = ButtonEnabledFromReleaseControls() &&
                                ButtonEnabledFromDatafileControls() &&
                                ButtonEnabledFromDatafileControlControls();

            // Enable/Disable Create button
            btnCreate.Enabled = buttonEnable;
        }

        /// <summary>
        /// "Create" button gets enabled from release controls.
        /// </summary>
        /// <returns></returns>
        private bool ButtonEnabledFromReleaseControls()
        {
            bool buttonEnable = true;
            foreach (RichTextBox control in gbRelease.Controls.OfType<RichTextBox>().Where(x => x != null))
            {
                control.BackColor = SystemColors.Window;
                control.ForeColor = SystemColors.WindowText;

                if ((s_listOfInitMessages.FirstOrDefault(x => x.Key == control).Value == control.Text))
                {
                    control.ForeColor = SystemColors.Highlight;
                    buttonEnable = false;
                }

                if (String.IsNullOrEmpty(control.Text))
                    control.BackColor = SystemColors.Highlight;
                else if (control == rtbReleaseMessage || (!Path.GetInvalidPathChars().Any(
                    invalidChar => control.Text.Contains(invalidChar)) && !control.Text.Contains("#")))
                {
                    continue;
                }

                control.ForeColor = SystemColors.Highlight;
                buttonEnable = false;
            }
            return buttonEnable;
        }

        /// <summary>
        /// Create button gets enabled from datafile controls.
        /// </summary>
        /// <returns></returns>
        private bool ButtonEnabledFromDatafileControls()
        {
            bool buttonEnable = true;
            foreach (Control control in gbDatafiles.Controls.Cast<Control>().Where(x => x is TextBox || x is RichTextBox))
            {
                control.BackColor = SystemColors.Window;
                control.ForeColor = SystemColors.WindowText;
                if ((s_listOfInitMessages.FirstOrDefault(x => x.Key == control).Value == control.Text))
                {
                    control.ForeColor = SystemColors.Highlight;
                    buttonEnable = false;
                }

                if (String.IsNullOrEmpty(control.Text))
                    control.BackColor = SystemColors.Highlight;
                else if (control != rtbDatafileUrl || (!Path.GetInvalidPathChars().Any(
                    invalidChar => control.Text.Contains(invalidChar)) && !control.Text.Contains("#")))
                {
                    continue;
                }

                control.ForeColor = SystemColors.Highlight;
                buttonEnable = false;
            }
            return buttonEnable;
        }

        /// <summary>
        /// Create button gets enabled from datafile control controls.
        /// </summary>
        /// <returns></returns>
        private bool ButtonEnabledFromDatafileControlControls()
        {
            bool buttonEnable = true;
            foreach (DatafileControl dfControl in gbDatafiles.Controls.OfType<DatafileControl>().Where(x => x != null))
            {
                dfControl.rtbDatafileMessage.BackColor = SystemColors.Window;
                dfControl.rtbDatafileMessage.ForeColor = SystemColors.WindowText;

                if (dfControl.rtbDatafileMessage.Text.Contains(s_listOfInitMessages.FirstOrDefault(x => x.Key == dfControl).Value))
                {
                    dfControl.rtbDatafileMessage.ForeColor = SystemColors.Highlight;
                    buttonEnable = false;
                }

                if (!String.IsNullOrEmpty(dfControl.rtbDatafileMessage.Text))
                    continue;

                dfControl.rtbDatafileMessage.BackColor = SystemColors.Highlight;
                buttonEnable = false;
            }
            return buttonEnable;
        }

        /// <summary>
        /// Deserializes the existing patch file.
        /// </summary>
        /// <returns></returns>
        private static SerializablePatch TryDeserializePatchXml()
        {
            String filePath = Path.Combine(PatchDir, PatchFilename);

            SerializablePatch xmlDoc = File.Exists(filePath) ? Util.DeserializeXML<SerializablePatch>(filePath) : null;

            return xmlDoc;
        }

        /// <summary>
        /// Serializes the patch file to string.
        /// </summary>
        /// <returns></returns>
        private string ExportPatchXml()
        {
            SerializablePatch serial = new SerializablePatch();

            ExportRelease(serial.Release);
            ExportDatafiles(serial.Datafiles);

            XmlDocument doc = (XmlDocument)Util.SerializeToXmlDocument(serial.GetType(), serial);
            return (doc != null ? Util.GetXMLStringRepresentation(doc) : String.Empty);
        }

        /// <summary>
        /// Serializes the release info for the patch file.
        /// </summary>
        /// <param name="serialRelease"></param>
        /// <returns></returns>
        private void ExportRelease(SerializableRelease serialRelease)
        {
            serialRelease.Date = dtpRelease.Value.ToString(DateTimeFormat, s_enUsCulture);
            serialRelease.Version = lblEVEMonVersion.Text;
            serialRelease.TopicAddress = rtbTopicUrl.Text;
            serialRelease.PatchAddress = String.Concat(rtbReleaseUrl.Text,
                                                       String.Format(CultureConstants.InvariantCulture, InstallerFilename,
                                                                     lblEVEMonVersion.Text));
            serialRelease.MD5Sum = lblMD5Sum.Text;
            serialRelease.InstallerArgs = InstallerArgs;
            serialRelease.AdditionalArgs = AdditionalArgs;
            serialRelease.Message = rtbReleaseMessage.Text.Trim();
        }

        /// <summary>
        /// Serializes the data files info for the patch file.
        /// </summary>
        /// <param name="datafiles"></param>
        /// <returns></returns>
        private void ExportDatafiles(ICollection<SerializableDatafile> datafiles)
        {
            string url = String.Format(CultureConstants.InvariantCulture, "{1}{2}{0}{3}",
                                       Path.AltDirectorySeparatorChar, rtbDatafileUrl.Text, tbExpansion.Text, tbExpRevision.Text);

            foreach (Datafile datafile in s_datafiles)
            {
                SerializableDatafile serialDatafile = new SerializableDatafile();
                datafiles.Add(serialDatafile);

                foreach (DatafileControl dfControl in gbDatafiles.Controls.OfType<DatafileControl>().Where(
                    x => x != null && x.gbDatafile.Text == datafile.Filename))
                {
                    serialDatafile.Name = dfControl.gbDatafile.Text;
                    serialDatafile.Date = dfControl.dtpDatafiles.Value.ToString(DateTimeFormat, s_enUsCulture);
                    serialDatafile.MD5Sum = dfControl.lblMD5Sum.Text;
                    serialDatafile.Address = url;
                    serialDatafile.Message = dfControl.rtbDatafileMessage.Text.Trim();
                }
            }
        }

        /// <summary>
        /// Creates the patch xml file.
        /// </summary>
        private void SaveFile()
        {
            string patch = ExportPatchXml();
            string filenamePath = Path.Combine(PatchDir, PatchFilename);

            try
            {
                FileHelper.OverwriteOrWarnTheUser(filenamePath,
                                                  fs =>
                                                      {
                                                          using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                                                          {
                                                              sw.Write(patch);
                                                              sw.Flush();
                                                              fs.Flush();
                                                          }
                                                          return true;
                                                      });
            }
            finally
            {
                const string MsgText = "The file was created successfully.";
                MessageBox.Show(MsgText, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Loads the release info from existing patch file.
        /// </summary>
        /// <returns></returns>
        private void LoadReleaseInfoFromFile()
        {
            SerializablePatch patch = TryDeserializePatchXml();
            if (patch == null)
                return;

            DateTime date;
            if (DateTime.TryParse(patch.Release.Date, out date))
                dtpRelease.Value = date;

            lblEVEMonVersion.Text = patch.Release.Version;
            rtbTopicUrl.Text = patch.Release.TopicAddress;
            rtbReleaseUrl.Text = patch.Release.PatchAddress.Remove(
                patch.Release.PatchAddress.LastIndexOf(Path.AltDirectorySeparatorChar) + 1,
                patch.Release.PatchAddress.Length - (patch.Release.PatchAddress.LastIndexOf(Path.AltDirectorySeparatorChar) + 1));
            lblMD5Sum.Text = patch.Release.MD5Sum;
            rtbReleaseMessage.Text = patch.Release.Message;
        }

        #endregion


        #region Control Handlers

        /// <summary>
        /// Occurs on clicking the "cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Occurs on clicking the "create" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        /// Occurs on clicking the "paste from clipboard" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPaste_Click(object sender, EventArgs e)
        {
            if (m_activeTextBox == null)
                return;

            m_activeTextBox.Text = Clipboard.GetText();

            if (m_activeTextBox.Parent.Parent is DatafileControl)
                EnsureHeaderMessage(m_activeTextBox);

            m_activeTextBox = null;
        }

        /// <summary>
        /// Occurs on clicking the "load for existing file" button in release section.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadReleaseInfo_Click(object sender, EventArgs e)
        {
            LoadReleaseInfoFromFile();
            UpdateCreateButtonEnabled();
        }

        /// <summary>
        /// Occurs on clicking the "load for existing file" button in datafiles section.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadDatafileInfo_Click(object sender, EventArgs e)
        {
            SerializablePatch patch = TryDeserializePatchXml();
            if (patch == null)
                return;

            string url = patch.Datafiles[0].Address;
            string revision = url.Remove(0, (url.LastIndexOf(Path.AltDirectorySeparatorChar) + 1));
            url = url.Remove(url.LastIndexOf(Path.AltDirectorySeparatorChar));
            string expansionName = url.Remove(0, (url.LastIndexOf(Path.AltDirectorySeparatorChar) + 1));
            url = url.Remove(url.LastIndexOf(Path.AltDirectorySeparatorChar) + 1);
            int expansionNameLastIndex = patch.Datafiles[0].Message.IndexOf(expansionName, StringComparison.Ordinal) +
                                         (expansionName.Length + 1);
            string message = patch.Datafiles[0].Message.Remove(0, expansionNameLastIndex);
            string version = message.Remove((message.IndexOf("(", StringComparison.OrdinalIgnoreCase) - 1),
                                            (message.Length - (message.IndexOf("(", StringComparison.OrdinalIgnoreCase) - 1)));

            foreach (SerializableDatafile datafile in patch.Datafiles)
            {
                rtbDatafileUrl.Text = url;
                tbExpansion.Text = expansionName;
                tbExpVersion.Text = version;
                tbExpRevision.Text = revision;

                foreach (DatafileControl dfControl in gbDatafiles.Controls.OfType<DatafileControl>().Where(
                    x => x != null && x.gbDatafile.Text == datafile.Name))
                {
                    DateTime date;
                    if (DateTime.TryParse(datafile.Date, out date))
                        dfControl.dtpDatafiles.Value = date;

                    dfControl.lblMD5Sum.Text = datafile.MD5Sum;
                    dfControl.rtbDatafileMessage.Text = datafile.Message;
                }
            }

            UpdateCreateButtonEnabled();
        }

        /// <summary>
        /// Occurs on clicking the "clear all" button in release section.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReleaseClear_Click(object sender, EventArgs e)
        {
            dtpRelease.ResetText();
            rtbTopicUrl.ResetText();
            rtbReleaseUrl.ResetText();
            rtbReleaseMessage.ResetText();

            UpdateCreateButtonEnabled();
        }

        /// <summary>
        /// Occurs on clicking the "clear all" button in datafiles section.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDatafilesClear_Click(object sender, EventArgs e)
        {
            rtbDatafileUrl.ResetText();
            tbExpansion.ResetText();
            tbExpVersion.ResetText();
            tbExpRevision.ResetText();

            foreach (DatafileControl dfControl in gbDatafiles.Controls.OfType<DatafileControl>().Where(x => x != null))
            {
                dfControl.dtpDatafiles.ResetText();
                dfControl.rtbDatafileMessage.ResetText();

                Datafile datafile = s_datafiles.First(x => x.Filename == dfControl.gbDatafile.Text);

                if (datafile != null)
                    UpdateDatafileInfo(dfControl, datafile);
            }

            UpdateCreateButtonEnabled();
        }

        /// <summary>
        /// Occurs on mouse double click inside a text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_DoubleClick(object sender, EventArgs e)
        {
            Control control = null;

            DatafileControl dfControl = sender as DatafileControl;
            if (dfControl != null)
                control = dfControl.rtbDatafileMessage;

            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox != null)
                control = richTextBox;

            TextBox textBox = sender as TextBox;
            if (textBox != null)
                control = textBox;

            if (control == null)
                return;

            m_activeTextBox = control;
            m_text = control.Text;
            control.Text = String.Empty;
        }

        /// <summary>
        /// Occurs when entering a text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
                textBox.ForeColor = SystemColors.WindowText;

            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox != null)
                richTextBox.ForeColor = SystemColors.WindowText;
        }

        /// <summary>
        /// Occurs when leaving a text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_Leave(object sender, EventArgs e)
        {
            Control control = null;

            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox != null)
                control = richTextBox;

            TextBox textBox = sender as TextBox;
            if (textBox != null)
                control = textBox;

            if (control == null)
                return;

            if (control == tbExpansion
                || control == tbExpVersion
                || control == tbExpRevision
                || control.Parent.Parent is DatafileControl)
                UpdateDatafilesControls();

            if (control.Text.Length == 0)
                control.Text = m_text;

            UpdateCreateButtonEnabled();
        }

        #endregion
    }
}