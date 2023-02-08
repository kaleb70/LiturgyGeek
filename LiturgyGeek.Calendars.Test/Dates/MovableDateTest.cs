using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Common.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Test.Dates
{
    [TestClass]
    public class MovableDateTest
    {
        private static readonly ChurchCalendarSystem westernCalendar = new ChurchCalendarSystem(new GregorianCalendar(), new GregorianPaschalCalendar());
        private static readonly ChurchCalendarSystem easternNewCalendar = new ChurchCalendarSystem(new RevisedJulianCalendar(), new JulianPaschalCalendar());
        private static readonly ChurchCalendarSystem easternOldCalendar = new ChurchCalendarSystem(new JulianCalendar(), new JulianPaschalCalendar());

        [TestMethod]
        public void TestGetInstanceFollowingFixed()
        {
            // Ash Wednesday after Christ the King Sunday
            Assert.AreEqual(new DateTime(2038, 3, 10),
                new MovableDate(-7, DayOfWeek.Wednesday)
                    .GetInstanceFollowing(new FixedDate(11, 20, DayOfWeek.Sunday), westernCalendar, 2037));
            Assert.AreEqual(new DateTime(1818, 2, 4),
                new MovableDate(-7, DayOfWeek.Wednesday)
                    .GetInstanceFollowing(new FixedDate(11, 20, DayOfWeek.Sunday), westernCalendar, 1817));

            // Cheesefare Sunday after Theophany
            Assert.AreEqual(new DateTime(2078, 3, 20),
                new MovableDate(-7, DayOfWeek.Sunday).GetInstanceFollowing(new FixedDate(1, 6), easternNewCalendar, 2078));
            Assert.AreEqual(new DateTime(2010, 2, 14),
                new MovableDate(-7, DayOfWeek.Sunday).GetInstanceFollowing(new FixedDate(1, 6), easternNewCalendar, 2010));
        }
    }
}
