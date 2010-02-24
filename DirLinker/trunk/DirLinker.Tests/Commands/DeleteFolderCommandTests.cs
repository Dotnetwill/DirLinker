using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DirLinker.Tests.Helpers;
using JunctionPointer.Interfaces;
using JunctionPointer.Commands;

namespace DirLinker.Tests.Commands
{
    [TestFixture]
    public class DeleteFolderCommandTests
    {
        [Test]
        public void Execute_Folder_deleted()
        {
            FakeFolder folder = new FakeFolder(@"c:\fakeFolder\");

            ICommand deleteCommand = new DeleteFolderCommand(folder);
            deleteCommand.Execute();

            Assert.IsTrue(folder.DeleteFolderCalled);
        }
    }
}
