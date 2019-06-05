using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpVotes.BusinessEntities.Entities
{
    public class TeamMemebersEntity
    {
        public int MemberId { get; set; }

        public string CompanyOrSoftwareName { get; set; }

        public bool IsService { get; set; }

        public  int? CompanyId { get; set; }

        public int? SoftwareId { get; set; }

        public string MemberName { get; set; }

        public string PictureName { get; set; }

        public string EmailId { get; set; }

        public string Designation { get; set; }

        public string LinkedInProfile { get; set; }

        public string StartDate { get; set; }

        public  string EndDate { get; set; }
    }
}
