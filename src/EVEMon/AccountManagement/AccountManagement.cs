using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Entities.Events;
using EVEMon.Gateways.Factories;
using EVEMon.Gateways.Specification;
using EVEMon.Services.Factories;
using EVEMon.Services.Specification;

namespace EVEMon.AccountManagement
{
	public partial class AccountManagement : EVEMonForm
	{
		public AccountManagement()
		{
			InitializeComponent();
		}

		private void bLoginSSO_Click(object sender, EventArgs e)
		{
			// We're trying to SSO with EVE. Call the gateway, subscribe to the success event and enjoy.
			IEVEAccountManager AccountManager = EVEAccountManagerFactory.GetEVEAccountManager();
			GlobalEvents.SSOComplete += (object Sender, SSOCompleteEventArgs args) =>
			{
				AccountManager.SetAccountTokens(string.Empty, args);
			};

			IEVEAuthGateway AuthGateway = EVEAuthFactory.GetEVEAuthGateway();
		}
	}
}