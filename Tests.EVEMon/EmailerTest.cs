using NUnit.Framework;
using EVEMon.Common;
using EVEMon;

namespace Tests.EVEMon
{
	[TestFixture]
	public class EmailerTest
	{
		private Settings settings;

		[SetUp]
		public void SetUp()
		{
			//must update these settings so that the actual mail server will work.
			//maybe it can be mocked, and then just check that all expected values have reached the SmtpClient?
			settings = new Settings
			           	{
			           		EmailServer = "your.smtp.server.here", EmailFromAddress = "a@hotmail.com",
			           		EmailToAddress = "your@address.here", EmailAuthRequired = true, EmailAuthUsername = "username",
			           		EmailAuthPassword = "password", EmailServerRequiresSsl = true
			           	};
		}

		[Test]
		public void TestSendTestMail_NoPortChanged()
		{
			Assert.That(Emailer.SendTestMail(settings), "failed sending mail");
		}

		[Test]
		public void TestSendTestMail_WithPortChanged()
		{
			// changing from 0, which uses the deafult port, to 25 - which is the default port.
			// the difference is that now the port will be set explicitly
			settings.PortNumber = 25;
			Assert.That(Emailer.SendTestMail(settings), "failed sending mail");
		}

		[Test]
		public void TestSendTestMail_BadPortChanged()
		{
			// changing from 0, which uses the deafult port, to 26 - which should not succeed
			settings.PortNumber = 26;
			Assert.That(!Emailer.SendTestMail(settings), "mail was sent through the wrong port");
		}
	}
}
