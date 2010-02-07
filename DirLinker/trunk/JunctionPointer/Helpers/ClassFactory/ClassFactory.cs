using System;
using System.Collections.Generic;
using JunctionPointer.Helpers.Interfaces;
using System.Reflection;

namespace JunctionPointer.Helpers.ClassFactory
{
    public class NoCurrentFactoryException : Exception
    {
        public NoCurrentFactoryException()
            : base("No class factory is specified")
        { }
    }

    public class ClassFactory : IClassFactory
    {
        public class TypeOptions : ITypeOptions
        {
            private Type _currentType;
            private IClassFactory _currentFactory;

            public TypeOptions(Type type, IClassFactory classFactory)
            {
                _currentType = type;
                _currentFactory = classFactory;    
            }
      
            public ITypeOptions WithFactory<T>()
            {
                _currentFactory.RegisterDelegateFactoryForType(_currentType, typeof(T));
                return this;
            }
        }

        public static IClassFactory CurrentFactory { get; set; }
 
        public static T CreateInstance<T>()
        {
            if (CurrentFactory == null)
            {
                throw new NoCurrentFactoryException();
            }
            return CurrentFactory.ManufactureType<T>();
        }

        private readonly IDictionary<Type, Type> _types = new Dictionary<Type, Type>();
        private readonly IDictionary<Type, Type> _typeFactories = new Dictionary<Type, Type>();

        public virtual ITypeOptions RegisterType<TContract, TImplementation>()
        {
            _types[typeof(TContract)] = typeof(TImplementation);

            return new TypeOptions(typeof(TContract), this);
        }

        public virtual T ManufactureType<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public virtual object Resolve(Type contract)
        {
            if (_types.ContainsKey(contract))
            {
                Type implementation = _types[contract];

                ConstructorInfo constructor = implementation.GetConstructors()[0];

                ParameterInfo[] constructorParameters = constructor.GetParameters();

                if (constructorParameters.Length == 0)
                    return Activator.CreateInstance(implementation);

                List<object> parameters = new List<object>(constructorParameters.Length);

                foreach (ParameterInfo parameterInfo in constructorParameters)
                {
                    parameters.Add(Resolve(parameterInfo.ParameterType));
                }

                return constructor.Invoke(parameters.ToArray());
            }
            throw new ArgumentException("contract is not a known type");
        }

        public void RegisterDelegateFactoryForType(Type type, Type factoryType)
        {
            
        }
    }
}
