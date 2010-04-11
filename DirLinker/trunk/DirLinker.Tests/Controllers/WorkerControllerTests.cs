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
using DirLinker.Data;
using System.Windows.Forms;

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

            link.AssertWasCalled(l => l.GetStatusData(Arg<Dispatcher>.Is.NotNull));
        }

        [Test]
        public void ShowWorker_ValidLinkService_FeedbackDataIsSetInTheView()
        {
            FeedbackData data = new FeedbackData();
            var link = MockRepository.GenerateMock<ILinkerService>();
            link.Stub(l => l.GetStatusData(Arg<Dispatcher>.Is.Anything)).Return(data);

            var view = MockRepository.GenerateMock<IWorkingView>();


            var workerController = new WorkerController(link, view);

            workerController.ShowWorker(null);

            view.AssertWasCalled(v => v.Feedback = data);
        }


        [Test]
        public void ShowWorker_ValidView_ViewIsShown()
        {
            FeedbackData data = new FeedbackData();
            var link = MockRepository.GenerateMock<ILinkerService>();
            link.Stub(l => l.GetStatusData(Arg<Dispatcher>.Is.Anything)).Return(data);

            var view = MockRepository.GenerateMock<IWorkingView>();

            var workerController = new WorkerController(link, view);

            workerController.ShowWorker(null);

            view.AssertWasCalled(v => v.Show(Arg<IWin32Window>.Is.Anything));
        }

        [Test]
        public void ShowWorker_ValidLinkService_LinkOperationIsStarted()
        {
            FeedbackData data = new FeedbackData();
            var link = MockRepository.GenerateMock<ILinkerService>();
            link.Stub(l => l.GetStatusData(Arg<Dispatcher>.Is.Anything)).Return(data);

            var view = MockRepository.GenerateMock<IWorkingView>();

            var workerController = new WorkerController(link, view);

            workerController.ShowWorker(null);

            link.AssertWasCalled(l => l.PerformOperation());
        }

        
    }
}
