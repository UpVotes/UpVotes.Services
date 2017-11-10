using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using UpVotes.Resolver;

namespace UpVotes.WebAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            //container.RegisterType<IUnitOfWork, UnitOfWork>();
            //container.RegisterType<ICompanyService, CompanyService>();

            RegisterTypes(container);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            //Component initialization via MEF
            ComponentLoader.LoadContainer(container, ".\\bin", "UpVotes.WebAPI.dll");
            ComponentLoader.LoadContainer(container, ".\\bin", "UpVotes.BusinessServices.dll");
        }
    }
}