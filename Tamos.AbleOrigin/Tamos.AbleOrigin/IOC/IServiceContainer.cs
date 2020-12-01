using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin.IOC
{
    public interface IServiceContainer : IDisposable
    {
        /*/// <summary>
        /// 是否网站应用
        /// </summary>
        bool IsWebApp { get; }*/

        /*/// <summary>
        /// 默认LifeStyle，缺省Register时使用
        /// </summary>
        LifeStyleType DefaultLifeStyle { get; }*/

        IServiceContainer Register<TService>() where TService : class;

        //IServiceContainer Register(Type type, LifeStyleType lifeStyle, string name);

        IServiceContainer Register<TService, TImpl>() where TService : class where TImpl : class, TService;
        
        IServiceContainer Register<TService, TImpl>(LifeStyleType lifeStyle) where TService : class where TImpl : class, TService;
        
        /// <summary>
        /// 注册实现多个接口的TImpl
        /// </summary>
        IServiceContainer Register<TImpl>(IEnumerable<Type> serviceTypes, LifeStyleType? lifeStyle = null);
        
        /*/// <summary>
        /// Update the implement of service
        /// </summary>
        IServiceContainer UpdateImpl<TService, TImpl>(LifeStyleType lifeStyle) where TService : class where TImpl : TService;

        IServiceContainer UpdateImpl<TImpl>(IEnumerable<Type> serviceTypes, LifeStyleType lifeStyle);*/

        bool HasRegistration<TService>();

        /// <summary>
        /// Get an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        TService GetInstance<TService>() where TService : class;

        /// <summary>
        /// 开启新的Scope，Dispose时其中的Service会一同释放。
        /// </summary>
        IDisposable BeginScope();

        /// <summary>
        /// 结束当前所处的Scope。一般用在需要与BeginScope分开调用的场景。
        /// </summary>
        void EndScope();
    }
}