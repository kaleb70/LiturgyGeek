using LiturgyGeek.Framework.Clcs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    public interface ICalendarReader
    {
        ChurchCalendar Read(Stream jsonStream, Stream? lineStream = null);

        ChurchCalendar Read(string json, string? lines = null);
    }
}
