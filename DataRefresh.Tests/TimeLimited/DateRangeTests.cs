using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExceptionsAssert = MSTestExtensions.ExceptionAssert;
using DataRefresh.TimeLimited;
using DateRangeArgs = System.Tuple<System.DateTimeOffset?, System.DateTimeOffset?, System.Boolean?>;

namespace DataRefresh.Tests.TimeLimited
{
    [TestClass]
    public class DateRangeTests
    {
        [TestMethod]
        public void DateRange_Init_Check()
        {
            // Check for ArgumentNullException throwing
            DateRangeArgs[] argNull =
            {
                new DateRangeArgs(null, null, true),
                new DateRangeArgs(null, null, false),
                new DateRangeArgs(DateTimeOffset.UtcNow, null, null),
                new DateRangeArgs(null, DateTimeOffset.UtcNow, null)
            };
            foreach (DateRangeArgs args in argNull)
            {
                ExceptionsAssert.Throws<ArgumentNullException>(() =>
                {
                    var test = new DateRange(args.Item1, args.Item2, args.Item3);
                });
            }

            // Check for ArgumentException throwing
            ExceptionsAssert.Throws<ArgumentException>(() =>
            {
                var test = new DateRange(
                    DateTimeOffset.UtcNow.AddSeconds(1),
                    DateTimeOffset.UtcNow.AddSeconds(-1),
                    true);
            });

            // Check each arrangement of valid input
            DateRangeArgs[] validArgs =
            {
                new DateRangeArgs(DateTimeOffset.UtcNow, null, true),
                new DateRangeArgs(null, DateTimeOffset.UtcNow, true),
                new DateRangeArgs(DateTimeOffset.UtcNow, null, false),
                new DateRangeArgs(null, DateTimeOffset.UtcNow, false),
                new DateRangeArgs(null, null, null),
                new DateRangeArgs(DateTimeOffset.UtcNow.AddSeconds(-1), DateTimeOffset.UtcNow.AddSeconds(1), true),
                new DateRangeArgs(DateTimeOffset.UtcNow.AddSeconds(-1), DateTimeOffset.UtcNow.AddSeconds(1), false)
            };
            foreach (DateRangeArgs args in validArgs)
            {
                var test = new DateRange(args.Item1, args.Item2, args.Item3);
            }
        }

        [TestMethod]
        public void DateRange_Init_Activate_Matches()
        {
            var active = new DateRange(DateTimeOffset.UtcNow, null, true);
            var inactive = new DateRange(DateTimeOffset.UtcNow, null, false);
            var empty = new DateRange(null, null, null);

            Assert.AreEqual(DateRangeActivation.Active, active.ActivateContent);
            Assert.AreEqual(DateRangeActivation.Inactive, inactive.ActivateContent);
            Assert.AreEqual(DateRangeActivation.Empty, empty.ActivateContent);
        }

        [TestMethod]
        public void DateRange_OverlapsWith_Verify()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateRangeArgs[][] overlapping =
            {
                // Two with start dates but no end dates
                // a =>     | ....->
                // b => | ....->
                new DateRangeArgs[] { new DateRangeArgs(now, null, true), new DateRangeArgs(now.AddSeconds(-100), null, true) },
                // Two with end dates but no start dates
                // a =>     <-.... |
                // b => <-.... |
                new DateRangeArgs[] { new DateRangeArgs(null, now, true), new DateRangeArgs(null, now.AddSeconds(-100), true) },
                // First range ends "now" with no start, second range starts 100s before "now" with no end
                // a => <-.... |
                // b =>     | ....->
                new DateRangeArgs[] { new DateRangeArgs(null, now, true), new DateRangeArgs(now.AddSeconds(-100), null, true) },
                // First range is complete, second range starts just before first range ends
                // a => | .... |
                // b =>     | ....->
                new DateRangeArgs[] { new DateRangeArgs(now, now.AddSeconds(100), true ), new DateRangeArgs(now.AddSeconds(50), null, true) },
                // First range is complete, second range ends just after first range starts
                // a =>     | .... |
                // b => <-.... |
                new DateRangeArgs[] { new DateRangeArgs(now, now.AddSeconds(100), true), new DateRangeArgs(null, now.AddSeconds(50), true) },
                // Both ranges are complete, and slightly overlap
                // a => | .... |
                // b =>     | .... |
                new DateRangeArgs[] { new DateRangeArgs(now, now.AddSeconds(100), true), new DateRangeArgs(now.AddSeconds(50), now.AddSeconds(150), true) }
            };
            DateRangeArgs[][] valid =
            {
                // First range ends just before second range starts
                // a => <-.... |
                // b =>          | ....->
                new DateRangeArgs[] { new DateRangeArgs(null, now, true), new DateRangeArgs(now.AddSeconds(100), null, true) },
                // Two complete ranges, first ends just before second starts
                // a => | .... |
                // b =>          | .... |
                new DateRangeArgs[] { new DateRangeArgs(now, now.AddSeconds(50), true), new DateRangeArgs(now.AddSeconds(100), now.AddSeconds(150), true) }
            };

            // Check that overlapping ranges return true
            foreach (DateRangeArgs[] args in overlapping)
            {
                var first = new DateRange(args[0].Item1, args[0].Item2, args[0].Item3);
                var second = new DateRange(args[1].Item1, args[1].Item2, args[1].Item3);
                // They should overlap regardless of which way we check them
                Assert.IsTrue(first.OverlapsWith(second));
                Assert.IsTrue(second.OverlapsWith(first));
            }

            // Check that valid ranges return false
            foreach (DateRangeArgs[] args in valid)
            {
                var first = new DateRange(args[0].Item1, args[0].Item2, args[0].Item3);
                var second = new DateRange(args[1].Item1, args[1].Item2, args[1].Item3);
                // They should remain valid regardless of which way we check them
                Assert.IsFalse(first.OverlapsWith(second));
                Assert.IsFalse(second.OverlapsWith(first));
            }
        }
    }
}
