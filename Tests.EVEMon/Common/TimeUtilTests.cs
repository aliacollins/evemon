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
        
        /// <summary>
        /// Valid time as returned by CCP.
        /// </summary>
        private string ValidCcpDateTime
        {
            get
            {
                return "2010-05-07 18:23:32";
            }
        }

        /// <summary>
        /// Invalid time, wrong format.
        /// </summary>
        private string InvalidCcpDateTime
        {
            get
            {
                return "18:23:32 2010-05-07";
            }
        }

        /// <summary>
        /// A point in time.
        /// </summary>
        private DateTime PointInTime
        {
            get
            {
                return new DateTime(2010, 05, 07, 18, 23, 32);;
            }
        }


        /// <summary>
        /// Able to convert a <c>DateTime</c> to a CCPTime.
        /// </summary>
        [Test]
        public void ConvertDateTimeToCCPDateTime()
        {
            string result = PointInTime.ToCCPTimeString();
            Assert.AreEqual(ValidCcpDateTime, result);
        }

        /// <summary>
        /// Able to convert a correctly formatted CCPDateTime to <c>DateTime</c>.
        /// </summary>
        [Test]
        public void ConvertValidCCPDateTimeToDateTime()
        {
            DateTime result = ValidCcpDateTime.CCPTimeStringToDateTime();
            Assert.AreEqual(PointInTime, result);
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
                    InvalidCcpDateTime.CCPTimeStringToDateTime();
                }
            );
        }
    }
}
