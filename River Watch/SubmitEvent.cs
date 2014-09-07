using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

        public void send(Stream fileStream) {
            // Reset the stream position to the beginning
            fileStream.Position = 0;

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

    }
}
