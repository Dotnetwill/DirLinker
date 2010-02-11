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

            public TypeOptions(IClassFactory classFactory)
            {
                _currentFactory = classFactory;    
            }
      
            public ITypeOptions WithFactory<T>()
            {
                _currentFactory.RegisterDelegateFactoryForType<TClass, T>();
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

        public void AddFactory(Type contract, Delegate factory)
        {
            _typeFactories.Add(contract, factory);    
        }

        public virtual ITypeOptions RegisterType<TContract, TImplementation>()
        {
            _types[typeof(TContract)] = typeof(TImplementation);

            return new TypeOptions<TContract>(this);
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

        public virtual void RegisterDelegateFactoryForType<TResult, TFactoryDelegateType>()
        {
            MethodInfo delegateInvoker = typeof(TFactoryDelegateType).GetMethod("Invoke");
            ParameterExpression[] factoryParams = GetParamsAsExpressions(delegateInvoker);

            //Build the factory from the template
            MethodInfo mi = typeof(ClassFactory).GetMethod("FactoryTemplate");
            mi = mi.MakeGenericMethod(typeof(TResult));

            Expression call = Expression.Call(mi, new Expression[] {Expression.Constant(this), 
                Expression.NewArrayInit(typeof(Object), factoryParams)} );

            TFactoryDelegateType factory = Expression.Lambda<TFactoryDelegateType>(call, factoryParams).Compile();

            _typeFactories.Add(typeof(TFactoryDelegateType), factory as Delegate);
        }

        private static ParameterExpression[] GetParamsAsExpressions(MethodInfo mi)
        {
            List<ParameterExpression> paramsAsExpression = new List<ParameterExpression>();

            Array.ForEach<ParameterInfo>(mi.GetParameters(),
                p => paramsAsExpression.Add(Expression.Parameter(p.ParameterType, p.Name)));

            return paramsAsExpression.ToArray();
        }

        public static T FactoryTemplate<T>(ClassFactory factory, params Object[] args)
        {
            return factory.ManufactureType<T>(args);
        }
    }
}