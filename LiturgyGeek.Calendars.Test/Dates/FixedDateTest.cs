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
    public class FixedDateTest
    {
        private static readonly ChurchCalendarSystem westernCalendar = new ChurchCalendarSystem(new GregorianCalendar(), new GregorianPaschalCalendar());
        private static readonly ChurchCalendarSystem easternNewCalendar = new ChurchCalendarSystem(new RevisedJulianCalendar(), new JulianPaschalCalendar());
        private static readonly ChurchCalendarSystem easternOldCalendar = new ChurchCalendarSystem(new JulianCalendar(), new JulianPaschalCalendar());

        [TestMethod]
        public void TestSimpleCase()
        {
            var fixedDate = new FixedDate(3, 25);

            Assert.AreEqual(new DateTime(2022, 3, 25), GetInstance(fixedDate, westernCalendar, 2022));
            Assert.AreEqual(new DateTime(2022, 3, 25), GetInstance(fixedDate, easternNewCalendar, 2022));
            Assert.AreEqual(new DateTime(2022, 4, 7), GetInstance(fixedDate, easternOldCalendar, 2022));
        }

        [TestMethod]
        public void TestFebruary29()
        {
            var fixedDate = new FixedDate(2, 29);

            Assert.AreEqual(new DateTime(2020, 2, 29), GetInstance(fixedDate, westernCalendar, 2020));
            Assert.AreEqual(new DateTime(2020, 2, 29), GetInstance(fixedDate, easternNewCalendar, 2020));
            Assert.AreEqual(new DateTime(2020, 3, 13), GetInstance(fixedDate, easternOldCalendar, 2020));

            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2022));
            Assert.IsNull(GetInstance(fixedDate, easternNewCalendar, 2022));
            Assert.IsNull(GetInstance(fixedDate, easternOldCalendar, 2022));

            Assert.AreEqual(new DateTime(2000, 2, 29), GetInstance(fixedDate, westernCalendar, 2000));
            Assert.AreEqual(new DateTime(2000, 2, 29), GetInstance(fixedDate, easternNewCalendar, 2000));
            Assert.AreEqual(new DateTime(2000, 3, 13), GetInstance(fixedDate, easternOldCalendar, 2000));

            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2100));
            Assert.IsNull(GetInstance(fixedDate, easternNewCalendar, 2100));
            Assert.AreEqual(new DateTime(2100, 3, 14), GetInstance(fixedDate, easternOldCalendar, 2100));
        }

        [TestMethod]
        public void TestLastOfFebruary()
        {
            var fixedDate = new FixedDate(3, -1);

            Assert.AreEqual(new DateTime(2020, 2, 29), GetInstance(fixedDate, westernCalendar, 2020));
            Assert.AreEqual(new DateTime(2020, 2, 29), GetInstance(fixedDate, easternNewCalendar, 2020));
            Assert.AreEqual(new DateTime(2020, 3, 13), GetInstance(fixedDate, easternOldCalendar, 2020));

            Assert.AreEqual(new DateTime(2022, 2, 28), GetInstance(fixedDate, westernCalendar, 2022));
            Assert.AreEqual(new DateTime(2022, 2, 28), GetInstance(fixedDate, easternNewCalendar, 2022));
            Assert.AreEqual(new DateTime(2022, 3, 13), GetInstance(fixedDate, easternOldCalendar, 2022));

            Assert.AreEqual(new DateTime(2000, 2, 29), GetInstance(fixedDate, westernCalendar, 2000));
            Assert.AreEqual(new DateTime(2000, 2, 29), GetInstance(fixedDate, easternNewCalendar, 2000));
            Assert.AreEqual(new DateTime(2000, 3, 13), GetInstance(fixedDate, easternOldCalendar, 2000));

            Assert.AreEqual(new DateTime(2100, 2, 28), GetInstance(fixedDate, westernCalendar, 2100));
            Assert.AreEqual(new DateTime(2100, 2, 28), GetInstance(fixedDate, easternNewCalendar, 2100));
            Assert.AreEqual(new DateTime(2100, 3, 14), GetInstance(fixedDate, easternOldCalendar, 2100));
        }

        [TestMethod]
        public void TestAbsoluteDayOfWeek()
        {
            var fixedDate = new FixedDate(11, 1, DayOfWeek.Monday);

            Assert.AreEqual(new DateTime(2022, 11, 7), GetInstance(fixedDate, westernCalendar, 2022));
            Assert.AreEqual(new DateTime(2023, 11, 6), GetInstance(fixedDate, westernCalendar, 2023));
            Assert.AreEqual(new DateTime(2024, 11, 4), GetInstance(fixedDate, westernCalendar, 2024));
            Assert.AreEqual(new DateTime(2025, 11, 3), GetInstance(fixedDate, westernCalendar, 2025));
            Assert.AreEqual(new DateTime(2026, 11, 2), GetInstance(fixedDate, westernCalendar, 2026));
            Assert.AreEqual(new DateTime(2027, 11, 1), GetInstance(fixedDate, westernCalendar, 2027));
            Assert.AreEqual(new DateTime(2028, 11, 6), GetInstance(fixedDate, westernCalendar, 2028));
            Assert.AreEqual(new DateTime(2029, 11, 5), GetInstance(fixedDate, westernCalendar, 2029));
        }

        [TestMethod]
        public void TestWindow()
        {
            var fixedDate = new FixedDate(11, 1, DayOfWeek.Monday, 3);

            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2022));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2023));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2024));
            Assert.AreEqual(new DateTime(2025, 11, 3), GetInstance(fixedDate, westernCalendar, 2025));
            Assert.AreEqual(new DateTime(2026, 11, 2), GetInstance(fixedDate, westernCalendar, 2026));
            Assert.AreEqual(new DateTime(2027, 11, 1), GetInstance(fixedDate, westernCalendar, 2027));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2028));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2029));
        }

        [TestMethod]
        public void TestDayOfWeekRangeStart()
        {
            var fixedDate = new FixedDate(8, 15, DayOfWeek.Sunday, DayOfWeek.Tuesday);

            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2020));
            Assert.AreEqual(new DateTime(2021, 8, 15), GetInstance(fixedDate, westernCalendar, 2021));
            Assert.AreEqual(new DateTime(2022, 8, 15), GetInstance(fixedDate, westernCalendar, 2022));
            Assert.AreEqual(new DateTime(2023, 8, 15), GetInstance(fixedDate, westernCalendar, 2023));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2024));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2025));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2026));
            Assert.AreEqual(new DateTime(2027, 8, 15), GetInstance(fixedDate, westernCalendar, 2027));
            Assert.AreEqual(new DateTime(2028, 8, 15), GetInstance(fixedDate, westernCalendar, 2028));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2029));
        }

        [TestMethod]
        public void TestDayOfWeekRangeMiddle()
        {
            var fixedDate = new FixedDate(8, 15, DayOfWeek.Tuesday, DayOfWeek.Thursday);

            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2020));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2021));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2022));
            Assert.AreEqual(new DateTime(2023, 8, 15), GetInstance(fixedDate, westernCalendar, 2023));
            Assert.AreEqual(new DateTime(2024, 8, 15), GetInstance(fixedDate, westernCalendar, 2024));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2025));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2026));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2027));
            Assert.AreEqual(new DateTime(2028, 8, 15), GetInstance(fixedDate, westernCalendar, 2028));
            Assert.AreEqual(new DateTime(2029, 8, 15), GetInstance(fixedDate, westernCalendar, 2029));
        }

        [TestMethod]
        public void TestDayOfWeekRangeEnd()
        {
            var fixedDate = new FixedDate(8, 15, DayOfWeek.Thursday, DayOfWeek.Saturday);

            Assert.AreEqual(new DateTime(2020, 8, 15), GetInstance(fixedDate, westernCalendar, 2020));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2021));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2022));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2023));
            Assert.AreEqual(new DateTime(2024, 8, 15), GetInstance(fixedDate, westernCalendar, 2024));
            Assert.AreEqual(new DateTime(2025, 8, 15), GetInstance(fixedDate, westernCalendar, 2025));
            Assert.AreEqual(new DateTime(2026, 8, 15), GetInstance(fixedDate, westernCalendar, 2026));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2027));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2028));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2029));
        }

        [TestMethod]
        public void TestDayOfWeekRangeWraparound()
        {
            var fixedDate = new FixedDate(8, 15, DayOfWeek.Saturday, DayOfWeek.Monday);

            Assert.AreEqual(new DateTime(2020, 8, 15), GetInstance(fixedDate, westernCalendar, 2020));
            Assert.AreEqual(new DateTime(2021, 8, 15), GetInstance(fixedDate, westernCalendar, 2021));
            Assert.AreEqual(new DateTime(2022, 8, 15), GetInstance(fixedDate, westernCalendar, 2022));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2023));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2024));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2025));
            Assert.AreEqual(new DateTime(2026, 8, 15), GetInstance(fixedDate, westernCalendar, 2026));
            Assert.AreEqual(new DateTime(2027, 8, 15), GetInstance(fixedDate, westernCalendar, 2027));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2028));
            Assert.IsNull(GetInstance(fixedDate, westernCalendar, 2029));
        }

        [TestMethod]
        public void TestGetInstanceFollowing()
        {
            var febDate = new FixedDate(2, 1);

            Assert.AreEqual(new DateTime(2020, 2, 1), febDate.GetInstanceFollowing(new FixedDate(1, 31), westernCalendar, 2020));
            Assert.AreEqual(new DateTime(2021, 2, 1), febDate.GetInstanceFollowing(new FixedDate(2, 2), westernCalendar, 2020));
        }

        [TestMethod]
        public void TestGetInstanceFollowingMovable()
        {
            // Christ the King Sunday after Pentecost
            Assert.AreEqual(new DateTime(2038, 11, 21),
                new FixedDate(11, 20, DayOfWeek.Sunday)
                    .GetInstanceFollowing(new MovableDate(8, DayOfWeek.Sunday), westernCalendar, 2038));
            Assert.AreEqual(new DateTime(1818, 11, 22),
                new FixedDate(11, 20, DayOfWeek.Sunday)
                    .GetInstanceFollowing(new MovableDate(8, DayOfWeek.Sunday), westernCalendar, 1818));

            // Theophany after Pentecost
            Assert.AreEqual(new DateTime(2079, 1, 6),
                new FixedDate(1, 6).GetInstanceFollowing(new MovableDate(8, DayOfWeek.Sunday), easternNewCalendar, 2078));
            Assert.AreEqual(new DateTime(2011, 1, 6),
                new FixedDate(1, 6).GetInstanceFollowing(new MovableDate(8, DayOfWeek.Sunday), easternNewCalendar, 2010));
        }

        private DateTime? GetInstance(FixedDate fixedDate, ChurchCalendarSystem calendarSystem, int year)
        {
            var result = fixedDate.GetInstance(calendarSystem, year);
            if (result.HasValue)
                Assert.IsNull(fixedDate.GetInstance(calendarSystem, year, result));
            return result;
        }
    }
}
