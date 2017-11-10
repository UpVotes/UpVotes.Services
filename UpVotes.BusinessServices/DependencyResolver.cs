using System.ComponentModel.Composition;
using UpVotes.BusinessServices.Interface;
using UpVotes.BusinessServices.Service;
using UpVotes.Resolver;

namespace UpVotes.BusinessServices
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<ICompanyService, CompanyService>();
            registerComponent.RegisterType<IUserService, UserService>();
            registerComponent.RegisterType<IUserTokenService, UserTokenService>();
            registerComponent.RegisterType<IFocusAreaService, FocusAreaService>();
            registerComponent.RegisterType<ICompanyReviewsService, CompanyReviewsService>();
        }
    }
}
