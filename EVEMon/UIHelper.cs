using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common;
using System.IO;
using EVEMon.Common.SettingsObjects;
using EVEMon.Common.Controls;
using System.Windows.Forms;
using System.IO.Compression;

namespace EVEMon
{
    /// <summary>
    /// Saves a couple of repetitive tasks.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Displays the plan exportation window and then exports it.
        /// </summary>
        /// <param name="plan"></param>
        public static void ExportPlan(Plan plan)
        {
            var character = (Character)plan.Character;

            // Assemble an initial filename and remove prohibited characters
            string planSaveName = character.Name + " - " + plan.Name;
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            int fileInd = planSaveName.IndexOfAny(invalidFileChars);
            while (fileInd != -1)
            {
                planSaveName = planSaveName.Replace(planSaveName[fileInd], '-');
                fileInd = planSaveName.IndexOfAny(invalidFileChars);
            }

            // Prompt the user to pick a file name
            SaveFileDialog sfdSave = new SaveFileDialog();
            sfdSave.FileName = planSaveName;
            sfdSave.Title = "Save to File";
            sfdSave.Filter = "EVEMon Plan Format (*.emp)|*.emp|XML  Format (*.xml)|*.xml|Text Format (*.txt)|*.txt";
            sfdSave.FilterIndex = (int)PlanFormat.Emp;

            DialogResult dr = sfdSave.ShowDialog();
            if (dr == DialogResult.Cancel) return;


            // Serialize
            try
            {
                string fileName = Path.GetTempFileName();

                // Emp is actually compressed text
                if (sfdSave.FilterIndex == (int)PlanFormat.Emp)
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        using (GZipStream gzs = new GZipStream(fs, CompressionMode.Compress))
                        {
                            using (var writer = new StreamWriter(gzs))
                            {
                                string output = PlanExporter.ExportAsXML(plan);
                                writer.Write(output);
                                writer.Flush();
                                gzs.Flush();
                                fs.Flush();
                            }
                        }
                    }
                }
                // Serialize to XML and text outputs
                else
                {
                    // Gets a string output
                    switch ((PlanFormat)sfdSave.FilterIndex)
                    {
                        case PlanFormat.Xml:
                            File.WriteAllText(fileName, PlanExporter.ExportAsXML(plan), Encoding.UTF8);
                            break;
                        case PlanFormat.Text:
                            // Prompts the user and returns if he canceled
                            var settings = PromptUserForPlanExportSettings(plan);
                            if (settings == null) return;

                            File.WriteAllText(fileName, PlanExporter.ExportAsText(plan, settings), Encoding.UTF8);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                // Moves to the final file
                FileHelper.OverwriteOrWarnTheUser(fileName, sfdSave.FileName, OverwriteOperation.Move);
            }
            catch (IOException err)
            {
                ExceptionHandler.LogException(err, true);
                MessageBox.Show("There was an error writing out the file:\n\n" + err.Message,
                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Prompt the user to select plan exportation settings.
        /// </summary>
        /// <returns></returns>
        public static PlanExportSettings PromptUserForPlanExportSettings(Plan plan)
        {
            PlanExportSettings settings = Settings.Exportation.PlanToText.Clone();
            using (CopySaveOptionsWindow f = new CopySaveOptionsWindow(settings, plan, false))
            {
                if (settings.Markup == MarkupType.Undefined)
                {
                    settings.Markup = MarkupType.None;
                }

                f.ShowDialog();
                if (f.DialogResult == DialogResult.Cancel) return null;

                // Save the new settings
                if (f.SetAsDefault)
                {
                    Settings.Exportation.PlanToText = settings;
                    Settings.Save();
                }

                return settings;
            }
        }

        /// <summary>
        /// Displays the character exportation window and then exports it.
        /// </summary>
        /// <param name="character"></param>
        public static void ExportCharacter(Character character)
        {
            // Open the dialog box
            using(var characterSaveDialog = new System.Windows.Forms.SaveFileDialog())
            {
                characterSaveDialog.Title = "Save Character Info";
                characterSaveDialog.Filter = "Text Format|*.txt|HTML Format|*.html|XML Format (CCP API)|*.xml|XML Format (EVEMon)|*.xml|PNG Image|*.png";
                characterSaveDialog.FileName = character.Name;
                characterSaveDialog.FilterIndex = (int)CharacterSaveFormat.CCPXML;

                var result = characterSaveDialog.ShowDialog();
                if (result == DialogResult.Cancel) return;

                // Serialize
                try
                {
                    // Save file to the chosen format and to a temp file
                    string tempFileName = Path.GetTempFileName();
                    CharacterSaveFormat format = (CharacterSaveFormat)characterSaveDialog.FilterIndex;
                    switch (format)
                    {
                        case CharacterSaveFormat.HTML:
                            File.WriteAllText(tempFileName, CharacterExporter.ExportAsHTML(character), Encoding.UTF8);
                            break;

                        case CharacterSaveFormat.CCPXML:
                            var content = CharacterExporter.ExportAsCCPXML(character);
                            if (content == null)
                            {
                                MessageBox.Show("This character has never been downloaded from CCP, cannot find it in the XML cache.", "Cannot export the character", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            File.WriteAllText(tempFileName, content, Encoding.UTF8);
                            break;

                        case CharacterSaveFormat.EVEMonXML:
                            File.WriteAllText(tempFileName, CharacterExporter.ExportAsEVEMonXML(character), Encoding.UTF8);
                            break;

                        case CharacterSaveFormat.Text:
                            File.WriteAllText(tempFileName, CharacterExporter.ExportAsText(character), Encoding.UTF8);
                            break;

                        case CharacterSaveFormat.PNG:
                            var monitor = Program.MainWindow.GetCurrentMonitor();
                            var bmp = monitor.GetCharacterScreenshot();
                            bmp.Save(tempFileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    // Writes to our file
                    FileHelper.OverwriteOrWarnTheUser(tempFileName, characterSaveDialog.FileName, OverwriteOperation.Move);
                }
                // Handle exception
                catch (IOException exc)
                {
                    ExceptionHandler.LogException(exc, true);
                    MessageBox.Show("A problem occured during exportation. The operation has not been completed.");
                }
            }
        }
    }
}
