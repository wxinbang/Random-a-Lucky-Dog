using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using static RLD.CPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.Services.LocalizeService;

namespace RLD.UWPCore
{
	public static class EmailProxy
	{
		public static async Task ComposeEmail()
		{
			var emailMessage = new EmailMessage();
			emailMessage.Body = "";

			var emailRecipient = new EmailRecipient("wxinbang@outlook.com");
			emailMessage.To.Add(emailRecipient);
			emailMessage.Subject = Localize(Feedback);

			await EmailManager.ShowComposeNewEmailAsync(emailMessage);
		}
		public static async Task ComposeEmail(string exception)
		{
			var emailMessage = new EmailMessage();
			emailMessage.Body = DateTime.Now.ToString() + Localize(ExceptionAt) + exception;

			var emailRecipient = new EmailRecipient("wxinbang@outlook.com");
			emailMessage.To.Add(emailRecipient);
			emailMessage.Subject = Localize(SoftwareCrashes);

			await EmailManager.ShowComposeNewEmailAsync(emailMessage);
		}

	}
}
