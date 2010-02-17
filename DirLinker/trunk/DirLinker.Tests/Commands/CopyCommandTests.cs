using System;
using NUnit.Framework;
using DirLinker.Tests.Helpers;
using JunctionPointer.Commands;
using JunctionPointer.Interfaces;
using Rhino.Mocks;

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
        public void Execute_target_file_exists_no_overwrite_target_should_not_be_overwritten()
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

            sourceFile.AssertWasCalled(f => f.CopyFile(targetFile, false));
            
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
    }
}
