using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpVotes.BusinessEntities.Entities
{
    public class SoftwareFilterEntity
    {
        public string SoftwareName { get; set; }
        public int SoftwareCategoryId { get; set; }        
        public int UserID { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public int Rows { get; set; }
        public int OrderColumn { get; set; }
    }
}
