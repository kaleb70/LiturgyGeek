using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Engine;
using LiturgyGeek.Calendars.Model;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Test.Engine
{
    [TestClass]
    public class CalendarReaderTest
    {
        [TestMethod]
        public void TestReadCalendar()
        {
            var json = File.ReadAllText(@"Engine\TestReadCalendar.json");
            var lines = File.ReadAllText(@"Engine\TestReadCalendar.txt");
            var testCalendar = new CalendarReader().Read(json, lines);

            Assert.AreEqual(5, testCalendar.Events.Count);

            Assert.AreEqual(
                new ChurchEvent()
                {
                    OccasionCode = "pascha",
                    Dates = new() { new MovableDate(1, DayOfWeek.Sunday) },
                    EventRankCode = "greatFeast",
                },
                testCalendar.Events[0]);
            Assert.AreEqual(
                new ChurchEvent()
                {
                    OccasionCode = "john",
                    Dates = new() { new FixedDate(5, 8) },
                    EventRankCode = "vigil",
                },
                testCalendar.Events[1]);
            Assert.AreEqual(
                new ChurchEvent()
                {
                    OccasionCode = "crossExaltation",
                    Dates = new() { new FixedDate(9, 14) },
                    EventRankCode = "greatFeast",
                    CommonRules = new() { "strictFastDay" },
                },
                testCalendar.Events[2]);
            Assert.AreEqual(
                new ChurchEvent()
                {
                    OccasionCode = "basilGreat",
                    Dates = new() { new FixedDate(1, 1) },
                    Flags = new() { "highlight" },
                },
                testCalendar.Events[3]);
            Assert.AreEqual(
                new ChurchEvent()
                {
                    OccasionCode = "theophanySundayBefore",
                    Dates = new()
                    {
                        new FixedDate(1, 1, DayOfWeek.Sunday, 5),
                        new FixedDate(1, 1, DayOfWeek.Monday, 1),
                        new FixedDate(1, 1, DayOfWeek.Tuesday, 1)
                    },
                    Flags = new() { "highlight" },
                },
                testCalendar.Events[4]);
        }
    }
}
