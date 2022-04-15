using System;
using System.Collections.Generic;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Container = SimpleInjector.Container;

namespace Tamos.AbleOrigin.Booster
{
    internal class SimpleServiceContainer : IServiceContainer
    {
        internal readonly Container Container;

        //private static List<IDisposable>? NeedDisposeItems; // Plan to dispose items on app exit
        
        public SimpleServiceContainer()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            //默认true会在首次GetInstance时，检查所有注册类型的创建，这容易引起错误，也有性能损失。
            Container.Options.EnableAutoVerification = false;
        }

        private static Lifestyle LifeStyleCvt(LifeStyleType lifeStyle = LifeStyleType.Scoped)
        {
            //if (lifeStyle == null) lifeStyle = IsWebApp ? LifeStyleType.PerWebRequest : LifeStyleType.Scoped;

            return lifeStyle switch
            {
                LifeStyleType.Scoped => Lifestyle.Scoped,
                LifeStyleType.Singleton => Lifestyle.Singleton,
                LifeStyleType.Transient => Lifestyle.Transient,
                _ => Lifestyle.Scoped
            };
        }

        #region Register

        public IServiceContainer Register<TService>() where TService : class
        {
            Container.Register<TService>(LifeStyleCvt());

            return this;
        }

        public IServiceContainer Register<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            Container.Register<TService, TImpl>(LifeStyleCvt());
            return this;
        }

        public IServiceContainer Register<TService, TImpl>(LifeStyleType lifeStyle) where TService : class where TImpl : class, TService
        {
            Container.Register<TService, TImpl>(LifeStyleCvt(lifeStyle));
            return this;
        }
        
        public IServiceContainer Register<TImpl>(IEnumerable<Type> serviceTypes, LifeStyleType lifeStyle = LifeStyleType.Scoped)
        {
            var implType = typeof(TImpl);
            foreach (var srvType in serviceTypes)
            {
                Container.Register(srvType, implType, LifeStyleCvt(lifeStyle));
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

        #region Scope and Dispose

        public IDisposable BeginScope()
        {
            return AsyncScopedLifestyle.BeginScope(Container);
        }

        public void EndScope()
        {
            Lifestyle.Scoped.GetCurrentScope(Container)?.Dispose();
        }

        public void RecordInScope(IDisposable instance)
        {
            Lifestyle.Scoped.RegisterForDisposal(Container, instance);
        }

        public void RecordInAppLife(IDisposable instance)
        {
            Container.ContainerScope.RegisterForDisposal(instance);

            /*NeedDisposeItems ??= new List<IDisposable>();
            NeedDisposeItems.Add(instance);*/
        }

        //Booster 调用时有try catch
        public void Dispose()
        {
            // 会在Asp.Net core中，注册跟随应用释放，这里不再单独Dispose
            //Container.Dispose();

            //NeedDisposeItems?.ForEach(x => x.Dispose());
        }

        #endregion
    }
}