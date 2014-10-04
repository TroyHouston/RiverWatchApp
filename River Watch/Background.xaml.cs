using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.BackgroundTransfer;
using System.IO.IsolatedStorage;

namespace River_Watch
{
    /** Code taken basically from the example from Microsoft */ 
    public partial class Background : PhoneApplicationPage
    {
        IEnumerable<BackgroundTransferRequest> transferRequests;

        // Booleans for tracking if any transfers are waiting for user
        // action. 
        bool WaitingForExternalPower;
        bool WaitingForExternalPowerDueToBatterySaverMode;
        bool WaitingForNonVoiceBlockingNetwork;
        bool WaitingForWiFi;

        public Background()
        {
            InitializeComponent();
        }

        private void UpdateRequestsList()
        {
            // The Requests property returns new references, so make sure that
            // you dispose of the old references to avoid memory leaks.
            if (transferRequests != null)
            {
                foreach (var request in transferRequests)
                {
                    request.Dispose();
                }
            }
            transferRequests = BackgroundTransferService.Requests;
        }


        private void UpdateUI()
        {
            // Update the list of transfer requests
            UpdateRequestsList();

            // Update the TransferListBox with the list of transfer requests.
            TransferListBox.ItemsSource = transferRequests;

            // If there are 1 or more transfers, hide the "no transfers"
            // TextBlock. IF there are zero transfers, show the TextBlock.
            if (TransferListBox.Items.Count > 0)
            {
                EmptyTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyTextBlock.Visibility = Visibility.Visible;
            }


        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Reset all of the user action Booleans on page load.
            WaitingForExternalPower = false;
            WaitingForExternalPowerDueToBatterySaverMode = false;
            WaitingForNonVoiceBlockingNetwork = false;
            WaitingForWiFi = false;

            // When the page loads, refresh the list of file transfers.
            InitialTransferStatusCheck();
            UpdateUI();
        }


        private void InitialTransferStatusCheck()
        {
            UpdateRequestsList();

            foreach (var transfer in transferRequests)
            {
                transfer.TransferStatusChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferStatusChanged);
                transfer.TransferProgressChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferProgressChanged);
                ProcessTransfer(transfer);
            }

            if (WaitingForExternalPower)
            {
                MessageBox.Show("You have one or more file transfers waiting for external power. Connect your device to external power to continue transferring.");
            }
            if (WaitingForExternalPowerDueToBatterySaverMode)
            {
                MessageBox.Show("You have one or more file transfers waiting for external power. Connect your device to external power or disable Battery Saver Mode to continue transferring.");
            }
            if (WaitingForNonVoiceBlockingNetwork)
            {
                MessageBox.Show("You have one or more file transfers waiting for a network that supports simultaneous voice and data.");
            }
            if (WaitingForWiFi)
            {
                MessageBox.Show("You have one or more file transfers waiting for a WiFi connection. Connect your device to a WiFi network to continue transferring.");
            }
        }

        private void ProcessTransfer(BackgroundTransferRequest transfer)
        {
            switch (transfer.TransferStatus)
            {
                case TransferStatus.Completed:

                    // If the status code of a completed transfer is 200 or 206, the
                    // transfer was successful
                    if (transfer.StatusCode == 200 || transfer.StatusCode == 206)
                    {
                        // Remove the transfer request in order to make room in the 
                        // queue for more transfers. Transfers are not automatically
                        // removed by the system.
                        RemoveTransferRequest(transfer.RequestId);

                        // In this example, the downloaded file is moved into the root
                        // Isolated Storage directory
                        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            string filename = "/shared/transfers/" + transfer.Tag;
                            if (isoStore.FileExists(filename))
                            {
                                isoStore.DeleteFile(filename);
                            }
                        }
                    }
                    else
                    {
                        // This is where you can handle whatever error is indicated by the
                        // StatusCode and then remove the transfer from the queue. 
                        RemoveTransferRequest(transfer.RequestId);

                        if (transfer.TransferError != null)
                        {
                            // Handle TransferError if one exists.
                        }
                    }
                    break;


                case TransferStatus.WaitingForExternalPower:
                    WaitingForExternalPower = true;
                    break;

                case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
                    WaitingForExternalPowerDueToBatterySaverMode = true;
                    break;

                case TransferStatus.WaitingForNonVoiceBlockingNetwork:
                    WaitingForNonVoiceBlockingNetwork = true;
                    break;

                case TransferStatus.WaitingForWiFi:
                    WaitingForWiFi = true;
                    break;
            }
        }

        void transfer_TransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            ProcessTransfer(e.Request);
            UpdateUI();
        }

        void transfer_TransferProgressChanged(object sender, BackgroundTransferEventArgs e)
        {
            UpdateUI();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // The ID for each transfer request is bound to the
            // Tag property of each Remove button.
            string transferID = ((Button)sender).Tag as string;

            // Delete the transfer request
            RemoveTransferRequest(transferID);

            // Refresh the list of file transfers
            UpdateUI();
        }

        private void RemoveTransferRequest(string transferID)
        {
            // Use Find to retrieve the transfer request with the specified ID.
            BackgroundTransferRequest transferToRemove = BackgroundTransferService.Find(transferID);

            if (transferToRemove == null)
            {
                return;
            }

            // Try to remove the transfer from the background transfer service.
            try
            {
                BackgroundTransferService.Remove(transferToRemove);
            }
            catch (Exception e)
            {
                // Handle the exception.
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        private void CancelAllButton_Click(object sender, EventArgs e)
        {
            // Loop through the list of transfer requests
            foreach (var transfer in BackgroundTransferService.Requests)
            {
                RemoveTransferRequest(transfer.RequestId);
            }

            // Refresh the list of file transfer requests.
            UpdateUI();
        }





    }
}