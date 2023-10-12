using System.Net;

namespace Socket.Quobject.EngineIoClientDotNet.Modules
{
	public class ServerCertificate
	{
		public static bool Ignore { get; set; }

		static ServerCertificate()
		{
			ServerCertificate.Ignore = false;
		}

		public static void IgnoreServerCertificateValidation()
		{
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
			ServerCertificate.Ignore = true;
		}
	}
}
