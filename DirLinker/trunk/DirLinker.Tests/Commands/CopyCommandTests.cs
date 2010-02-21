using System;
using NUnit.Framework;
using DirLinker.Tests.Helpers;
using JunctionPointer.Commands;
using JunctionPointer.Interfaces;
using Rhino.Mocks;
using JunctionPointer.Exceptions;
using System.IO;

namespace DirLinker.Tests.Commands
{
    [TestFixture]
    public class CopyCommandTests
    { 
        
        [Test]
        public void Status_should_return_status_with_both_filenames_in()
        {
            FakeFile sourceFile = new FakeFile("file1");
            FakeFile targetFile = new FakeFile("file2");

            CopyCommand testCopyCommand = new CopyCommand(sourceFile, targetFile, false);
            String status = testCopyCommand.Status;

            Assert.That(status.Contains("file1"));
            Assert.That(status.Contains("file2"));
        }

        [Test]
        public void Execute_source_file_should_copy_to_target()
        {
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(@"c:\source\");
            sourceFile.Stub(s => s.FileName).Return("source");

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(s => s.Folder).Return(@"c:\source\");
            targetFile.Stub(s => s.FileName).Return("target");

            ICommand testCopyCommand = new CopyCommand(sourceFile, targetFile, false);
            testCopyCommand.Execute();

            sourceFile.AssertWasCalled(f => f.CopyFile(Arg<IFile>.Is.Same(targetFile), Arg<Boolean>.Is.Anything));
        }
        
        [Test]
        public void Execute_Target_File_Exists_When_Overwriting_Should_Check_Target_ReadOnly_Flag()
        {
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(@"c:\source");
            sourceFile.Stub(s => s.FileName).Return("File1");

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.Normal);
           
            ICommand testCopyCommand = new CopyCommand(sourceFile, targetFile, true);
            testCopyCommand.Execute();

            targetFile.AssertWasCalled(f => f.GetAttributes());
        }

        [Test]
        public void Execute_target_file_exists_no_overwrite_copy_not_attempted()
        {
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(@"c:\source\");
            sourceFile.Stub(s => s.FileName).Return("source");

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(s => s.Exists()).Return(true);
            targetFile.Stub(s => s.Folder).Return(@"c:\source\");
            targetFile.Stub(s => s.FileName).Return("target");

            ICommand testCopyCommand = new CopyCommand(sourceFile, targetFile, false);
            testCopyCommand.Execute();

            sourceFile.AssertWasNotCalled(f => f.CopyFile(Arg<IFile>.Is.Anything, Arg<Boolean>.Is.Anything));
        }

        [Test]
        public void Execute_target_file_exists_overwrite_target_should_be_overwritten()
        {
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(@"c:\source\");
            sourceFile.Stub(s => s.FileName).Return("source");

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(s => s.Exists()).Return(true);
            targetFile.Stub(s => s.Folder).Return(@"c:\source\");
            targetFile.Stub(s => s.FileName).Return("target");

            ICommand testCopyCommand = new CopyCommand(sourceFile, targetFile, true);
            testCopyCommand.Execute();

            sourceFile.AssertWasCalled(f => f.CopyFile(targetFile, true));

        }

        [Test]
        public void Undo_Copies_target_back_to_source_when_source_does_not_exist()
        {
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Exists()).Return(false);
            sourceFile.Stub(s => s.Folder).Return(@"c:\source\");
            sourceFile.Stub(s => s.FileName).Return("source");

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(s => s.Exists()).Return(true);
            targetFile.Stub(s => s.Folder).Return(@"c:\source\");
            targetFile.Stub(s => s.FileName).Return("target");

            ICommand testCopyCommand = new CopyCommand(sourceFile, targetFile, true);
            testCopyCommand.Execute();

            testCopyCommand.Undo();

            targetFile.AssertWasCalled(x => x.CopyFile(Arg<IFile>.Is.Same(sourceFile), Arg<Boolean>.Is.Anything));

        }

        [Test]
        public void Undo_Does_not_copy_target_to_source_if_source_exists()
        {
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Exists()).Return(true);
            sourceFile.Stub(s => s.Folder).Return(@"c:\source\");
            sourceFile.Stub(s => s.FileName).Return("source");

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(s => s.Exists()).Return(true);
            targetFile.Stub(s => s.Folder).Return(@"c:\source\");
            targetFile.Stub(s => s.FileName).Return("target");

            ICommand testCopyCommand = new CopyCommand(sourceFile, targetFile, true);
            testCopyCommand.Execute();

            testCopyCommand.Undo();

            targetFile.AssertWasNotCalled(x => x.CopyFile(Arg<IFile>.Is.Same(sourceFile), Arg<Boolean>.Is.Anything));

        }

        [Test]
        [ExpectedException(ExpectedException=typeof(CommandRunnerException))]
        public void Undo_Does_nothing_if_executed_has_not_been_called_first()
        {
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Exists()).Return(false);
            sourceFile.Stub(s => s.Folder).Return(@"c:\source\");
            sourceFile.Stub(s => s.FileName).Return("source");

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(s => s.Exists()).Return(true);
            targetFile.Stub(s => s.Folder).Return(@"c:\source\");
            targetFile.Stub(s => s.FileName).Return("target");

            ICommand testCopyCommand = new CopyCommand(sourceFile, targetFile, true);
            testCopyCommand.Undo();
        }

    }
}
