using System;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Tamos.AbleOrigin.Configuration;

namespace Tamos.AbleOrigin.UnitTest
{
    public class IocTest
    {
        [Test]
        public void PerformTest()
        {
            var spCont = new SimpleInjector.Container();
            spCont.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            spCont.Register<ICentralConfigProvider, TestConfigProvider>(Lifestyle.Scoped);
            using (AsyncScopedLifestyle.BeginScope(spCont))
            {
                spCont.GetInstance<ICentralConfigProvider>();
            }

            var autofacBd = new Autofac.ContainerBuilder();
            autofacBd.RegisterType<TestConfigProvider>().As<ICentralConfigProvider>();
            var autofac = autofacBd.Build();
            using (var scope = autofac.BeginLifetimeScope())
            {
                autofac.Resolve<ICentralConfigProvider>();    
            }
            
            
            var count = 1000;
            Console.WriteLine($"Start test {count} times: -----------------------");
            
            //----SimpleInjector
            var task = Task.Run(() => TestUtil.RunWatch("SimpleInjector", count, () =>
            {
                using (AsyncScopedLifestyle.BeginScope(spCont))
                {
                    var siIns = spCont.GetInstance<ICentralConfigProvider>();
                }
            }));

            //-------------------------------------------------
            var task1 = Task.Run(() => TestUtil.RunWatch("Autofac", count, () =>
            {
                using (var scope = autofac.BeginLifetimeScope())
                {
                    autofac.Resolve<ICentralConfigProvider>();    
                }
            }));

            //----SimpleInjector
            /*var task = Task.Run(() => TestUtil.RunWatch("Castle", count, () =>
            {
                using (castle.BeginScope())
                {
                    castle.Resolve<ICentralConfigProvider>();
                }
            }));*/

            task.Wait();
            task1.Wait();
        }

        [Test]
        public void CallTest()
        {
            var spCont = new SimpleInjector.Container();
            spCont.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            spCont.Register<ICentralConfigProvider, TestConfigProvider>(Lifestyle.Scoped);

            AsyncScopedLifestyle.BeginScope(spCont);
            
            spCont.GetInstance<ICentralConfigProvider>();

            Lifestyle.Scoped.GetCurrentScope(spCont)?.Dispose();
            Console.WriteLine("Dispose scope.");
            
            Console.WriteLine("CallTest End--------------------");
        }
    }

    public class TestConfigProvider : ICentralConfigProvider, IDisposable
    {
        public bool Set(string key, string value)
        {
            return true;
        }

        public string Get(string key)
        {
            return null;
        }

        public string GetAppSetting(string key)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Console.WriteLine($"Thread-{System.Threading.Thread.CurrentThread.ManagedThreadId} TestConfigProvider dispose.");
        }
    }
}