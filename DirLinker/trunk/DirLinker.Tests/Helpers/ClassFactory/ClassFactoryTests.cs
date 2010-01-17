using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using JunctionPointer.Helpers.Interfaces;

namespace DirLinker.Tests.Helpers.ClassFactory
{
    [TestFixture]
    public class ClassFactoryTests
    {
        interface ITestClass { }
        class TestClass : ITestClass { }

        interface IDepend { }
        class Depend : IDepend { }
        interface ITestClassWithDepend { IDepend Depend {get; set;} }
        class TestClassWithDepend : ITestClassWithDepend
        {
            public IDepend Depend {get; set;}

            public TestClassWithDepend(IDepend idepend)
            {
                Depend = idepend;
            }
        }


        [Test]
        public void ManufactureType_ClassNoDependecies_CreatedSuccessfully()
        {

            IClassFactory testClssFactory = new JunctionPointer.Helpers.ClassFactory.ClassFactory();
            testClssFactory.RegisterType<ITestClass, TestClass>();

            ITestClass manufacturedType = testClssFactory.ManufactureType<ITestClass>();


            Assert.IsTrue(manufacturedType is TestClass);
        }

        [Test]
        public void ManufactureType_ClassWithDespendecies_CreatedSuccessfully()
        {
            IClassFactory testClassFactory = new JunctionPointer.Helpers.ClassFactory.ClassFactory();

            testClassFactory.RegisterType<IDepend, Depend>();
            testClassFactory.RegisterType<ITestClassWithDepend, TestClassWithDepend>();

            ITestClassWithDepend manufacturedType = testClassFactory.ManufactureType<ITestClassWithDepend>();

            Assert.IsTrue(manufacturedType is ITestClassWithDepend);
            Assert.IsTrue(manufacturedType.Depend is Depend);
        }

    }
}
