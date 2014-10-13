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

        private async Task<string> GetMessage()
        {
            if (dataReader == null) dataReader = new DataReader(socket.InputStream);
            await dataReader.LoadAsync(4);
            uint messageLen = (uint)dataReader.ReadInt32();
            await dataReader.LoadAsync(messageLen);
            return dataReader.ReadString(messageLen);
        }

        public async void buttonDiscoverDevices_Click()
        {
            try
            {
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                var peers = await PeerFinder.FindAllPeersAsync();

                if (peers.Count == 0)
                {
                    MessageBox.Show("Could not find any other devices", "Bluetooth", MessageBoxButton.OK);
                    return;
                }

                StringBuilder list = new StringBuilder();
                foreach (PeerInformation p in peers)
                {
                    list.AppendLine(p.HostName + " : " + p.ServiceName);
                }



                // Just use the first Peer
                PeerInformation partner = peers[0];
                // Attempt a connection
                socket = new StreamSocket();
                await socket.ConnectAsync(partner.HostName, "1");

                string message = await GetMessage();

            }
            catch (Exception e)
            {
                if ((uint)e.HResult == 0x8007048f)
                {
                    MessageBoxResult result =
                   MessageBox.Show("This app accesses your phone's bluetooth connection. Is that ok?", "Bluetooth", MessageBoxButton.OKCancel);


                    if (result == MessageBoxResult.OK)
                    {
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

