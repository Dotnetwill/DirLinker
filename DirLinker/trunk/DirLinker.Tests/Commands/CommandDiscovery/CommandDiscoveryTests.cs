using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DirLinker.Interfaces;
using DirLinker.Commands;
using Rhino.Mocks;
using DirLinker.Tests.Helpers;

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
#pragma warning disable 00067

        public event RequestUserReponse AskUser;
#pragma warning restore 00067

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
        public ICommand CreateFolderLinkCommand(IFolder linkTo, IFolder linkFrom)
        {
            return new MockCommand(linkTo, linkFrom) { CommandName = "CreateLinkCommand" };
        }

        public ICommand CreateFileLinkCommand(IFile linkTo, IFile linkFrom)
        {
            return new MockCommand(linkTo, linkFrom) { CommandName = "CreateFileLinkCommand" };
        }


        #region ICommandFactory Members


        public ICommand DeleteFileCommand(IFile file)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [TestFixture]
    public class CommandDiscoveryTests
    {

        [Test]
        public void GetCommandListForFolderTask_TargetDoesNotExist_JustCreateLinkCommandReturned()
        {
            ICommandFactory factory = new MockCommandFactory();
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(l => l.FolderExists()).Return(false);

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();
            linkFrom.Stub(l => l.FolderExists()).Return(true);

            ICommandDiscovery discoverer = new CommandDiscovery(factory, null, null);
            List<ICommand> taskList = discoverer.GetCommandListForFolderTask(linkTo, linkFrom, false, false);

            Assert.IsTrue(taskList.Count() == 1, "There should be one item in the list");
            Assert.IsTrue(((MockCommand)taskList[0]).CommandName.Equals("CreateLinkCommand"));
        }

        [Test]
        public void GetCommandListForFolderTask_TargetExist_DeleteFolderThenCreateLinkReturned()
        {
            ICommandFactory factory = new MockCommandFactory();
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(l => l.FolderExists()).Return(true);

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();
            linkFrom.Stub(l => l.FolderExists()).Return(true);

            ICommandDiscovery discoverer = new CommandDiscovery(factory, null, null);
            List<ICommand> taskList = discoverer.GetCommandListForFolderTask(linkTo, linkFrom, false, false);

            Assert.IsTrue(taskList.Count() == 2, "There should be two items in the list");
            Assert.IsTrue(((MockCommand)taskList[0]).CommandName.Equals("DeleteFolderCommand"));
            Assert.IsTrue(((MockCommand)taskList[1]).CommandName.Equals("CreateLinkCommand"));
        }

        [Test]
        public void GetCommandListForFolderTask_TargetExistsNoFiles_DeleteFolderThenCreateLinkReturned()
        {
            ICommandFactory factory = new MockCommandFactory();
            FakeFolder linkTo = new FakeFolder() { FolderExistsReturnValue = true };
            
            FakeFolder linkFrom = new FakeFolder(@"c:\dest\");
            linkFrom.FolderExistsReturnValue = true;

            ICommandDiscovery discoverer = new CommandDiscovery(factory, s => new FakeFile(s), f => new FakeFolder(f));
            List<ICommand> taskList = discoverer.GetCommandListForFolderTask(linkTo, linkFrom, true, false);

            Assert.IsTrue(taskList.Count() == 2, "There should be three items in the list");
            Assert.IsTrue(((MockCommand)taskList[0]).CommandName.Equals("DeleteFolderCommand"));
            Assert.IsTrue(((MockCommand)taskList[1]).CommandName.Equals("CreateLinkCommand"));
        }

        [Test]
        public void GetCommandListForFolderTask_TargetExistsOneFiles_MoveFileThenDeleteFolderThenCreateLinkReturned()
        {
            ICommandFactory factory = new MockCommandFactory();
            FakeFolder linkTo = new FakeFolder();
            linkTo.FileList = new List<IFile> { Helpers.CreateStubHelpers.GetIFileStub("1.txt", @"c:\path") };
            linkTo.FolderExistsReturnValue = true;

            FakeFolder linkFrom = new FakeFolder(@"c:\dest\");
            linkFrom.FolderExistsReturnValue = true;

            ICommandDiscovery discoverer = new CommandDiscovery(factory, (f) => new FakeFile(f), f => new FakeFolder(f));
            List<ICommand> taskList = discoverer.GetCommandListForFolderTask(linkTo, linkFrom, true, false);

            Assert.IsTrue(taskList.Count() == 3, "There should be three items in the list");
            Assert.IsTrue(((MockCommand)taskList[0]).CommandName.Equals("MoveFileCommand"));
            Assert.IsTrue(((MockCommand)taskList[1]).CommandName.Equals("DeleteFolderCommand"));
            Assert.IsTrue(((MockCommand)taskList[2]).CommandName.Equals("CreateLinkCommand"));
        }

        [Test]
        public void GetCommandListForFolderTask_SourceDoesNotExists_CreateFolderCommandIsFirst()
        {
            ICommandFactory factory = new MockCommandFactory();
            IFolder linkTo = new FakeFolder(@"c:\target\");

            FakeFolder linkFrom = new FakeFolder(@"c:\destination\");
            linkFrom.FolderExistsReturnValue = false;

            ICommandDiscovery discoverer = new CommandDiscovery(factory, (f) => new FakeFile(f), f => new FakeFolder(f));
            List<ICommand> taskList = discoverer.GetCommandListForFolderTask(linkTo, linkFrom, false, false);
            
            MockCommand mockCommand = (MockCommand)taskList[0];

            Assert.AreEqual("CreateFolder", mockCommand.CommandName);
            Assert.AreEqual(mockCommand.ctorParams[0], linkFrom);   
        }

        [Test]
        public void GetCommandListForFolderTask_TargetHasEmptySubFolder_SubfolderCreatedInSource()
        {
            ICommandFactory factory = MockRepository.GenerateMock<ICommandFactory>();

            FakeFolder linkTo = new FakeFolder(@"c:\target\") { FolderExistsReturnValue = true };
            linkTo.SubFolderList = new List<IFolder>()
                {
                    new FakeFolder(@"c:\target\subfolder\")
                };

            FakeFolder linkFrom = new FakeFolder(@"c:\destination\") { FolderExistsReturnValue = true };

            ICommandDiscovery discoverer = new CommandDiscovery(factory, f => new FakeFile(f), f => new FakeFolder(f) { FolderExistsReturnValue = false });
            List<ICommand> taskList = discoverer.GetCommandListForFolderTask(linkTo, linkFrom, true, false);

            factory.AssertWasCalled(f => f.CreateFolder(Arg<IFolder>.Matches(folder => folder.FolderPath.Equals(@"c:\destination\subfolder\"))));
            
        }


        [Test]
        public void GetCommandListForFolderTask_TargetHasSubFolderWithOneWithNoOverwriteFile_SubfolderAndOneIsMovedCreatedInSource()
        {
            ICommandFactory factory = MockRepository.GenerateMock<ICommandFactory>();
            FakeFolder subFolder = new FakeFolder(@"c:\target\subfolder\");
            subFolder.FileList = new List<IFile> {  Helpers.CreateStubHelpers.GetIFileStub("1.txt", @"c:\target\subfolder\") };

            FakeFolder linkTo = new FakeFolder(@"c:\target\") { FolderExistsReturnValue = true };
            linkTo.SubFolderList = new List<IFolder> { subFolder };
            
            FakeFolder linkFrom = new FakeFolder(@"c:\destination\") { FolderExistsReturnValue = true };

            ICommandDiscovery discoverer = new CommandDiscovery(factory, f => new FakeFile(f), f => new FakeFolder(f) { FolderExistsReturnValue = false });
            discoverer.GetCommandListForFolderTask(linkTo, linkFrom, true, false);

            factory.AssertWasCalled(f => f.MoveFileCommand(
                Arg<IFile>.Matches(source => source.FullFilePath.Equals(@"c:\target\subfolder\1.txt")),
                Arg<IFile>.Matches(target => target.FullFilePath.Equals(@"c:\destination\subfolder\1.txt")),
                Arg<Boolean>.Is.Equal(false)
                ));
            
        }

        [Test]
        public void GetCommandListForFolderTask_TargetHasSubFolderWithOneWithOverwriteFile_SubfolderAndOneIsMovedCreatedInSource()
        {
            ICommandFactory factory = MockRepository.GenerateMock<ICommandFactory>();
            FakeFolder subFolder = new FakeFolder(@"c:\target\subfolder\");
            subFolder.FileList = new List<IFile> { Helpers.CreateStubHelpers.GetIFileStub("1.txt", @"c:\target\subfolder\") };

            FakeFolder linkTo = new FakeFolder(@"c:\target\") { FolderExistsReturnValue = true };
            linkTo.SubFolderList = new List<IFolder> { subFolder };

            FakeFolder linkFrom = new FakeFolder(@"c:\destination\") { FolderExistsReturnValue = true };

            ICommandDiscovery discoverer = new CommandDiscovery(factory, f => new FakeFile(f), f => new FakeFolder(f) { FolderExistsReturnValue = false });
            discoverer.GetCommandListForFolderTask(linkTo, linkFrom, true, true);

            factory.AssertWasCalled(f => f.MoveFileCommand(
                Arg<IFile>.Matches(source => source.FullFilePath.Equals(@"c:\target\subfolder\1.txt")),
                Arg<IFile>.Matches(target => target.FullFilePath.Equals(@"c:\destination\subfolder\1.txt")),
                Arg<Boolean>.Is.Equal(true)
                ));

        }

        [Test]
        public void GetCommandListTask_File_ReturnedCommandListContainsFileLink()
        {
          
            var factory = MockRepository.GenerateMock<ICommandFactory>();
            var commandDiscovery = new CommandDiscovery(factory, f => new FakeFile(f) { ExistsReturnValue = true}, f => new FakeFolder(f));

            commandDiscovery.GetCommandListTask("testFile", "", false, false);

            factory.AssertWasCalled(f => f.CreateFileLinkCommand(Arg<IFile>.Matches(file => file.FullFilePath.Equals("testFile")), Arg<IFile>.Is.Anything));
        }

        [Test]
        public void GetCommandListTask_Folder_ReturnedCommandListContainsFolderLink()
        {
            var factory = MockRepository.GenerateMock<ICommandFactory>();
            var commandDiscovery = new CommandDiscovery(factory, f => new FakeFile(f) { ExistsReturnValue = true }, f => new FakeFolder(f));

            commandDiscovery.GetCommandListTask("testFile", "", false, false);

            factory.AssertWasCalled(f => f.CreateFolderLinkCommand(Arg<IFolder>.Matches(f => f.FolderPath.Equals("testFile")), Arg<IFolder>.Is.Anything));
        }

        [Test]
        public void GetCommandListTask_File_TargetIsFolderFileNameIsAppended()
        {
            Assert.Fail(); 
        }

        [Test]
        public void GetCommandListTask_Folder_TargetIsFileExceptionThrown()
        {
            Assert.Fail();
        }

        [Test]
        public void GetCommandListForFileTask_LinkToDoesntExist_CreateLinkOnly()
        {
            FakeFile linkTo = new FakeFile("LinkTo");
            FakeFile linkFrom = new FakeFile("LinkFrom");

            var commandFactory = MockRepository.GenerateMock<ICommandFactory>();

            var commandDiscovery = new CommandDiscovery(commandFactory, f => new FakeFile(f), f => new FakeFolder(f));
            var commandList = commandDiscovery.GetCommandListForFileTask(linkTo, linkFrom, false, false);

            Assert.AreEqual(1, commandList.Count);
            commandFactory.AssertWasCalled(cf => cf.CreateFileLinkCommand(Arg<IFile>.Is.Equal(linkTo),
                                                                          Arg<IFile>.Is.Equal(linkFrom)));
        }
        
        [Test]
        public void GetCommandListForFileTask_LinkToExists_DeleteThenCreateLinkOnly()
        {
            FakeFile linkTo = new FakeFile("LinkTo");
            linkTo.ExistsReturnValue = true;
            FakeFile linkFrom = new FakeFile("LinkFrom");

            var commandFactory = MockRepository.GenerateMock<ICommandFactory>();

            var commandDiscovery = new CommandDiscovery(commandFactory, f => new FakeFile(f), f => new FakeFolder(f));
            var commandList = commandDiscovery.GetCommandListForFileTask(linkTo, linkFrom, false, false);

            Assert.AreEqual(2, commandList.Count);
            commandFactory.AssertWasCalled(cf => cf.DeleteFileCommand(Arg<IFile>.Is.Equal(linkTo)));

            commandFactory.AssertWasCalled(cf => cf.CreateFileLinkCommand(Arg<IFile>.Is.Equal(linkTo),
                                                                          Arg<IFile>.Is.Equal(linkFrom)));
        }

        [Test]
        public void GetCommandListForFileTask_LinkToExistsMoveFileOverWrite_MoveFileCommandAdded()
        {
            FakeFile linkTo = new FakeFile("LinkTo");
            linkTo.ExistsReturnValue = true;
            FakeFile linkFrom = new FakeFile("LinkFrom");

            var commandFactory = MockRepository.GenerateMock<ICommandFactory>();

            var commandDiscovery = new CommandDiscovery(commandFactory, f => new FakeFile(f), f => new FakeFolder(f));
            var commandList = commandDiscovery.GetCommandListForFileTask(linkTo, linkFrom, true, true);

            Assert.AreEqual(2, commandList.Count);
            commandFactory.AssertWasCalled(cf => cf.MoveFileCommand(Arg<IFile>.Is.Equal(linkTo), 
                                                                    Arg<IFile>.Is.Equal(linkFrom),
                                                                    Arg<Boolean>.Is.Equal(true)));

            commandFactory.AssertWasCalled(cf => cf.CreateFileLinkCommand(Arg<IFile>.Is.Equal(linkTo),
                                                                          Arg<IFile>.Is.Equal(linkFrom)));
        
        }

        [Test]
        public void GetCommandListForFileTask_LinkToExistsMoveFileNoOverWrite_MoveFileCommandAdded()
        {
            FakeFile linkTo = new FakeFile("LinkTo");
            linkTo.ExistsReturnValue = true;
            FakeFile linkFrom = new FakeFile("LinkFrom");

            var commandFactory = MockRepository.GenerateMock<ICommandFactory>();

            var commandDiscovery = new CommandDiscovery(commandFactory, f => new FakeFile(f), f => new FakeFolder(f));
            var commandList = commandDiscovery.GetCommandListForFileTask(linkTo, linkFrom, true, false);

            Assert.AreEqual(2, commandList.Count);
            commandFactory.AssertWasCalled(cf => cf.MoveFileCommand(Arg<IFile>.Is.Equal(linkTo), 
                                                                    Arg<IFile>.Is.Equal(linkFrom),
                                                                    Arg<Boolean>.Is.Equal(false)));

            commandFactory.AssertWasCalled(cf => cf.CreateFileLinkCommand(Arg<IFile>.Is.Equal(linkTo),
                                                                          Arg<IFile>.Is.Equal(linkFrom)));
        
        }
    }
}
