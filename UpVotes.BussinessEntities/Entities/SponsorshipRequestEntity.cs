using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpVotes.BusinessEntities.Entities
{
    public class SponsorshipRequestEntity
    {
        public int Provider { get; set; }

        public int CompanyOrSoftwareID { get; set; }
        public string CompanyOrSoftwareName { get; set; }

        public int SponsorshipCategoryID { get; set; }

        public  int CreatedBy { get; set; }

        public string StartDate { get; set; }

        public  string EndDate { get; set; }
    }

    public class SponsorshipExpiredListEntity
    {
        public string Name { get; set; }
        public string Package { get; set; }
        public string Category { get; set; }
    }
}
