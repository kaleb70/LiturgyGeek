using LiturgyGeek.Calendars.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Test.Dates
{
    [TestClass]
    public class ChurchDateTest
    {
        [TestMethod]
        public void TestParseWeeklyDate()
        {
            Assert.AreEqual(new WeeklyDate(DayOfWeek.Monday), ChurchDate.Parse("Monday"));
        }

        [TestMethod]
        public void TestParseMovableDate()
        {
            Assert.AreEqual(new MovableDate(-1, DayOfWeek.Sunday), ChurchDate.Parse("-1/Sunday"));
            Assert.AreEqual(new MovableDate(6, DayOfWeek.Thursday), ChurchDate.Parse("6/Thursday"));
        }

        [TestMethod]
        public void TestParseFixedDate()
        {
            Assert.AreEqual(new FixedDate(1, 6), ChurchDate.Parse("1/6"));
            Assert.AreEqual(new FixedDate(12, 25), ChurchDate.Parse("12/25"));

            Assert.AreEqual(new FixedDate(11, 27, DayOfWeek.Sunday), ChurchDate.Parse("11/27/Sunday"));
            Assert.AreEqual(new FixedDate(12, 22, DayOfWeek.Thursday, 3), ChurchDate.Parse("12/22/Thursday/3"));

            Assert.AreEqual(new FixedDate(1, 1, DayOfWeek.Monday, DayOfWeek.Tuesday), ChurchDate.Parse("1/1/Monday-Tuesday"));
        }

        [TestMethod]
        public void TestParseMonthlyDate()
        {
            Assert.AreEqual(new MonthlyDate(10), ChurchDate.Parse("*/10"));
            Assert.AreEqual(new MonthlyDate(10, DayOfWeek.Sunday), ChurchDate.Parse("*/10/Sunday"));
        }

        [TestMethod]
        public void TestSerialization()
        {
            var original = new DateContainer(new MovableDate(-1, DayOfWeek.Sunday));
            var text = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<DateContainer>(text);

            Assert.AreEqual("{\"Date\":\"-1/Sunday\"}", text);
            Assert.AreEqual(original, result);
        }

        public class DateContainer
        {
            public ChurchDate Date { get; set; }

            public DateContainer(ChurchDate date)
            {
                Date = date;
            }

            public override string ToString() => Date.ToString();

            public override bool Equals(object? obj) => obj is DateContainer other && Date.Equals(other.Date);

            public override int GetHashCode() => Date.GetHashCode();
        }
    }
}
