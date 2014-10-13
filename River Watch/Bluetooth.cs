using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.UI.Popups;
using System.Windows;
using Microsoft.Phone.Tasks;
using Windows.Storage.Streams;




namespace River_Watch
{
    class Bluetooth
    {
        private DataReader dataReader;
        private StreamSocket socket;

        /* Attemps to get the message from the socket
         * 
         */
        private async Task<string> GetMessage()
        {
            //Create a datareader
            if (dataReader == null) dataReader = new DataReader(socket.InputStream);
            //
            await dataReader.LoadAsync(4);
            uint messageLen = (uint)dataReader.ReadInt32();
            await dataReader.LoadAsync(messageLen);
            return dataReader.ReadString(messageLen);
        }

        /*
         * Can be called when a button is pressed
         * Will instigate an attempt to connect to a local device
         * Will not work with Bluetooth LE devices
         */
        public async void buttonDiscoverDevices_Click()
        {
            try
            {
                //Getting every peered device
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                var peers = await PeerFinder.FindAllPeersAsync();

                //Found no peers
                if (peers.Count == 0)
                {
                    MessageBox.Show("Could not find any other devices", "Bluetooth", MessageBoxButton.OK);
                    return;
                }

                //Adding the peers information to a string
                StringBuilder list = new StringBuilder();
                foreach (PeerInformation p in peers)
                {
                    list.AppendLine(p.HostName + " : " + p.ServiceName);
                }



                // Just use the first Peer, can change to allow the user to choose
                PeerInformation partner = peers[0];
                // Attempt a connection to that device
                socket = new StreamSocket();
                
                await socket.ConnectAsync(partner.HostName, "1");

                //Attempt to get the method
                string message = await GetMessage();

            }
            catch (Exception e)
            {
                //Error is when there is no bluetooth connection
                if ((uint)e.HResult == 0x8007048f)
                {
                    MessageBoxResult result =
                   MessageBox.Show("This app accesses your phone's bluetooth connection. Is that ok?", "Bluetooth", MessageBoxButton.OKCancel);


                    if (result == MessageBoxResult.OK)
                    {
                        //Change page to the bluetooth settings
                        ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
                        connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
                        connectionSettingsTask.Show();
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}

