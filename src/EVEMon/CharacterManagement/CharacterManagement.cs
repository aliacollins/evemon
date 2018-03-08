using EVEMon.Common.Controls;
using System;

namespace EVEMon.CharacterManagement {
    public partial class CharacterManagement : EVEMonForm
	{
		public CharacterManagement()
		{
			InitializeComponent();
		}

		private void bLoginSSO_Click(object sender, EventArgs e)
		{
			// We're trying to SSO with EVE. Call the gateway, subscribe to the success event and enjoy.
			/*IEVECharacterManager CharacterManager = EVECharacterManagerFactory.GetEVECharacterManager();
			GlobalEvents.SSOComplete += (object Sender, SSOCompleteEventArgs args) =>
			{
				CharacterManager.SetCharacterTokens(args);
			};

			IEVEAuthGateway AuthGateway = EVEAuthFactory.GetEVEAuthGateway();*/
		}

		private void CharacterManagement_Load(object sender, EventArgs e)
		{
			// TODO - Ashilta - Asynchronously load the existing character information
		}
	}
}
