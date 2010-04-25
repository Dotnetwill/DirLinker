using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Implementation;
using NUnit.Framework;
using DirLinker.Data;
using DirLinker.Interfaces;
using DirLinker.Tests.Helpers;

namespace DirLinker.Tests.Implemenation
{
    [TestFixture]
    public class OperationValidationTests
    {
        [Test]
        public void ValidOperation_ValidData_TrueIsReturned()
        {
            var validator = new OperationValidation(f => new FakeFile(f));

            var data = new LinkOperationData() { CreateLinkAt = "test1", LinkTo = "test2" };

            String errorMessage;
            Boolean valid = validator.ValidOperation(data, out errorMessage);

            Assert.IsTrue(valid);
        }

        [Test]
        public void ValidOperation_LinkAndPathDoNotMatch_FalseIsReturnedAlongWithErrorMessage()
        {
            var validator = new OperationValidation(f => new FakeFile(f));

            var data = new LinkOperationData() {CreateLinkAt = "test", LinkTo = "test"};
            String errorMessage;
            Boolean valid = validator.ValidOperation(data, out errorMessage);

            Assert.IsFalse(valid);
            Assert.AreEqual("A path can not be linked to itself", errorMessage);
        }

        [Test]
        public void ValidOperation__CreateLinkAtExistsNoCopyBeforeDeleteLinkToDoesNotExist_FalseIsReturnedAlongWithErrorMessage()
        {
            String createLinkAt = "linkAt";
            String linkTo = "linkTo";
            var fileFactory = GetFileFactoryThatReturnsExistsFor(createLinkAt);
            var validator = new OperationValidation(fileFactory);
            

            var data = new LinkOperationData() { CreateLinkAt = createLinkAt, LinkTo = linkTo, CopyBeforeDelete =  false};

            String errorMessage;
            Boolean valid = validator.ValidOperation(data, out errorMessage);

            Assert.IsFalse(valid);
            Assert.AreEqual("When creating a file link the linked to file must exist", errorMessage);
        }

        [Test]
        public void ValidOperation_File_CreateLinkAtExistsCopyBeforeDeleteLinkToDoesNotExist_ValidOperation()
        {
            String createLinkAt = "linkAt";
            String linkTo = "linkTo";
            var fileFactory = GetFileFactoryThatReturnsExistsFor(createLinkAt);
            var validator = new OperationValidation(fileFactory);


            var data = new LinkOperationData() { CreateLinkAt = createLinkAt, LinkTo = linkTo, CopyBeforeDelete = true };

            String errorMessage;
            Boolean valid = validator.ValidOperation(data, out errorMessage);

            Assert.IsTrue(valid);
        }

        [Test]
        public void ValidOperation_File_LinkToExists_ValidOperation()
        {
            String createLinkAt = "linkAt";
            String linkTo = "linkTo";
            var fileFactory = GetFileFactoryThatReturnsExistsFor(linkTo);
            var validator = new OperationValidation(fileFactory);


            var data = new LinkOperationData() { CreateLinkAt = createLinkAt, LinkTo = linkTo, CopyBeforeDelete = true };

            String errorMessage;
            Boolean valid = validator.ValidOperation(data, out errorMessage);

            Assert.IsTrue(valid);
        }

        private IFileFactoryForPath GetFileFactoryThatReturnsExistsFor(String fileToReturnTrueFor)
        {
            IFileFactoryForPath fileFactory = (f) =>
            {
                var fileToReturn = new FakeFile(f);

                fileToReturn.ExistsReturnValue = f.Equals(fileToReturnTrueFor);

                return fileToReturn;
            };

            return fileFactory;
        }

    }
}
