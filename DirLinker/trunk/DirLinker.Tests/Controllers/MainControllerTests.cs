using System;

using NUnit.Framework;
using DirLinker.Interfaces.Views;
using DirLinker.Controllers;
using DirLinker.Interfaces;
using DirLinker.Tests.Helpers;
using DirLinker.Interfaces.Controllers;
using OCInject;

namespace DirLinker.Tests.Controllers
{
    [TestFixture]
    public class MainControllerTests
    {
 
        [Test]
        public void PerformOperation_ValuesFromViewPassedToWorker_PassedCorrectly()
        {
            IDirLinker stubLinker = new StubDirLinker();
            ILinkerView stubMainView = new StubMainView();
            StubWorkingController workingController = new StubWorkingController();

            ClassFactory classFactory = new ClassFactory();
            classFactory.RegisterType<ILinkerView>().AlwaysReturnObject(stubMainView);
            classFactory.RegisterType<IDirLinker>().AlwaysReturnObject(stubLinker);
            classFactory.RegisterType<IWorkingController>().AlwaysReturnObject(workingController);


            stubMainView.LinkPoint = @"Link point";
            stubMainView.LinkTo = @"Link to";
            stubMainView.CopyBeforeDelete = true;
            stubMainView.OverWriteTargetFiles = true;

            MainController controller = new MainController(classFactory);
            controller.PerformOperation(new Object(), new EventArgs());

            Assert.AreEqual(stubMainView.LinkPoint, workingController.LinkPoint);
            Assert.AreEqual(stubMainView.LinkTo, workingController.LinkTo);
            Assert.AreEqual(stubMainView.CopyBeforeDelete, workingController.CopyBeforeDelete);
            Assert.AreEqual(stubMainView.CopyBeforeDelete, workingController.OverWriteTargetFiles);
        }

    }
}
