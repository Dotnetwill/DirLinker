using System;

namespace JunctionPointer.Helpers.Interfaces
{
    public interface IClassFactory
    {
        void RegisterDelegateFactoryForType(Type type, Type factoryType);
        ITypeOptions RegisterType<TContract, TImplementation>();
        T ManufactureType<T>();

    }

    public interface ITypeOptions
    {
        ITypeOptions WithFactory<T>();
    }
}
