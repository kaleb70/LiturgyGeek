using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Globalization
{
    public class GregorianPaschalCalendar : PaschalCalendar
    {
        private static readonly ConcurrentDictionary<int, DateTime> paschaDictionary = new ConcurrentDictionary<int, DateTime>();

        /// <summary>
        /// The Gregorian Calendar for liturgical purposes begins on October 15, 1582.
        /// </summary>
        public override DateTime MinSupportedDateTime => new DateTime(1582, 10, 15);

        public override DateTime FindPascha(int year)
        {
            if (new DateTime(year, 3, 21) < MinSupportedDateTime || new DateTime(year + 1, 3, 21) > MaxSupportedDateTime)
                throw new ArgumentException($"Year {year} not supported for Pascha calculation.", nameof(year));
            return paschaDictionary.GetOrAdd(year, y =>
            {
                int g = y % 19;
                int c = y / 100;
                int h = (c - (c / 4) - ((8 * c + 13) / 25) + (19 * g) + 15) % 30;
                int i = h - (h / 28) * (1 - (29 / (h + 1)) * ((21 - g) / 11));
                int j = (y + (y / 4) + i + 2 - c + (c / 4)) % 7;
                int l = i - j;
                int month = 3 + ((l + 40) / 44);
                int day = l + 28 - 31 * (month / 4);

                DateTime result = new DateTime(y, month, day);
                ValidateDateTime(result);
                return result;
            });
        }
    }
}
