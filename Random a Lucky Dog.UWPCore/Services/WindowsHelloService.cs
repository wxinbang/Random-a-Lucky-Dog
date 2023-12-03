﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Xaml.Media;

namespace RLD.UWPCore.Services
{
	public static class WindowsHelloService
	{
		public static async Task<bool> IsWindowsHelloAvailableAsync()
		{
			bool keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
			if (keyCredentialAvailable == false)
			{
				// Key credential is not enabled yet as user 
				// needs to connect to a Microsoft Account and select a PIN in the connecting flow.
				Debug.WriteLine("Microsoft Passport is not setup!\nPlease go to Windows Settings and set up a PIN to use it.");
				return false;
			}
			return true;
		}
		public static async Task<KeyCredentialRetrievalResult> CreatePassportKeyAsync(string accountId = "LocalHost")
		{
			return await KeyCredentialManager.RequestCreateAsync(accountId, KeyCredentialCreationOption.ReplaceExisting);

			//switch (keyCreationResult.Status)
			//{
			//	case KeyCredentialStatus.Success:
			//		Debug.WriteLine("Successfully made key");

			//		// In the real world authentication would take place on a server.
			//		// So every time a user migrates or creates a new Microsoft Passport account Passport details should be pushed to the server.
			//		// The details that would be pushed to the server include:
			//		// The public key, keyAttesation if available, 
			//		// certificate chain for attestation endorsement key if available,  
			//		// status code of key attestation result: keyAttestationIncluded or 
			//		// keyAttestationCanBeRetrievedLater and keyAttestationRetryType
			//		// As this sample has no concept of a server it will be skipped for now
			//		// for information on how to do this refer to the second Passport sample

			//		//For this sample just return true
			//		return true;
			//	case KeyCredentialStatus.UserCanceled:
			//		Debug.WriteLine("User cancelled sign-in process.");
			//		break;
			//	case KeyCredentialStatus.NotFound:
			//		// User needs to setup Microsoft Passport
			//		Debug.WriteLine("Microsoft Passport is not setup!\nPlease go to Windows Settings and set up a PIN to use it.");
			//		break;
			//	default:
			//		break;
			//}

			//return false;
		}
	}
}
