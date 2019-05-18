using System.Collections.Generic;
using System.Web;

namespace UpVotes.BusinessEntities.Entities
{
    public class CategoryLinksEntity
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Category { get; set; }
        public int CountOfListing { get; set; }
        public string cityName { get; set; }
        public string cityURL { get; set; }
        public string cityCategory { get; set; }
        public int cityCountOfListing { get; set; }
        public string countryName { get; set; }
        public string countryURL { get; set; }
        public string countryCategory { get; set; }
        public int countryCountOfListing { get; set; }
        public string stateName { get; set; }
        public string stateURL { get; set; }
        public string stateCategory { get; set; }
        public int stateCountOfListing { get; set; }
    }
}
