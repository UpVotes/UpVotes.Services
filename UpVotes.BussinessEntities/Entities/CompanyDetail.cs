using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    public class CompanyDetail
    {
        public List<CompanyEntity> CompanyList { get; set; }

        public int AverageUserRating { get; set; }

        public int TotalNoOfUsers { get; set; }
    }
}
