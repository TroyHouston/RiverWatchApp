using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace River_Watch
{
    /** 
     * Custom string converter to change [GUID]-[image name] to [image name] in background upload page.
     */
    public class StringFormatConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String original  = value as string;
            if (original != null) {
                int index = original.IndexOf("-");
                if (index != -1) {
                    return original.Substring(index + 1);
                }
            }
         
            return "Untitled";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
