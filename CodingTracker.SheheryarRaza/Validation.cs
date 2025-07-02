using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.SheheryarRaza
{
    public class Validation
    {

        public const string DateTimeFormat = "yyyy-MM-dd HH:mm";
        public static bool IsValidDateTime(string input, out DateTime parsedDateTime)
        {

            return DateTime.TryParseExact(input, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);
        }

        public static bool IsValidInteger(string input, out int parsedInt)
        {
            return int.TryParse(input, out parsedInt);
        }
    }
}
