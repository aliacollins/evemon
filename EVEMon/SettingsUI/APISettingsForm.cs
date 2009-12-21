using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using EVEMon.Common;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Controls;

namespace EVEMon.SettingsUI
{
    /// <summary>
    /// Maintenance dialog to edit an APIConfiguration instance.
    /// </summary>
    public partial class APISettingsForm : EVEMonForm
    {
        private readonly SerializableAPIProviders m_providers;
        private readonly SerializableAPIProvider m_provider;

        public APISettingsForm(SerializableAPIProviders providers, SerializableAPIProvider newProvider)
        {
            InitializeComponent();
            m_providers = providers;
            m_provider = newProvider;
        }

        /// <summary>
        /// Overrides System.Windows.Forms.Form.OnLoad()
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Initialise();
        }

        /// <summary>
        /// Form initialisation. Populates and enables the main form elements using the provided APIConfiguration instance.
        /// The Name property of the APIConfiguration instance may only be changed if it has not been previously assigned.
        /// </summary>
        private void Initialise()
        {
            if (m_provider != null)
            {
                txtConfigurationName.Text = m_provider.Name;
                txtAPIHost.Text = m_provider.Url;
                InitialiseDataGrid();
            }
        }

        /// <summary>
        /// Populates the DataGridView control with APIMethod details from the specified APIConfiguration instance.
        /// The APIMethod is stored in the DataGridViewRow.Tag property for reference.
        /// </summary>
        private void InitialiseDataGrid()
        {
            dgMethods.Rows.Clear();
            foreach (var method in m_provider.Methods)
            {
                // Skip "none"
                if (method.Method == APIMethods.None) continue;

                // Add row
                int rowIndex = dgMethods.Rows.Add(method.Method, method.Path);
                dgMethods.Rows[rowIndex].Tag = method;
            }
        }

        /// <summary>
        /// Resets the Path column of the Methods DataGridView to the Path property of the APIConfiguration.DefaultMethods collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUseDefaults_Click(object sender, EventArgs e)
        {
            var defaultMethods = APIProvider.DefaultProvider.Methods;
            foreach (DataGridViewRow row in dgMethods.Rows)
            {
                var rowMethod = (SerializableAPIMethod)row.Tag;
                foreach (APIMethod defaultMethod in defaultMethods)
                {
                    if (defaultMethod.Method == rowMethod.Method)
                    {
                        row.Cells[1].Value = defaultMethod.Path;
                    }
                }
            }
        }

        /// <summary>
        /// Validates user input and assigns the edited values back to the APIConfiguration instance before closing the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                m_provider.Name = txtConfigurationName.Text;
                m_provider.Url = txtAPIHost.Text;

                foreach (DataGridViewRow row in dgMethods.Rows)
                {
                    var method = (SerializableAPIMethod)row.Tag;
                    method.Path = (string)row.Cells[1].Value;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Validates the Configuration Name input value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtConfigurationName_Validating(object sender, CancelEventArgs e)
        {
            string configName = txtConfigurationName.Text.Trim();

            // Checks it is not a empty name
            if (configName == string.Empty)
            {
                ShowValidationError(txtConfigurationName, "Configuration Name cannot be blank.");
                e.Cancel = true;
                return;
            }

            // Check the name does not already exist
            bool exist = (configName == APIProvider.DefaultProvider.Name);
            foreach (var provider in m_providers.CustomProviders)
            {
                exist |= (configName == provider.Name && provider != m_provider);
            }

            if (exist)
            {
                ShowValidationError(txtConfigurationName, "There is already a provider named " + configName + ".");
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Clears a Configuration Name validation error once the input value has been validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtConfigurationName_Validated(object sender, EventArgs e)
        {
            ClearValidationError(txtConfigurationName);
        }

        /// <summary>
        /// Validates the API Host Name input value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAPIHost_Validating(object sender, CancelEventArgs e)
        {
            string apiHost = txtAPIHost.Text.Trim();
            if (apiHost == string.Empty)
            {
                ShowValidationError(txtAPIHost, "API Host Name cannot be blank.");
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Clears an API Host validation error once the input value has been validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAPIHost_Validated(object sender, EventArgs e)
        {
            ClearValidationError(txtAPIHost);
        }

        /// <summary>
        /// Validates an API Method Path input value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMethods_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if ((string)e.FormattedValue == string.Empty)
            {
                ShowValidationError(dgMethods, string.Format("Path for method {0} cannot be blank", dgMethods.Rows[e.RowIndex].Cells[0].Value));
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Clears an API Method Path validation error once the input value has been validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMethods_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            ClearValidationError(dgMethods);
        }

        /// <summary>
        /// Displays a validation error notification for the specified control using the specified message.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="errorMessage"></param>
        private void ShowValidationError(Control control, string errorMessage)
        {
            errorProvider.SetError(control, errorMessage);
        }

        /// <summary>
        /// Clears a validation error notification on the specified control.
        /// </summary>
        /// <param name="control"></param>
        private void ClearValidationError(Control control)
        {
            errorProvider.SetError(control,string.Empty);
        }
    }
}
