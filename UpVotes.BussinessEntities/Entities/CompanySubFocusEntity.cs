using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpVotes.BusinessEntities.Entities
{
    public class CompanySubFocusEntity
    {
        public int CompanySubFocusID { get; set; }

        public int CompanyFocusID { get; set; }

        public int SubFocusAreaID { get; set; }

        public double SubFocusAreaPercentage { get; set; }

        public bool IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
