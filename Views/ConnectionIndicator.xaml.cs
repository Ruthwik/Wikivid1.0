using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleConnectionControl.Control
{
    public sealed partial class ConnectionIndicator : UserControl
    {
        //Handles the OnNetworkStatusChange event 
        NetworkStatusChangedEventHandler networkStatusCallback = null;

        //Indicates if the connection profile is registered for network status change events. Set the default value to FALSE. 
        public static bool registeredNetworkStatusNotif = false;

        // create image source for online
        public static string uri_on = "ms-appx:///Assets/Internet.png";
        public BitmapImage onSource = new BitmapImage(new Uri(uri_on, UriKind.Absolute));

        // create image source for offline
        public static string uri_off = "ms-appx:///Assets/WorkOffline.png";
        public BitmapImage offSource = new BitmapImage(new Uri(uri_off, UriKind.Absolute));
        MessageDialog dlg;
        string ErrorMessage = string.Empty;
               
        public ConnectionIndicator()
        {
            this.InitializeComponent();

            networkStatusCallback = new NetworkStatusChangedEventHandler(OnNetworkStatusChange);

            // register for network status change notifications
            if (!registeredNetworkStatusNotif)
            {
                NetworkInformation.NetworkStatusChanged += networkStatusCallback;
                registeredNetworkStatusNotif = true;
            }

            networkStatusCallback(null);
        }

        private void OnNetworkStatusChange(object sender)
        {
            string ErrorMessage = string.Empty;
          try
            {
                
               
                 // get the ConnectionProfile that is currently used to connect to the Internet                
                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();


                if (InternetConnectionProfile != null && InternetConnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                {

                    //ErrorMessage = "Suggestions could not be retrieved -- please verify that the URL points to a valid service";
                    this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Status.Source = onSource);
                 
                }
                else
                {
                    this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Status.Source = offSource);
                  //ErrorMessage = "Suggestions could not be retrieved -- please verify that the URL points to a valid service";
                }


               

            }

            catch (Exception ex)
            {
                //ErrorMessage = "Suggestions could not be retrieved -- please verify that the URL points to a valid service";
                Status.Source = offSource;
            }
       /*   if (ErrorMessage != string.Empty)
          {
              dlg = new MessageDialog(ErrorMessage);
               dlg.ShowAsync();
          }
            */
         }
        
    }
}
