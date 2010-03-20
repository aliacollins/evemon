using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using EVEMon.Common;

namespace Tests.EVEMon.Common
{
    [TestFixture]
    public class TimeUtilTests
    {
        // Some constants for testing
        private readonly DateTime pointInTime = new DateTime(2010, 05, 07, 18, 23, 32);
        private const string validCcpDateTime = "2010-05-07 18:23:32";
        private const string invalidCcpDateTime = "18:23:32 2010-05-07";

        /// <summary>
        /// Able to convert a <c>DateTime</c> to a CCPTime.
        /// </summary>
        [Test]
        public void ConvertDateTimeToCCPDateTime()
        {
            string result = TimeUtil.ConvertDateTimeToCCPTimeString(pointInTime);
            Assert.AreEqual(validCcpDateTime, result);
        }

        /// <summary>
        /// Able to convert a correctly formatted CCPDateTime to <c>DateTime</c>.
        /// </summary>
        [Test]
        public void ConvertValidCCPDateTimeToDateTime()
        {
            DateTime result = TimeUtil.ConvertCCPTimeStringToDateTime(validCcpDateTime);
            Assert.AreEqual(pointInTime, result);
        }

        /// <summary>
        /// Handles an incorrect input string appropiately.
        /// </summary>
        /// <remarks>
        /// Exception thrown is a .NET Exception so there is no need to test the exception itself.
        /// </remarks>
        [Test]
        public void ConvertInvalidCCPDateTimeToDateTime()
        {
            Assert.Throws<FormatException>
                (delegate()
                {
                    TimeUtil.ConvertCCPTimeStringToDateTime(invalidCcpDateTime);
                }
            );
        }
    }
}
