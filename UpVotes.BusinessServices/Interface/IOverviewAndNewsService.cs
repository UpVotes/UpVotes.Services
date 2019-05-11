using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface IOverviewAndNewsService
    {
        OverviewNewsResponse GetCompanySoftwareNews(OverviewNewsEntity overviewNewsEntity);
        OverviewNewsResponse GetSoftwareCompanyNewsByName(OverviewNewsEntity overviewNewsEntity);
        bool IsNewsExists(string Title, string WebsiteURL);
        int SaveAdminNews(OverviewNewsEntity companyEntity);
        int SaveUserNews(OverviewNewsEntity companyEntity);
    }
}
