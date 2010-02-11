using System;
using System.Linq;
using System.Collections.Generic;
using JunctionPointer.Helpers.Interfaces;
using System.Reflection;
using System.Linq.Expressions;

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
        public class TypeOptions<TClass> : ITypeOptions 
        {
            private IClassFactory _currentFactory;
            private DelegateFactoryCreator _delegateFactoryCreator;

            public TypeOptions(IClassFactory classFactory, DelegateFactoryCreator factoryCreator)
            {
                _currentFactory = classFactory;
                _delegateFactoryCreator = factoryCreator;
            }
      
            public ITypeOptions WithFactory<T>()
            {
                _delegateFactoryCreator.RegisterDelegateFactoryForType<TClass, T>();
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
        private readonly IDictionary<Type, Delegate> _typeFactories = new Dictionary<Type, Delegate>();

        private DelegateFactoryCreator _FactoryCreator;

        public ClassFactory()
        {
            _FactoryCreator = new DelegateFactoryCreator(this);
        }

        public void AddFactory(Type contract, Delegate factory)
        {
            _typeFactories.Add(contract, factory);    
        }

        public virtual ITypeOptions RegisterType<TContract, TImplementation>()
        {
            _types[typeof(TContract)] = typeof(TImplementation);

            return new TypeOptions<TContract>(this, _FactoryCreator);
        }

        public virtual T ManufactureType<T>()
        {
            return ManufactureType<T>(new Object[] {});
        }
        public virtual T ManufactureType<T>(params Object[] args)
        {
            return (T)Resolve(typeof(T), args);
        }

        public virtual object Resolve(Type contract, params Object[] args)
        {
            
            if (_types.ContainsKey(contract))
            {
                Type implementation = _types[contract];

                ConstructorInfo constructor = implementation.GetConstructors()[0];

                ParameterInfo[] constructorParameters = constructor.GetParameters();

                if (constructorParameters.Length == 0)
                    return Activator.CreateInstance(implementation);

                List<Object> parameters = new List<Object>(constructorParameters.Length);
                List<Object> arguments = new List<Object>(args);

                foreach (ParameterInfo parameterInfo in constructorParameters)
                {
                    parameters.Add(ResolveConstructorArgs(arguments, parameterInfo));
                }

                return constructor.Invoke(parameters.ToArray());
            }
            throw new ArgumentException("contract is not a known type");
        }

        private Object ResolveConstructorArgs(IList<Object> args, ParameterInfo parameterInfo)
        {
            Object param;

            param = args.FirstOrDefault(o => parameterInfo.ParameterType.IsAssignableFrom(o.GetType()));

            if (param != null)
            {
                args.Remove(param);
            }
            else if (_typeFactories.ContainsKey(parameterInfo.ParameterType))
            {
                param = _typeFactories[parameterInfo.ParameterType];
            }
            else
            {
                param = Resolve(parameterInfo.ParameterType);
            }

            return param;
        }
   }
}