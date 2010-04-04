using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DirLinker.Interfaces;
using Rhino.Mocks;
using DirLinker.Controllers;
using DirLinker.Interfaces.Views;
using System.Windows.Threading;

namespace DirLinker.Tests.Controllers
{
    [TestFixture]
    public class WorkerControllerTests
    {
        [Test]
        public void ShowWorker_ValidLinkService_DispatcherIsSet()
        {
            var link = MockRepository.GenerateMock<ILinkerService>();
            var view = MockRepository.GenerateMock<IWorkingView>();

            var workerController = new WorkerController(link, view);

            workerController.ShowWorker(null);

            link.AssertWasCalled(l => l.SetDispatcher(Arg<Dispatcher>.Is.NotNull));
        }
    }
}
