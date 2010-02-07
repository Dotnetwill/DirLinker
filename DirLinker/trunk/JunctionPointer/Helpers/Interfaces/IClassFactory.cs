using System;

namespace JunctionPointer.Helpers.Interfaces
{
    public interface IClassFactory
    {
        void RegisterDelegateFactoryForType<TResult, TFactoryDelegateType>();
        ITypeOptions RegisterType<TContract, TImplementation>();
        T ManufactureType<T>();

    }

    public interface ITypeOptions
    {
        ITypeOptions WithFactory<T>();
    }
}
