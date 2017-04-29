using System;
using FileManager.BL.Interfaces.Unity;
using Microsoft.Practices.Unity;

namespace FileManager.BL.Unity
{
    internal sealed class Factory<TOut> : IFactory<TOut>
    {
        private readonly IUnityContainer _container;
        private readonly CompositeResolverOverride _resolverOverrides;

        public Factory(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            _resolverOverrides = new CompositeResolverOverride();
            _container = container;
        }

        public TOut Create()
        {
            return Create(string.Empty);
        }

        public TOut Create(string name)
        {
            return _container.Resolve<TOut>(name, _resolverOverrides);
        }

        public IFactory<TOut> ConstructWith<T>(T firstDependency)
        {
            return And(firstDependency);
        }

        public IFactory<TOut> And<T>(T dependency)
        {
            _resolverOverrides.Add(new DependencyOverride<T>(new InjectionParameter<T>(dependency)));
            return this;
        }
    }
}