using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    public class OverviewNewsEntity
    {
        public int CategoryID { get; set; }
        public string SubcategoryID { get; set; }
        public string location { get; set; }
        public int IsCompanySoftware { get; set; }
        public string Title { get; set; }
        public string UrlTitle { get; set; }
        public int CompanySoftwareID { get; set; }
        public string CompanySoftwareName { get; set; }
        public string WebsiteURL { get; set; }
        public string Description { get; set; }
        public string YoutubeURL { get; set; }
        public string ImageName { get; set; }
        public int CreatedBy { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string City { get; set; }
        public int Subcategory { get; set; }
    }

    public class OverviewNewsResponse
    {
        public List<OverviewNewsResponseEntity> OverviewNewsData { get; set; }
    }

    public class OverviewNewsResponseEntity
    {
        public int CompanySoftwareNewsID { get; set; }
        public int? IsCompanySoftware { get; set; }
        public int? CategoryID { get; set; }
        public int? SubCategoryID { get; set; }
        public int? CountryID { get; set; }
        public int? StateID { get; set; }
        public string City { get; set; }
        public int? CompanyOrSoftwareID { get; set; }
        public string WebsiteURL { get; set; }
        public string NewsTitle { get; set; }
        public string NewsTitleUrl { get; set; }
        public string NewsDescription { get; set; }
        public string ImageName { get; set; }
        public string YoutubeURL { get; set; }
        public string CreatedDate { get; set; }
        public string categoryname { get; set; }
        public string subcategoryname { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string SoftwareCompanyName { get; set; }
        public string LogoName { get; set; }
        public int isRelated { get; set; }
    }


}
