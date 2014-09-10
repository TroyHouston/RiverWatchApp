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
            List<String> tags = new List<String>() { "cow", "sheep"};
            s.Append("\"geolocation\":");
            s.Append("{");
            s.Append("\"lat\":"); s.Append("3");
            s.Append(",");
            s.Append("\"long\":"); s.Append("4");
            s.Append("}");

            s.Append(",");

            s.Append("\"description\":"); s.Append("\"some description\"");

            s.Append(",");

            // From the Android code, it seems to be doubly-arrayed
            s.Append("\"tags\":[[");

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
            s.Append("]]");

            s.Append(",");
            s.Append("\"physical_location\":"); s.Append("\"addresss\"");

            s.Append("}");
            return s.ToString();
        }

        public async void send(Stream fileStream) {
            // Reset the stream position to the beginning
            fileStream.Position = 0;
            long size = fileStream.Length;

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
                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the image to the local folder. 
                        while ((bytesRead = fileStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
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
            BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(new Uri("http://www.google.co.nz"));
            transferRequest.UploadLocation = (downloadUri);
            // Set the transfer method. GET and POST are supported.
            transferRequest.Method = "POST";
            transferRequest.Headers.Add("Content-Type", "multipart/form-data; boundary=80bb0a10641341d493fe42ea98084457");

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


            /* Post request consists of three parts 
             * 
             * 1.  "Incident as JSON: " (followed by submission JSON?)
             * 2.  "image" (followed by file data assumedly as binary) 
             * 3.  "data" (followed by submission JSON?) 
             * 
             * Still need to figure out what the difference is. 
             */

            /* Some example code
             * I think we need to use http client or httpwebrequest
             * 
            using (HttpClient client = new HttpClient())
            {

                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    if (request.Headers != null)
                    {
                        foreach (KeyValuePair<string, string> header in request.Headers)
                        {
                            content.Headers.Add(header.Key, header.Value);
                        }
                    }
                    using (StreamContent streamContent = new StreamContent(photoStream))
                    {
                        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);
                        content.Add(streamContent, multipartName, fileName);

                        // Post the message and ensure a result.
                        using (HttpResponseMessage response = await client.PostAsync(request.TargerUrl, content))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                string responseData = await response.Content.ReadAsStringAsync();
                                return new RestResponse(ResponseStatus.Success, null) { ResponseData = responseData };
                            }
                            else
                            {
                                return new RestResponse(ResponseStatus.Fail, null) { Exception = new Exception(response.ReasonPhrase) };
                            }
                        }
                    }
                }
            }*/
        }

        private string generatePostData(string boundary, string fileName)
        {
            StringBuilder headers = new StringBuilder();
            headers.AppendFormat("--{0}\r\nContent-Disposition name=\"{1}\"\r\n\r\n{2}\r\n",
                boundary, "data", createJSONSubmit());
            headers.AppendFormat("--{0}\r\nContent-Disposition name=\"{1}\"; filename=\"{2}\"\r\n" +
                "Content-Type: {3}\r\n\r\n",
                boundary, "file", fileName, "application/octet-stream");

            return headers.ToString();
        }

        private byte[] generateRequestBinary() {

            return new byte[0];
        }

    }
}
