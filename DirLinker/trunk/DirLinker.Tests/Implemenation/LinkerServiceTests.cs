using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DirLinker.Implemenation;
using DirLinker.Data;
using System.Windows.Threading;

namespace DirLinker.Tests.Implemenation
{
    [TestFixture]
    class LinkerServiceTests
    {
        [Test]
        public void GetStatusData_ValidMessengerPassed_StatusDataObjectIsReturned()
        {
            var linker = new LinkerService();

            FeedbackData data = linker.GetStatusData(Dispatcher.CurrentDispatcher);

            Assert.IsNotNull(data);
        }

        [Test]
        public void GetStatusData_NullPassed_ExceptionThrown()
        {
            var linker = new LinkerService();
            Assert.Throws<ArgumentNullException>(() => linker.GetStatusData(null));
        }

        [Test]
        public void PerformOperation_ValidData_CommandIsRequestedFromCommandDiscovery()
        {
            Assert.Fail();
        }

        [Test]
        public void PerformOperation_ValidDataAndDispatcher_StatusSetToCommandDiscoveryRunning()
        {
            Assert.Fail();
        }

        [Test]
        public void PerformOperation_ValidData_CommandsQueuedInCommandRunnerFromCommandDiscovery()
        {
            Assert.Fail();
        }

        [Test]
        public void PerformOperation_ValidData_CommandRunnerStarted()
        {
            Assert.Fail();
        }

        [Test]
        public void PerformOperation_ValidData_CommandRunnerStartedWithValidThreadMessenger()
        {
            Assert.Fail();
        }
    }
}
