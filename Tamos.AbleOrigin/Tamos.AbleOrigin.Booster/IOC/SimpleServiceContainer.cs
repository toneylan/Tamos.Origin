using System;
using System.Collections.Generic;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Tamos.AbleOrigin.IOC;
using Container = SimpleInjector.Container;

namespace Tamos.AbleOrigin.Booster
{
    internal class SimpleServiceContainer : IServiceContainer
    {
        internal readonly Container Container;

        //public bool IsWebApp { get; }
        
        public SimpleServiceContainer()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Set to false. This will be the default in v5.x and going forward.
            Container.Options.ResolveUnregisteredConcreteTypes = false;
        }

        private Lifestyle LifeStyle(LifeStyleType? lifeStyle = null)
        {
            //if (lifeStyle == null) lifeStyle = IsWebApp ? LifeStyleType.PerWebRequest : LifeStyleType.Scoped;
            if (lifeStyle == null) lifeStyle = LifeStyleType.Scoped;
            
            switch (lifeStyle.Value)
            {
                case LifeStyleType.Scoped:
                    return Lifestyle.Scoped;
                case LifeStyleType.Singleton:
                    return Lifestyle.Singleton;
                case LifeStyleType.Transient:
                    return Lifestyle.Transient;
                default:
                    return Lifestyle.Scoped;
            }
        }

        #region Register

        public IServiceContainer Register<TService>() where TService : class
        {
            Container.Register<TService>(LifeStyle());

            return this;
        }

        public IServiceContainer Register<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            Container.Register<TService, TImpl>(LifeStyle());
            return this;
        }

        public IServiceContainer Register<TService, TImpl>(LifeStyleType lifeStyle) where TService : class where TImpl : class, TService
        {
            Container.Register<TService, TImpl>(LifeStyle(lifeStyle));
            return this;
        }
        
        public IServiceContainer Register<TImpl>(IEnumerable<Type> serviceTypes, LifeStyleType? lifeStyle = null)
        {
            var implType = typeof(TImpl);
            foreach (var srvType in serviceTypes)
            {
                Container.Register(srvType, implType, LifeStyle(lifeStyle));
            }
            
            return this;
        }
        
        #endregion
        
        #region Resolve
        
        public bool HasRegistration<TService>()
        {
            return Container.GetRegistration(typeof(TService), false) != null;
        }

        public TService GetInstance<TService>() where TService : class
        {
            return Container.GetInstance<TService>();
        }

        #endregion

        #region Scope dispose

        public IDisposable BeginScope()
        {
            return AsyncScopedLifestyle.BeginScope(Container);
        }

        public void EndScope()
        {
            Lifestyle.Scoped.GetCurrentScope(Container)?.Dispose();
        }
        
        public void Dispose()
        {
            Container.Dispose();
        }

        #endregion
    }
}