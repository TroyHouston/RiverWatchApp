using System;
using System.Collections.Generic;
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
            s.Append("\"tag1\"");
                s.Append(",");
                s.Append("\"tag2\"");
            s.Append("]]");

            s.Append(",");
            s.Append("\"physical_location\":"); s.Append("\"addresss\"");

            s.Append("}");
            return s.ToString();
        }

    }
}
