using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DirLinker.Interfaces;
using Rhino.Mocks;
using DirLinker.Interfaces.Controllers;
using DirLinker.Controllers;
using DirLinker.Tests.Helpers;

namespace DirLinker.Tests.Controllers
{
    [TestFixture]
    public class ProgressControllerTests
    {
        [Test]
        public void PerformLink_ValuesCorrectlyPassedToCommandDiscovery()
        {
            ICommandDiscovery mockDiscovery = MockRepository.GenerateMock<ICommandDiscovery>();

            IFolder linkFrom = new FakeFolder("linkfrom");
            IFolder linkTo = new FakeFolder("linkto");
            Boolean copyBeforeDelete = true;
            Boolean overwriteTargetFiles = true;


            IProgressController controller = new ProgressController(null, mockDiscovery);
            controller.PerformLink(linkFrom.FolderPath, linkFrom.FolderPath, copyBeforeDelete, overwriteTargetFiles);

            mockDiscovery.AssertWasCalled(m => m.GetCommandListForTask(linkFrom, linkTo, copyBeforeDelete, overwriteTargetFiles));
        }
    }
}
