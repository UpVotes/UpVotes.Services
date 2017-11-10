namespace UpVotes.BusinessEntities.Entities
{
    public class CurrencyEntity
    {
        public int CurrencyID { get; set; }

        public string CurrencyName { get; set; }

        public string CurrencySymbol { get; set; }

        public string CurrencyCode { get; set; }

        public bool IsActive { get; set; }
    }
}
