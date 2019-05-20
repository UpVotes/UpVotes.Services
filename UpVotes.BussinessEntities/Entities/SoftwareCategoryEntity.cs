using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpVotes.BusinessEntities.Entities
{
    public class SoftwareCategoryEntity
    {
        public int SoftwareCategoryId { get; set; }

        public int ServiceCategoryId { get; set; }

        public int SoftwareId { get; set; }

        public  int CreatedBy { get; set; }

        public  DateTime CreatedDate { get; set; }

        public  int ModifiedBy { get; set; }

        public  DateTime ModifiedDate { get; set; }
    }
}
