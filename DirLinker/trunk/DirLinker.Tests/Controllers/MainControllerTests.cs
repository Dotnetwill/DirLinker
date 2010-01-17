using System;

using NUnit.Framework;
using JunctionPointer.Interfaces.Views;
using JunctionPointer.Controllers;
using JunctionPointer.Interfaces;
using DirLinker.Tests.Helpers;
using JunctionPointer.Interfaces.Controllers;

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

            UnitTestClassFactory classFactory = new UnitTestClassFactory();
            classFactory.ReturnObjectForType<ILinkerView>(stubMainView);
            classFactory.ReturnObjectForType<IDirLinker>(stubLinker);
            classFactory.ReturnObjectForType<IWorkingController>(workingController);


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
