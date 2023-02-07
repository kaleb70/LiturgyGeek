using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common
{
    public static class Int32Extensions
    {
        // TODO: i18n
        public static string FormatOrdinal(this Int32 value)
        {
            return value.ToString()
                    + (value % 100) switch
                    {
                        11 or 12 or 13 => "th",
                        _ => (value % 10) switch
                        {
                            1 => "st",
                            2 => "nd",
                            3 => "rd",
                            _ => "th",
                        },
                    };
        }
    }
}
