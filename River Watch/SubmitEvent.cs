using System;
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
        public SubmitEvent()
        {

        }

        // I would heavily suggest a JSON library at some point
        public String createJSONSubmit()
        {
            StringBuilder s = new StringBuilder("{");
            List<String> tags = new List<String>() { "hello", "bye"};
            s.Append("\"description\":"); s.Append("\"\"");
            s.Append(",");
            s.Append("\"geolocation\":");
            s.Append("{");
            s.Append("\"lat\":"); s.Append("3");
            s.Append(",");
            s.Append("\"long\":"); s.Append("4");
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

        public void send(Stream fileStream) {
            // Reset the stream position to the beginning
            fileStream.Position = 0;
            long size = fileStream.Length;
            String boundary = "sfdsfdsfdsfdsfds";
            // Save to file system first 
            try
            {
                
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoStore.DirectoryExists("/shared/transfers"))
                    {
                        isoStore.CreateDirectory("/shared/transfers");
                    }
                }

                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream targetStream = isStore.OpenFile(@"/shared/transfers/temp.jpeg", FileMode.Create);
                    {
                        // Write everything but the image data
                        byte[] header = Encoding.UTF8.GetBytes(generatePostData(boundary, "temp.jpeg"));
                        targetStream.Write(header, 0, header.Length);

                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the image to the local folder. 
                        while ((bytesRead = fileStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }

                        // Write the final boundary marker
                        byte[] footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--" + "\r\n"); // <<<<<<----
                        targetStream.Write(footer, 0, footer.Length);
                    }
                    targetStream.Close();
                }
            }
            finally {
               // fileStream.Close();
            }

            /*
            StorageFile file = await StorageFile.GetFileFromPathAsync(@"shared/transfers/temp.jpeg");

            //!------!!!
            var boundary = "DSFDSFWERJWREWJ:LDJF:SDF:SSDF";
            //!------!!!

            byte[] form = Encoding.UTF8.GetBytes(generatePostData(boundary, file.Name));
            byte[] fdata = new byte[size];
            byte[] bound = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            // read file data
            IRandomAccessStream ra = await file.OpenAsync(FileAccessMode.Read);
            IInputStream istream = ra.GetInputStreamAt(0);
            DataReader reader = new DataReader(istream);
            await reader.LoadAsync((uint)fdata.Length);
            reader.ReadBytes(fdata);
            // connect three parts
            byte[] data = new byte[form.Length + fdata.Length + bound.Length];
            Array.Copy(form, 0, data, 0, form.Length);
            Array.Copy(fdata, 0, data, form.Length, fdata.Length);
            Array.Copy(bound, 0, data, form.Length + fdata.Length, bound.Length);

            
            using (IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isolatedStorageFileStream = isolatedStorageFile.OpenFile(fileName, FileMode.Open))
                {
                    using (StreamReader streamReader = new StreamReader(isolatedStorageFileStream))
                    {
                        text = streamReader.ReadToEnd();
                    }
                }
            }

            MemoryStream mem = new MemoryStream(data);
            IInputStream stream = mem.AsInputStream(); */

            /*BackgroundUploader up = new BackgroundUploader();
            up.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + boundary);
            up.Method = "POST";

            uo = await up.CreateUploadFromStreamAsync(new Uri(url), stream);
            await uo.StartAsync().AsTask(new Progress<UploadOperation>(OnUploadProgressChanged));

            return data;*/

            // Make sure that the required "/shared/transfers" directory exists
            // in isolated storage.



            // Check to see if the maximum number of requests per app has been exceeded.
            if (BackgroundTransferService.Requests.Count() >= 25)
            {
                // Note: Instead of showing a message to the user, you could store the
                // requested file URI in isolated storage and add it to the queue later.
            }

            // Create the new transfer request, passing in the URI of the file to 
            // be transferred.
            Uri downloadUri = new Uri("shared/transfers/temp.jpeg", UriKind.RelativeOrAbsolute);
            BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(new Uri("http://192.168.1.91:8000/api/image"));
            transferRequest.UploadLocation = (downloadUri);
            // Set the transfer method. GET and POST are supported.
            transferRequest.Method = "POST";
            transferRequest.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);

            transferRequest.TransferStatusChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferStatusChanged);
            transferRequest.TransferProgressChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferProgressChanged);

            try
            {
                BackgroundTransferService.Add(transferRequest);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Unable to add background transfer request. " + ex.Message);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to add background transfer request.");
            }


            /* Post request consists of two parts 
             * 
             * 1.  "image" (followed by file data as binary) 
             * 2.  "data" (followed by submission JSON) 
             * 
             */
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
                        //RemoveTransferRequest(transfer.RequestId); // TODO

                        // In this example, the downloaded file is moved into the root
                        // Isolated Storage directory
                        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            string filename = "temp.jpeg";//transfer.Tag;
                            if (isoStore.FileExists(filename))
                            {
                                Debug.WriteLine("Deleting previous temp file.");
                                isoStore.DeleteFile(filename);
                            }
                            //isoStore.MoveFile(transfer.DownloadLocation.OriginalString, filename);
                        }
                    }
                    else
                    {
                        // This is where you can handle whatever error is indicated by the
                        // StatusCode and then remove the transfer from the queue. 
                        //RemoveTransferRequest(transfer.RequestId); // TODO

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
        

        private string generatePostData(string boundary, string fileName)
        {
            StringBuilder headers = new StringBuilder();
            headers.AppendFormat("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
                boundary, "data", createJSONSubmit());
            headers.AppendFormat("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\n" +
                "\r\n",
                boundary, "image", fileName);

            return headers.ToString();
        }

        private byte[] generateRequestBinary() {

            return new byte[0];
        }

    }
}
