using System;
using System.Net;
using VideoOS.Platform;
using VideoOS.Platform.Login;

namespace MilestoneDriverExample
{
    public class MilestoneDriver
    {
        private static readonly Guid IntegrationId = Guid.NewGuid(); // Replace with your integration ID
        private const string IntegrationName = "MilestoneDriverExample";
        private const string Version = "1.0";
        private const string ManufacturerName = "YourCompany";

        public void Initialize()
        {
            // Initialize the environment
            VideoOS.Platform.SDK.Environment.Initialize();
        }

        public bool Connect(string serverAddress, string username, string password)
        {
            // Add server to environment
            Uri serverUri = new Uri(serverAddress);
            NetworkCredential nc = new System.Net.NetworkCredential(username, password);
            LoginSettings loginSettings = new LoginSettings(serverUri, nc, Guid.NewGuid() , "test" , false);
            VideoOS.Platform.SDK.Environment.AddServer(loginSettings, true);

            // Log in to the server
            try
            {
                VideoOS.Platform.SDK.Environment.Login(serverUri, IntegrationId, IntegrationName, Version, ManufacturerName, false);
                return true;
            }
            catch (VideoOS.Platform.SDK.Platform.InvalidCredentialsMIPException)
            {
                Console.WriteLine("Invalid credentials.");
            }
            catch (VideoOS.Platform.SDK.Platform.ServerNotFoundMIPException)
            {
                Console.WriteLine("Server not found.");
            }
            catch (VideoOS.Platform.SDK.Platform.LoginFailedInternalMIPException)
            {
                Console.WriteLine("Internal login error.");
            }
            return false;
        }

        public Item GetFirstCamera()
        {
            // Get the list of cameras
            var allCameras = Configuration.Instance.GetItemsByKind(Kind.Camera);
            return allCameras.Count > 0 ? allCameras[0] : null;
        }

        public void Logout(string serverAddress)
        {
            Uri serverUri = new Uri(serverAddress);
            VideoOS.Platform.SDK.Environment.Logout(serverUri);
            VideoOS.Platform.SDK.Environment.RemoveServer(serverUri);
        }
    }
}
