using System;
using System.ComponentModel;
using System.Windows.Forms;
using EVEMon.Common;
using System.Collections.Generic;

namespace EVEMon
{
    /// <summary>
    /// Maintenance dialog to edit an APIConfiguration instance.
    /// </summary>
    public partial class APISettingsForm : Form
    {
        private readonly APIConfiguration _apiConfiguration;

        public APISettingsForm() : this(null)
        {
        }

        public APISettingsForm(APIConfiguration apiConfiguration)
        {
            InitializeComponent();
            _apiConfiguration = apiConfiguration;
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
            if (_apiConfiguration != null)
            {
                txtConfigurationName.Text = _apiConfiguration.Name;
                txtConfigurationName.Enabled = _apiConfiguration.Name == string.Empty;
                txtAPIHost.Text = _apiConfiguration.Server;
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
            foreach (APIMethod method in _apiConfiguration.Methods)
            {
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
            List<APIMethod> defaultMethods = APIConfiguration.DefaultMethods;
            foreach (DataGridViewRow row in dgMethods.Rows)
            {
                APIMethod rowMethod = (APIMethod)row.Tag;
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
                _apiConfiguration.Name = txtConfigurationName.Text;
                _apiConfiguration.Server = txtAPIHost.Text;
                foreach (DataGridViewRow row in dgMethods.Rows)
                {
                    APIMethod method = (APIMethod)row.Tag;
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
            if (configName == string.Empty)
            {
                ShowValidationError(txtConfigurationName, "Configuration Name cannot be blank.");
                e.Cancel = true;
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
