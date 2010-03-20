using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DirLinker.Interfaces;
using DirLinker.Commands;
using Rhino.Mocks;

namespace DirLinker.Tests.Commands
{

    public class MockCommand : ICommand
    {
        public Object[] ctorParams;
        public String CommandName { get; set; }

        public MockCommand(params Object[] constructorParams)
        {
            ctorParams = constructorParams;
        }

        #region ICommand Members

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string UserFeedback
        {
            get { throw new NotImplementedException(); }
        }

        public event RequestUserReponse AskUser;

        #endregion
    }

    public class MockCommandFactory : ICommandFactory
    {

        public ICommand MoveFileCommand(IFile source, IFile target, Boolean overwriteTarget)
        {
            return new MockCommand(source, target, overwriteTarget) { CommandName = "MoveFileCommand" };
        }
        public ICommand CreateFolder(IFolder folder)
        {
            return new MockCommand(folder) { CommandName = "CreateFolder" };
        }
        public ICommand DeleteFolderCommand(IFolder folder)
        {
            return new MockCommand(folder) { CommandName = "DeleteFolderCommand" };
        }
        public ICommand CreateLinkCommand(IFolder linkTo, IFolder linkFrom)
        {
            return new MockCommand(linkTo, linkFrom) { CommandName = "CreateLinkCommand" };
        }

    }

    [TestFixture]
    public class CommandDiscoveryTests
    {

        [Test]
        public void GetCommandListForTask_TargetDoesNotExist_JustCreateLinkCommandReturned()
        {
            ICommandFactory factory = new MockCommandFactory();
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(l => l.FolderExists()).Return(false);

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();

            ICommandDiscovery discoverer = new CommandDiscovery(factory);
            List<ICommand> taskList = discoverer.GetCommandListForTask(linkTo, linkFrom, false, false);

            Assert.IsTrue(taskList.Count() == 1, "There should be one item in the list");
            Assert.IsTrue(((MockCommand)taskList[0]).CommandName.Equals("CreateLinkCommand"));
        }

        [Test]
        public void GetCommandListForTask_TargetExist_DeleteFolderThenCreateLinkReturned()
        {
            ICommandFactory factory = new MockCommandFactory();
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(l => l.FolderExists()).Return(true);

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();

            ICommandDiscovery discoverer = new CommandDiscovery(factory);
            List<ICommand> taskList = discoverer.GetCommandListForTask(linkTo, linkFrom, false, false);

            Assert.IsTrue(taskList.Count() == 2, "There should be two items in the list");
            Assert.IsTrue(((MockCommand)taskList[0]).CommandName.Equals("DeleteFolderCommand"));
            Assert.IsTrue(((MockCommand)taskList[1]).CommandName.Equals("CreateLinkCommand"));
        }

        [Test]
        public void GetCommandListForTask_TargetExistsNoFiles_DeleteFolderThenCreateLinkReturned()
        {
            ICommandFactory factory = new MockCommandFactory();
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(l => l.FolderExists()).Return(true);

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();

            ICommandDiscovery discoverer = new CommandDiscovery(factory);
            List<ICommand> taskList = discoverer.GetCommandListForTask(linkTo, linkFrom, true, false);

            Assert.IsTrue(taskList.Count() == 2, "There should be three items in the list");
            Assert.IsTrue(((MockCommand)taskList[0]).CommandName.Equals("DeleteFolderCommand"));
            Assert.IsTrue(((MockCommand)taskList[1]).CommandName.Equals("CreateLinkCommand"));
        }

        [Test]
        public void GetCommandListForTask_TargetExistsOneFiles_MoveFileThenDeleteFolderThenCreateLinkReturned()
        {
            ICommandFactory factory = new MockCommandFactory();
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(l => l.FolderExists()).Return(true);
            linkTo.Stub(l => l.GetFileList()).Return(new List<IFile> 
                        { 
                            Helpers.CreateStubHelpers.GetIFileStub("1.txt", @"c:\path") 
                        });

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();

            ICommandDiscovery discoverer = new CommandDiscovery(factory);
            List<ICommand> taskList = discoverer.GetCommandListForTask(linkTo, linkFrom, true, false);

            Assert.IsTrue(taskList.Count() == 3, "There should be three items in the list");
            Assert.IsTrue(((MockCommand)taskList[0]).CommandName.Equals("MoveFileCommand"));
            Assert.IsTrue(((MockCommand)taskList[1]).CommandName.Equals("DeleteFolderCommand"));
            Assert.IsTrue(((MockCommand)taskList[2]).CommandName.Equals("CreateLinkCommand"));
        }

    }
}
