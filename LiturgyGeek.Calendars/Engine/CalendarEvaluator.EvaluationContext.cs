using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Model;
using LiturgyGeek.Common.Collections;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    partial class CalendarEvaluator
    {
        private class EvaluationContext
        {
            private readonly CalendarEvaluator evaluator;

            private readonly Dictionary<DateInstanceKey, DateInstanceContainer> dateInstanceCache
                = new Dictionary<DateInstanceKey, DateInstanceContainer>();

            public EvaluationContext(CalendarEvaluator evaluator)
            {
                this.evaluator = evaluator;
            }

            public DateInstanceContainer GetDateInstanceContainer(int basisYear, ChurchDate date, ChurchDate? priorDate = null)
            {
                DateInstanceKey key = new DateInstanceKey(basisYear, date, priorDate);
                if (!dateInstanceCache.TryGetValue(key, out var result))
                {
                    result = new DateInstanceContainer();
                    if (date.IsRecurring)
                    {
                        var resultSet = new HashSet<DateTime>();

                        DateTime? single = null;
                        while ((single = date.GetInstance(evaluator.calendarSystem, basisYear, single)) != null)
                            resultSet.Add(single.Value);

                        switch (resultSet.Count)
                        {
                            case 1:
                                result.Single = resultSet.Single();
                                break;

                            case > 1:
                                result._RecurringHash = resultSet;
                                break;
                        }
                    }
                    else
                        result.Single = date.GetInstanceFollowing(priorDate, evaluator.calendarSystem, basisYear);

                    dateInstanceCache[key] = result;
                }
                return result;
            }

            public IEnumerable<DateTime> GetDateInstances(int basisYear, ChurchDate date, ChurchDate? priorDate = null)
            {
                var container = GetDateInstanceContainer(basisYear, date, priorDate);
                return container.IsRecurring ? container.Recurring! : SingleOrNone();

                IEnumerable<DateTime> SingleOrNone()
                {
                    if (!container.IsUndefined)
                        yield return container.Single!.Value;
                }
            }

            public DateTime? GetSingleDateInstance(int basisYear, ChurchDate date, ChurchDate? priorDate = null)
            {
                if (date.IsRecurring)
                    throw new InvalidOperationException($"Can't call GetSingleDateInstance() for recurring date {date}.");

                return GetDateInstanceContainer(basisYear, date, priorDate).Single;
            }

            private struct DateInstanceKey
            {
                public int basisYear;
                public ChurchDate date;
                public ChurchDate? priorDate;

                public DateInstanceKey(int basisYear, ChurchDate date, ChurchDate? priorDate)
                {
                    this.basisYear = basisYear;
                    this.date = date;
                    this.priorDate = priorDate;
                }
            }

            public struct DateInstanceContainer
            {
                public DateTime? Single { get; set; }

                // This pattern helps to protect the collection from being accidentally
                // modified once it has been put into the struct.
                public IReadOnlyCollection<DateTime>? Recurring => _RecurringHash;
                public HashSet<DateTime>? _RecurringHash { private get; set; }

                public bool IsUndefined => Single == null && Recurring == null;
                public bool IsRecurring => Recurring != null;
            }
        }
    }
}
