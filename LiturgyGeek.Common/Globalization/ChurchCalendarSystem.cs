using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Globalization
{
    public class ChurchCalendarSystem
    {
        public Calendar FixedCalendar { get; private init; }

        public PaschalCalendar MovableCalendar { get; private init; }

        public ChurchCalendarSystem(Calendar fixedCalendar, PaschalCalendar movableCalendar)
        {
            FixedCalendar = fixedCalendar;
            MovableCalendar = movableCalendar;
        }
    }
}
