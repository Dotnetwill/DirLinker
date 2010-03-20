using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DirLinker.Tests.Helpers;
using DirLinker.Interfaces;
using DirLinker.Commands;
using DirLinker.Exceptions;

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

        [Test]
        public void UserFeedback_returns_a_status_containing_the_folder_name()
        {
            string folderPath = @"fakeFolder";
            FakeFolder folder = new FakeFolder(folderPath);
            folder.FolderExistsReturnValue = true;

            ICommand deleteCommand = new DeleteFolderCommand(folder);


            StringAssert.Contains(folderPath, deleteCommand.UserFeedback);
        }

        [Test]
        public void Execute_Folder_contains_files_files_delete_first()
        {
            FakeFolder folder = new FakeFolder(@"fakeFolder");
            folder.FolderExistsReturnValue = true;
            folder.FileList = new List<IFile>() {
                                                new FakeFile("file1"),
                                                new FakeFile("file2"),
                                                };

            ICommand deleteComand = new DeleteFolderCommand(folder);
            deleteComand.Execute();

            Assert.IsTrue(folder.FileList.TrueForAll(f => ((FakeFile)f).DeleteCalled), "Not all the files were deleted before trying to delete the folder!");

        }

        [Test]
        public void Execute_folder_contains_readonly_files_readonly_attribute_cleared()
        {
            FakeFolder folder = new FakeFolder(@"fakeFolder");
            folder.FolderExistsReturnValue = true;
            folder.FileList = new List<IFile>() {
                                                new FakeFile("file1") { Attributes = System.IO.FileAttributes.ReadOnly },
                                                new FakeFile("file2") { Attributes = System.IO.FileAttributes.ReadOnly },
                                                };

            ICommand command = new DeleteFolderCommand(folder);
            command.Execute();
            Assert.IsTrue(folder.FileList.TrueForAll(f => ((FakeFile)f).Attributes == System.IO.FileAttributes.Normal));
        }

        [Test]
        public void Execute_throws_if_folder_contains_any_subfolders()
        {
            FakeFolder folder = new FakeFolder(@"fakeFolder");
            folder.FolderExistsReturnValue = true;
            folder.SubFolderList = new List<IFolder>() {
                                                new FakeFolder("folder1"),
                                                new FakeFolder("folder2"),
                                                };

            ICommand deleteComand = new DeleteFolderCommand(folder);
            
            Assert.Throws<DirLinkerException>(() => deleteComand.Execute());
        }
    }
}
