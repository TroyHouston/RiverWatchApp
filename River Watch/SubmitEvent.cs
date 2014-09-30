﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.Devices;
using System.IO.IsolatedStorage;
using Microsoft.Phone.BackgroundTransfer;
using System.Windows;
using System.Diagnostics;

namespace River_Watch
{
    class SubmitEvent
    {
        /*
         * Construct a submit event
         */
        public SubmitEvent()
        {

        }

        /*
         *  Data field - which must be in JSON format. 
         *
         *  I would heavily suggest a JSON library at some point.
         *  Especially if we do more with the phone app and get more results.
         */
        public String createJSONSubmit(List<String> tags, double lat, double lon)
        {
            StringBuilder s = new StringBuilder("{");

            s.Append("\"description\":"); s.Append("\"\"");
            s.Append(",");
            s.Append("\"geolocation\":");
            s.Append("{");
            s.Append("\"lat\":"); s.Append(lat);
            s.Append(",");
            s.Append("\"long\":"); s.Append(lon);
            s.Append("}");
            s.Append(",");
            s.Append("\"name\":"); s.Append("\"dsds\"");
            s.Append(",");
            s.Append("\"tags\":[");

            for (int i = 0; i < tags.Count; i++)
            {
                s.Append("\"");
                s.Append(tags[i]);
                s.Append("\"");
                if (i != tags.Count - 1)
                {
                    s.Append(",");
                }
            }
            s.Append("]");

            // s.Append(",");
            // s.Append("\"physical_location\":"); s.Append("\"addresss\"");

            s.Append("}");
            return s.ToString();
        }

        /* 
         * Creates all the headers for the request body.
         * 
         * The only thing that is missing from the body is the actual file data. 
         * (followed by the finally boundary marker)
         */
        private string generatePostData(string boundary, string fileName, List<String> tags, double lat, double lon)
        {
            StringBuilder headers = new StringBuilder();
            headers.AppendFormat("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
                boundary, "data", createJSONSubmit(tags, lat, lon));
            headers.AppendFormat("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\n\r\n",
                boundary, "image", fileName);

            return headers.ToString();
        }


        /* POST request consists of two parts 
         * 
         * 1.  "image" (followed by file data as binary) 
         * 2.  "data" (followed by submission JSON) 
         * 
         * Still needs work on multiple POST requests at the same time. 
         * Needs to form a queue of BackgroundTransferRequests. 
         * 
         */
        public bool send(Stream fileStream, List<String> tags, double lat, double lon)
        {
            // Reset the stream position to the beginning
            fileStream.Position = 0;
            long size = fileStream.Length;

            // Autogenerated boundary marker
            String boundary = String.Format("----------{0:N}", Guid.NewGuid());

            // Generate a unique filename for this data
            String filename = String.Format("{0:N}", Guid.NewGuid());

            // Save to file system first
            // This saves the entire post body, with content descriptors and boundary markers.
            // This is the only method for BackgroundTransferRequest to utilize without 8.1 APIs. 
            try
            {

                // Make sure that the required "/shared/transfers" directory exists
                // in isolated storage.

                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoStore.DirectoryExists("/shared/transfers"))
                    {
                        isoStore.CreateDirectory("/shared/transfers");
                    }
                }

                // Encoding with UTF-8 will probably still cause issues when it is outside Latin range
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    IsolatedStorageFileStream targetStream = isStore.OpenFile(@"/shared/transfers/" + filename, FileMode.Create);
                    {
                        // Write everything but the image data
                        byte[] header = Encoding.UTF8.GetBytes(generatePostData(boundary, filename, tags, lat, lon));
                        targetStream.Write(header, 0, header.Length);

                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Write the image data to the request 
                        while ((bytesRead = fileStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }

                        // Write the final boundary marker
                        byte[] footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--" + "\r\n");
                        targetStream.Write(footer, 0, footer.Length);
                    }
                    targetStream.Close();
                }
            }
            finally
            {
                // fileStream.Close();
            }


            // Check to see if the maximum number of requests per app has been exceeded.
            if (BackgroundTransferService.Requests.Count() >= 25)
            {
                // Note: Instead of showing a message to the user, you could store the
                // requested file URI in isolated storage and add it to the queue later.
                MessageBox.Show("Unable to add background transfer request. Queue is full.");
                return false;
            }

            // Create the new transfer request, passing in the URI of the file to 
            // be transferred.
            Uri downloadUri = new Uri("shared/transfers/" + filename, UriKind.RelativeOrAbsolute);
            
            /* !!!!! FOR USE IN PRODUCTION !!!!!! */
            //BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(new Uri(Constants.SERVER_URL + Constants.SUBMIT_PATH));
           
            /* !!!!! FOR USE IN TESTING !!!!! */
            BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(new Uri("http://192.168.1.91:8000/api/image"));
            
            transferRequest.UploadLocation = (downloadUri);

            // Set the filename so you can delete it later on
            transferRequest.Tag = filename;

            // Set the transfer method.
            transferRequest.Method = "POST";
            transferRequest.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);

            // We probably need to check that the request actually went through
            // But to do this, we need to save the response somewhere...
            // transferRequest.DownloadLocation = new Uri(...); 

            transferRequest.TransferStatusChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferStatusChanged);
            transferRequest.TransferProgressChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferProgressChanged);

            try
            {
                BackgroundTransferService.Add(transferRequest);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Unable to add background transfer request. " + ex.Message);
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to add background transfer request.");
                return false;
            }

            return true;
        }

        private void transfer_TransferProgressChanged(object sender, BackgroundTransferEventArgs e)
        {
            Debug.WriteLine(((e.Request.BytesSent * 100.0) / e.Request.TotalBytesToSend).ToString());
        }

        private void transfer_TransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            Debug.WriteLine("transfer status: " + e.Request.TransferStatus.ToString());
            BackgroundTransferRequest transfer = e.Request;
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

                        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            string filename = "/shared/transfers/" + transfer.Tag;

                            if (isoStore.FileExists(filename))
                            {
                                Debug.WriteLine("Deleting previous temp file.");
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
                    //WaitingForExternalPower = true;
                    break;

                case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
                    //WaitingForExternalPowerDueToBatterySaverMode = true;
                    break;

                case TransferStatus.WaitingForNonVoiceBlockingNetwork:
                    //WaitingForNonVoiceBlockingNetwork = true;
                    break;

                case TransferStatus.WaitingForWiFi:
                    //WaitingForWiFi = true;
                    break;
            }
        }

        private void RemoveTransferRequest(string transferID)
        {
            // Use Find to retrieve the transfer request with the specified ID.
            BackgroundTransferRequest transferToRemove = BackgroundTransferService.Find(transferID);

            // Try to remove the transfer from the background transfer service.
            try
            {
                BackgroundTransferService.Remove(transferToRemove);
            }
            catch (Exception e)
            {
                // Handle the exception.
                Debug.WriteLine(e);
            }
        }

    }
}
