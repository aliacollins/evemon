using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Initialise();
        }

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

        private void InitialiseDataGrid()
        {
            dgMethods.Rows.Clear();
            foreach (APIMethod method in _apiConfiguration.Methods)
            {
                int rowIndex = dgMethods.Rows.Add(method.Method, method.Path);
                dgMethods.Rows[rowIndex].Tag = method;
            }
        }

        private void btnUseDefaults_Click(object sender, EventArgs e)
        {
            _apiConfiguration.Methods =APIConfiguration.DefaultMethods;
            InitialiseDataGrid();
        }

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

        private void txtConfigurationName_Validating(object sender, CancelEventArgs e)
        {
            string configName = txtConfigurationName.Text.Trim();
            if (configName == string.Empty)
            {
                ShowValidationError(txtConfigurationName, "Configuration Name cannot be blank.");
                e.Cancel = true;
            }
        }

        private void txtConfigurationName_Validated(object sender, EventArgs e)
        {
            ClearValidationError(txtConfigurationName);
        }

        private void txtAPIHost_Validating(object sender, CancelEventArgs e)
        {
            string apiHost = txtAPIHost.Text.Trim();
            if (apiHost == string.Empty)
            {
                ShowValidationError(txtAPIHost, "API Host Name cannot be blank.");
                e.Cancel = true;
            }
        }

        private void txtAPIHost_Validated(object sender, EventArgs e)
        {
            ClearValidationError(txtAPIHost);
        }

        private void dgMethods_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if ((string)e.FormattedValue == string.Empty)
            {
                ShowValidationError(dgMethods, string.Format("Path for method {0} cannot be blank", dgMethods.Rows[e.RowIndex].Cells[0].Value));
                e.Cancel = true;
            }
        }

        private void dgMethods_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            ClearValidationError(dgMethods);
        }

        private void ShowValidationError(Control control, string errorMessage)
        {
            errorProvider.SetError(control, errorMessage);
        }

        private void ClearValidationError(Control control)
        {
            errorProvider.SetError(control,string.Empty);
        }
    }
}
