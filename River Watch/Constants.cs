using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River_Watch
{
    static class Constants
    {
        public static string SERVER_URL { get { return "http://www.wainz.org.nz"; } }
        public static string SECURE_SERVER_URL { get { return "https://www.wainz.org.nz"; } }

        public static string SUBMIT_PATH { get { return SERVER_URL + "/api/image" ; } }
        public static string SECURE_SUBMIT_PATH { get { return SERVER_URL + "/api/image"; } }
    }
}
