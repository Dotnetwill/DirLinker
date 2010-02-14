using System;
using NUnit.Framework;
using JunctionPointer.Controllers;
using DirLinker.Tests.Helpers;
using JunctionPointer.Interfaces.Views;
using Rhino.Mocks;

namespace DirLinker.Tests.Controllers
{
    [TestFixture]
    public class WorkDialogControllerTests
    {
        [Test]
        public void DoDirectoryLinkWithFeedBack_CallsDirLinkerWithTheCorrectParameters_CorrectParamsPassed()
        {
           
            IWorkingView mockView = MockRepository.GenerateMock<IWorkingView>();

            String linkPoint = "Link Point";
            String linkTo = "Link To";
            
            Boolean copyBeforeDelete = true;

            StubDirLinker stubLinker = new StubDirLinker();
            FakeBackgroundWorker bgWorker = new FakeBackgroundWorker();
            WorkingDialogController workingController = new WorkingDialogController(stubLinker, mockView, bgWorker);

            workingController.DoDirectoryLinkWithFeedBack(linkPoint, linkTo, copyBeforeDelete, true);

            Assert.AreEqual(linkPoint, stubLinker.LinkPoint);
            Assert.AreEqual(linkTo, stubLinker.LinkTo);
            Assert.AreEqual(copyBeforeDelete, stubLinker.CopyBeforeDelete);


        }
    }
}
