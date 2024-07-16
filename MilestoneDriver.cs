using System;
using System.Net;
using System.Threading;
using System.Windows.Threading;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Login;

namespace MilestoneDriverExample
{
    public class MilestoneDriver
    {
        private static readonly Guid IntegrationId = Guid.NewGuid(); // Replace with your integration ID
        private const string IntegrationName = "MilestoneDriverExample";
        private const string Version = "1.0";
        private const string ManufacturerName = "YourCompany";
        private ImageViewerWpfControl _imageViewerControl;
        private Thread _staThread;
        private Dispatcher _dispatcher;

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

        public void StartLiveStreaming(Item camera)
        {
            _staThread = new Thread(() =>
            {
                _dispatcher = Dispatcher.CurrentDispatcher;

                // Create ImageViewerControl to display video
                _imageViewerControl = new ImageViewerWpfControl();
                _imageViewerControl.CameraFQID = camera.FQID;
                _imageViewerControl.Initialize();
                _imageViewerControl.Connect();
                _imageViewerControl.StartLive();
                Console.WriteLine("Live streaming started.");

                Dispatcher.Run();
            });

            _staThread.SetApartmentState(ApartmentState.STA);
            _staThread.Start();
        }

        public void StopLiveStreaming()
        {
            if (_imageViewerControl != null && _dispatcher != null)
            {
                _dispatcher.Invoke(() =>
                {
                    _imageViewerControl.IsTabStop = true;
                    _imageViewerControl.Disconnect();
                    _imageViewerControl.Close();
                    _imageViewerControl = null;
                    Console.WriteLine("Live streaming stopped.");

                    _dispatcher.InvokeShutdown();  // Shutdown the dispatcher to exit the thread
                });

                _staThread.Join();  // Wait for the STA thread to finish
                _dispatcher = null;
                _staThread = null;
            }
        }

        public void Logout(string serverAddress)
        {
            Uri serverUri = new Uri(serverAddress);
            VideoOS.Platform.SDK.Environment.Logout(serverUri);
            VideoOS.Platform.SDK.Environment.RemoveServer(serverUri);
        }
    }
}
