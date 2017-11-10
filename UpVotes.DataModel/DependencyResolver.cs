using System.ComponentModel.Composition;
using UpVotes.DataModel.UnitOfWork;
using UpVotes.Resolver;

namespace UpVotes.DataModel
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IUnitOfWork, UnitOfWork.UnitOfWork>();
        }
    }
}
