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

namespace EVEMon.CharacterManagement
{
	public partial class CharacterManagement : EVEMonForm
	{
		public CharacterManagement()
		{
			InitializeComponent();
		}

		private void bLoginSSO_Click(object sender, EventArgs e)
		{
			// We're trying to SSO with EVE. Call the gateway, subscribe to the success event and enjoy.
			IEVECharacterManager CharacterManager = EVECharacterManagerFactory.GetEVECharacterManager();
			GlobalEvents.SSOComplete += (object Sender, SSOCompleteEventArgs args) =>
			{
				CharacterManager.SetCharacterTokens(args);
			};

			IEVEAuthGateway AuthGateway = EVEAuthFactory.GetEVEAuthGateway();
		}

		private void CharacterManagement_Load(object sender, EventArgs e)
		{
			// TODO - Ashilta - Asynchronously load the existing character information
		}
	}
}