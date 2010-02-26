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
            folder.FolderExistsReturnValue = true;

            ICommand deleteCommand = new DeleteFolderCommand(folder);
            deleteCommand.Execute();

            Assert.IsTrue(folder.DeleteFolderCalled);
        }

        [Test]
        public void Undo_Creates_folder_when_undo_is_called_after_execute()
        {
            FakeFolder folder = new FakeFolder(@"c:\fakeFolder\");
            folder.FolderExistsReturnValue = true;

            ICommand deleteCommand = new DeleteFolderCommand(folder);
            deleteCommand.Execute();
            deleteCommand.Undo();

            Assert.IsTrue(folder.CreateFolderCalled);
        }

        [Test]
        public void Undo_Execute_not_called_folder_is_not_created()
        {
            FakeFolder folder = new FakeFolder(@"c:\fakeFolder\");
            folder.FolderExistsReturnValue = true;

            ICommand deleteCommand = new DeleteFolderCommand(folder);
            deleteCommand.Undo();

            Assert.IsFalse(folder.CreateFolderCalled);
        }

        [Test]
        public void Execute_folder_does_not_exist_delete_not_called()
        {
            FakeFolder folder = new FakeFolder(@"c:\fakeFolder\");
            folder.FolderExistsReturnValue = false;

            ICommand deleteCommand = new DeleteFolderCommand(folder);
            deleteCommand.Execute();

            Assert.IsFalse(folder.DeleteFolderCalled);
        }
    }
}
