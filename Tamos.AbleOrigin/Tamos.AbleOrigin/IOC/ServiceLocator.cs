namespace Tamos.AbleOrigin;

public static class ServiceLocator
{
    #region Container Set

    /// <summary>
    /// IOC Container 的解耦抽象
    /// </summary>
    public static IServiceContainer Container { get; private set; }

    public static void UseContainer(IServiceContainer container)
    {
        Container = container;
    }
        
    #endregion

    public static TService GetInstance<TService>() where TService : class
    {
        // ReSharper disable once PossibleNullReferenceException
        return Container.GetInstance<TService>();
    }

    #region Container或反射获取

    /// <summary>
    /// IOC中无法获取对象时，尝试反射获取。
    /// </summary>
    internal static T? GetOrReflect<T>(string typeName) where T : class
    {
        string errMsg = null;
        try
        {
            T srv = null;
            // ReSharper disable once ConstantConditionalAccessQualifier
            if (Container?.HasRegistration<T>() == true) srv = Container.GetInstance<T>();
            if (srv != null) return srv;

            //反射查找（可能在FrameInitializer前发生调用或未调用）
            typeName = $"{Utility.ProdDllBooster}.{typeName}, {Utility.ProdDllBooster}";
            var srvType = Type.GetType(typeName);
            if (srvType != null) return Activator.CreateInstance(srvType) as T;
        }
        catch (Exception e)
        {
            errMsg = e.Message;
        }

        LogService.SafeWrite($"ServiceLocator尝试获取对象：{typeof(T).Name}({typeName})，失败：{errMsg}");
        return null;
    }

    #endregion
    
}