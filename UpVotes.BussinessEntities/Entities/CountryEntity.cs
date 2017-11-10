namespace UpVotes.BusinessEntities.Entities
{
    public class CountryEntity
    {
        public int CountryID { get; set; }

        public string CountryName { get; set; }

        public string CountryCode { get; set; }

        public int CurrencyID { get; set; }

        public bool IsActive { get; set; }
    }
}
