namespace UpVotes.BusinessEntities.Entities
{
    public class StateEntity
    {
        public int StateID { get; set; }

        public string StateName { get; set; }

        public int CountryID { get; set; }

        public bool IsActive { get; set; }
    }
}
