using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface IContactUsService
    {
        int SaveContactUsInformation(ContactUsInfoEntity companyEntity);
    }
}
