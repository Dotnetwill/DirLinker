using System;
using JunctionPointer.Helpers.Interfaces;
using JunctionPointer.Interfaces;
using System.Collections.Generic;

namespace DirLinker.Tests.Helpers
{
    public class QueueBasedClassFactory : IClassFactory
    {


        public QueueBasedClassFactory()
        {
            IFolderQueue = new Queue<IFolder>();
            IFileQueue = new Queue<IFile>();
        }
        
        public Queue<IFolder> IFolderQueue { get; set; }
    
        public Queue<IFile> IFileQueue { get; set; }
    
        public void RegisterType<TContract, TImplementation>()
        {
            throw new NotImplementedException();
        }

        public T ManufactureType<T>()
        {
            if (typeof(T).Equals(typeof(IFolder)))
            {
                return (T)IFolderQueue.Dequeue();
            }
            if (typeof(T).Equals(typeof(IFile)))
            {
                return (T)IFileQueue.Dequeue();
            }

            else
            {
                return default(T);
            }

        }

        #region IClassFactory Members

        public void RegisterDelegateFactoryForType<T>(Type factoryType)
        {
            throw new NotImplementedException();
        }

        ITypeOptions IClassFactory.RegisterType<TContract, TImplementation>()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IClassFactory Members

        public void RegisterDelegateFactoryForType<TResult, TFactoryDelegateType>()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IClassFactory Members


        public T ManufactureType<T>(params object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
