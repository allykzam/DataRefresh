using System;
using System.Linq;
using System.Collections.Generic;
using DataRefresh.TimeLimited;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DateRangeArgs = System.Tuple<System.DateTimeOffset?, System.DateTimeOffset?, System.Boolean?>;
using ExceptionsAssert = MSTestExtensions.ExceptionAssert;

namespace DataRefresh.Tests.TimeLimited
{
    [TestClass]
    public class TimeLimitedTests
    {
        private class TimeLimited_OutsideRanges : TimeLimited<DateRange, Int32>
        {
            private bool _AvailableOutsideRanges = true;
            public override bool AvailableOutsideRanges
            {
                get
                {
                    return _AvailableOutsideRanges;
                }
            }

            public void SetVal(bool value)
            {
                _AvailableOutsideRanges = value;
            }

            public TimeLimited_OutsideRanges(
                IEnumerable<DateRange> dateRanges,
                Int32 identifier)
                : base(dateRanges, identifier)
            { }
        }

        private class TimeLimited_WithoutRanges : TimeLimited<DateRange, Int32>
        {
            private bool _AvailableWithoutRanges = true;
            public override bool AvailableWithoutRanges
            {
                get
                {
                    return _AvailableWithoutRanges;
                }
            }

            public void SetVal(bool value)
            {
                _AvailableWithoutRanges = value;
            }

            public TimeLimited_WithoutRanges(
                IEnumerable<DateRange> dateRanges,
                Int32 identifier)
                : base(dateRanges, identifier)
            { }
        }

        [TestMethod]
        public void TimeLimited_IsAvailable()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Note that the active values need to include an entry with a
            // blank date range
            //
            // Additionally, the inactive values need to include all of the
            // active ranges *EXCEPT* the empty one, but with the Activate
            // flags all set to false
            DateRangeArgs[][] active =
            {
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-1), null,              true) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-0), null,              true) },
                new DateRangeArgs[] { new DateRangeArgs(null,               now.AddSeconds(1), true) },
                new DateRangeArgs[] { new DateRangeArgs(null,               now.AddSeconds(0), true) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-1), now.AddSeconds(1), true) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-0), now.AddSeconds(1), true) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-1), now.AddSeconds(0), true) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-0), now.AddSeconds(0), true) },
                new DateRangeArgs[] { new DateRangeArgs(null,               null,              null) },
                new DateRangeArgs[] { }
            };
            DateRangeArgs[][] inactive =
            {
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(1),  null,               true)  },
                new DateRangeArgs[] { new DateRangeArgs(null,               now.AddSeconds(-1), true)  },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(1),  now.AddSeconds(2),  true)  },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-1), null,               false) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-0), null,               false) },
                new DateRangeArgs[] { new DateRangeArgs(null,               now.AddSeconds(1),  false) },
                new DateRangeArgs[] { new DateRangeArgs(null,               now.AddSeconds(0),  false) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-1), now.AddSeconds(1),  false) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-0), now.AddSeconds(1),  false) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-1), now.AddSeconds(0),  false) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-0), now.AddSeconds(0),  false) },
                new DateRangeArgs[] { new DateRangeArgs(now.AddSeconds(-2), now.AddSeconds(-1), true)  }
            };

            // Check each active value
            foreach (DateRangeArgs[] arg in active)
            {
                var ranges = arg.Select((x) => new DateRange(x.Item1, x.Item2, x.Item3));
                TimeLimited<DateRange, Int32> value = new TimeLimited<DateRange, Int32>(ranges, 0);
                Assert.IsTrue(value.IsAvailable(now));
            }

            // Check each inactive value
            foreach (DateRangeArgs[] arg in inactive)
            {
                var ranges = arg.Select((x) => new DateRange(x.Item1, x.Item2, x.Item3));
                TimeLimited<DateRange, Int32> value = new TimeLimited<DateRange, Int32>(ranges, 0);
                Assert.IsFalse(value.IsAvailable(now));
            }
        }

        [TestMethod]
        public void TimeLimited_Ctor_Exceptions()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Check for an exception passing bad date ranges
            ExceptionsAssert.Throws<ArgumentException>(() =>
            {
                var test = new TimeLimited<DateRange, Int32>(
                    new DateRange[]
                    {
                        new DateRange(null, null, null),
                        new DateRange(null, null, null)
                    },
                    0);
            });

            // Check for an exception passing overlapping date ranges
            ExceptionsAssert.Throws<ArgumentException>(() =>
            {
                var test = new TimeLimited<DateRange, Int32>(
                    new DateRange[]
                    {
                        new DateRange(now.AddSeconds(-1), now.AddSeconds(1), true),
                        new DateRange(now, now.AddSeconds(2), true)
                    },
                    0);
            });
        }

        [TestMethod]
        public void TimeLimited_AvailableWithoutRanges()
        {
            // Arrange
            var notAvailable = new TimeLimited_WithoutRanges(
                new DateRange[] { },
                0);
            var available = new TimeLimited_WithoutRanges(
                new DateRange[] { },
                0);

            // Act
            notAvailable.SetVal(false);
            available.SetVal(true);

            // Assert
            Assert.IsFalse(notAvailable.IsAvailable(DateTimeOffset.UtcNow));
            Assert.IsTrue(available.IsAvailable(DateTimeOffset.UtcNow));
        }

        [TestMethod]
        public void TimeLimited_AvailableOutsideRanges()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            // Arrange
            var notAvailable = new TimeLimited_OutsideRanges(
                new DateRange[] { new DateRange(now.AddSeconds(1), null, true) },
                0);
            var available = new TimeLimited_OutsideRanges(
                new DateRange[] { new DateRange(now.AddSeconds(1), null, true) },
                0);

            // Act
            notAvailable.SetVal(false);
            available.SetVal(true);

            // Assert
            Assert.IsFalse(notAvailable.IsAvailable(DateTimeOffset.UtcNow));
            Assert.IsTrue(available.IsAvailable(DateTimeOffset.UtcNow));
        }
    }
}
