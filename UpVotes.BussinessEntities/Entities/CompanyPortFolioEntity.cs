namespace UpVotes.BusinessEntities.Entities
{
    public class CompanyPortFolioEntity
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }

        public int CompanyPortFolioID { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }

        public string Title { get; set; }

        public string VideoURL { get; set; }
        public int CreatedBy { get; set; }
    }
}
