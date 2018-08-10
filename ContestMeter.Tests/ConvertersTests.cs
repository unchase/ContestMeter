using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContestMeter.Common;
using System.Threading;
using ContestMeter.UI;

namespace ContestMeter.Tests
{
    [TestClass]
    public class ConvertersTests
    {
        [TestMethod]
        public void ConvertTrueToVisible()
        {
            var unit = Converters.Visibility;

            var val = unit.Convert(true, typeof(System.Windows.Visibility),
                null, Thread.CurrentThread.CurrentUICulture);

            Assert.AreEqual(System.Windows.Visibility.Visible, val);
        }

        [TestMethod]
        public void ConvertFalseToCollapsed()
        {
            var unit = Converters.Visibility;

            var val = unit.Convert(false, typeof(System.Windows.Visibility),
                null, Thread.CurrentThread.CurrentUICulture);

            Assert.AreEqual(System.Windows.Visibility.Hidden, val);
        }

        [TestMethod]
        public void ConvertNullToVisible()
        {
            var unit = Converters.Visibility;

            var val = unit.Convert(null, typeof(System.Windows.Visibility),
                null, Thread.CurrentThread.CurrentUICulture);

            Assert.AreEqual(System.Windows.Visibility.Visible, val);
        }
    }
}
