using System;
using System.Linq;
using System.Collections.Generic;
using JunctionPointer.Helpers.Interfaces;
using System.Reflection;
using System.Linq.Expressions;

namespace JunctionPointer.Helpers.ClassFactory
{
    public class DelegateFactoryCreator
    {
        IClassFactory _ClassFactory;

        public DelegateFactoryCreator(IClassFactory classFactory)
        {
            _ClassFactory = classFactory;
        }

        public virtual void RegisterDelegateFactoryForType<TResult, TFactoryDelegateType>()
        {
            MethodInfo delegateInvoker = typeof(TFactoryDelegateType).GetMethod("Invoke");
            ParameterExpression[] factoryParams = GetParamsAsExpressions(delegateInvoker);

            //Build the factory from the template
            MethodInfo mi = GetMatchingTemplateMethod(delegateInvoker.GetParameters());
            Type[] concreteTypes = GetTypeListFromMethodInfo(typeof(TResult), delegateInvoker);
            mi = mi.MakeGenericMethod(concreteTypes);

            List<Expression> delegateParams = new List<Expression>();
            delegateParams.Add(Expression.Constant(_ClassFactory));
            delegateParams.AddRange(factoryParams);

            Expression call = Expression.Call(mi, delegateParams.ToArray());

            TFactoryDelegateType factory = Expression.Lambda<TFactoryDelegateType>(call, factoryParams).Compile();

            _ClassFactory.AddFactory(typeof(TFactoryDelegateType), factory as Delegate);
        }

        private ParameterExpression[] GetParamsAsExpressions(MethodInfo mi)
        {
            List<ParameterExpression> paramsAsExpression = new List<ParameterExpression>();

            Array.ForEach<ParameterInfo>(mi.GetParameters(),
                p => paramsAsExpression.Add(Expression.Parameter(p.ParameterType, p.Name)));

            return paramsAsExpression.ToArray();
        }

        private MethodInfo GetMatchingTemplateMethod(ParameterInfo[] delegateParams)
        {
            Int32 paramCount = delegateParams.Count();
            
            //add one to the paramCount to account for the ClassFactory param
            paramCount++;

            return typeof(DelegateFactoryCreator).FindMembers(MemberTypes.Method, BindingFlags.Static | BindingFlags.Public, Type.FilterName, "FactoryTemplate")
                .ToList()
                .Cast<MethodInfo>()
                .First(m => m.GetParameters().Count() == paramCount);
            
        }

        private Type[] GetTypeListFromMethodInfo(Type resultType, MethodInfo delegateInvoker)
        {
            List<Type> types = new List<Type>();
            types.Add(resultType);

            types.AddRange(delegateInvoker.GetParameters()
                .Select(t => t.ParameterType));

            return types.ToArray();
        }

        public static T FactoryTemplate<T>(ClassFactory factory)
        {
            return factory.ManufactureType<T>();
        }

        public static T FactoryTemplate<T>(ClassFactory factory, params Object[] args)
        {
            return factory.ManufactureType<T>(args);
        }

        public static TResult FactoryTemplate<T1, TResult>(ClassFactory factory, T1 param1)
        {
            return factory.ManufactureType<TResult>();
        
        }

        public static TResult FactoryTemplate<T1, T2, TResult>(ClassFactory factory, T1 param1, T2 param2)
        {
            return factory.ManufactureType<TResult>();
        }

        public static TResult FactoryTemplate<T1, T2, T3, TResult>(ClassFactory factory, T1 param1, T2 param2, T3 param3)
        {
            return factory.ManufactureType<TResult>();
        
        }

    }
}
