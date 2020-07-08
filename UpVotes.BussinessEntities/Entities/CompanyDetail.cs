using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    public class CompanyDetail
    {
        public List<CompanyEntity> CompanyList { get; set; }

        public int AverageUserRating { get; set; }

        public int TotalNoOfUsers { get; set; }

        public List<FocusAreaEntity> FocusAreaList { get; set; }
        public List<ClaimInfoDetail> ClaimList { get; set; }
    }

    public class CategoryMetaTags
    {
        public int CategoryBasedMetaTagsID { get; set; }
        public string FocusAreaName { get; set; }
        public string SubFocusAreaName { get; set; }
        public string Title { get; set; }
        public string TwitterTitle { get; set; }
        public string Descriptions { get; set; }
        public string OgImageURL { get; set; }
    }
}
