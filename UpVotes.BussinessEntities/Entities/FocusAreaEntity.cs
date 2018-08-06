using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    public class FocusAreaEntity
    {
        public int FocusAreaID { get; set; }

        public string FocusAreaName { get; set; }

        public string FocusType { get; set; }

        public bool IsActive { get; set; }

        public List<SubFocusAreaEntity> SubFocusAreaEntity { get; set; }
    }

    public  class FocusAreaDetail
    {
        public List<FocusAreaEntity> FocusAreaList { get; set; }
    }
}
