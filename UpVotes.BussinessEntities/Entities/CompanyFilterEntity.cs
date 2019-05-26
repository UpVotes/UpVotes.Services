using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpVotes.BusinessEntities.Entities
{
    public class CompanyFilterEntity
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }

        public decimal MinRate { get; set; }

        public decimal MaxRate { get; set; }

        public int MinEmployee { get; set; }

        public int MaxEmployee { get; set; }

        public string SortBy { get; set; }

        public int FocusAreaID { get; set; }

        public string Location { get; set; }

        public string SubFocusArea { get; set; }

        public int UserID { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }
        public int Rows { get; set; }
    }
}
