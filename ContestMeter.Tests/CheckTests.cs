using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContestMeter.Common;

namespace ContestMeter.Tests
{
    [TestClass]
    public class CheckTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckNotNullFailed()
        {
            Check.NotNull(null, "null");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckNotEmptyFailed()
        {
            Check.NotNullOrEmpty("", "null");
        }

        [TestMethod]
        public void CheckNotEmpty()
        {
            Check.NotNullOrEmpty("not-empty", "not-empty");
        }

        [TestMethod]
        public void CheckNotNull()
        {
            Check.NotNull("not-empty", "not-empty");
        }
    }
}
