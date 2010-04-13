using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DirLinker.Implemenation;
using DirLinker.Data;
using System.Windows.Threading;
using Rhino.Mocks;
using DirLinker.Interfaces;
using DirLinker.Tests.Helpers;

namespace DirLinker.Tests.Implemenation
{
    [TestFixture]
    class LinkerServiceTests
    {
        [Test]
        public void GetStatusData_ValidMessengerPassed_StatusDataObjectIsReturned()
        {
            var linker = GetLinkerService();

            FeedbackData data = linker.GetStatusData(Dispatcher.CurrentDispatcher);

            Assert.IsNotNull(data);
        }

        [Test]
        public void GetStatusData_NullPassed_ExceptionThrown()
        {
            var linker = GetLinkerService();
            Assert.Throws<ArgumentNullException>(() => linker.GetStatusData(null));
        }

        [Test]
        public void PerformOperation_ValidData_CommandIsRequestedFromCommandDiscovery()
        {
            String linkTo = "Test2";
            String linkFrom = "Test1";

            var commandDiscovery = MockRepository.GenerateMock<ICommandDiscovery>();
            var linker = GetLinkerService(commandDiscovery, f => new FakeFolder(f));

            LinkOperationData data = new LinkOperationData();
            data.CopyBeforeDelete = true;
            data.CreateLinkAt = linkFrom;
            data.LinkTo = linkTo;
            data.OverwriteExistingFiles = true;

            linker.SetOperationData(data);

            linker.PerformOperation();

            commandDiscovery.AssertWasCalled(d => d.GetCommandListForTask(
                                                  Arg<IFolder>.Matches(f => f.FolderPath.Equals(linkTo)),
                                                  Arg<IFolder>.Matches(f => f.FolderPath.Equals(linkFrom)), 
                                                  data.CopyBeforeDelete, 
                                                  data.OverwriteExistingFiles));

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

        private LinkerService GetLinkerService()
        {
            var commandDiscovery = MockRepository.GenerateMock<ICommandDiscovery>();
            return GetLinkerService(commandDiscovery);
        }

        private LinkerService GetLinkerService(ICommandDiscovery commandDiscovery)
        {
            return GetLinkerService(commandDiscovery, f => new FakeFolder(f));
        }

        private LinkerService GetLinkerService(ICommandDiscovery commandDiscovery, IFolderFactoryForPath folderFactory)
        {
            return new LinkerService(commandDiscovery, folderFactory);
        }

    }
}
