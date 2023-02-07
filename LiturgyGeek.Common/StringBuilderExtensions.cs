using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common
{
    public static class StringBuilderExtensions
    {
        public static void AppendOrdinal(this StringBuilder builder, int ordinal)
        {
            builder.Append(ordinal.FormatOrdinal());
        }
    }
}
