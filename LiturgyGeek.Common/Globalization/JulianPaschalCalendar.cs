using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Globalization
{
    public class JulianPaschalCalendar : PaschalCalendar
    {
        private static readonly ConcurrentDictionary<int, DateTime> paschaDictionary = new ConcurrentDictionary<int, DateTime>();
        private readonly JulianCalendar julianCalendar = new JulianCalendar();

        public override DateTime FindPascha(int year)
        {
            return paschaDictionary.GetOrAdd(year, y =>
            {
                int g = y % 19;
                int i = (19 * g + 15) % 30;
                int j = (y + (y / 4) + i) % 7;
                int l = i - j;
                int month = 3 + ((l + 40) / 44);
                int day = l + 28 - 31 * (month / 4);

                DateTime result = new DateTime(y, month, day, julianCalendar);
                ValidateDateTime(result);
                return result;
            });
        }
    }
}
