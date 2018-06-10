using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpVotes.BusinessEntities.Entities
{
    public class SubFocusAreaEntity
    {
        public int SubFocusAreaID { get; set; }

        public int? FocusAreaID { get; set; }

        public string SubFocusAreaName { get; set; }

        public bool IsActive { get; set; }
    }
}
