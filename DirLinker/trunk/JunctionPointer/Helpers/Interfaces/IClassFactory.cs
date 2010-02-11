using System;

namespace JunctionPointer.Helpers.Interfaces
{
    public interface IClassFactory
    {
        void AddFactory(Type contract, Delegate factory);
        //void RegisterDelegateFactoryForType<TResult, TFactoryDelegateType>();
        ITypeOptions RegisterType<TContract, TImplementation>();
        T ManufactureType<T>();
        T ManufactureType<T>(params Object[] args);

    }

    public interface ITypeOptions
    {
        ITypeOptions WithFactory<T>();
    }
}
