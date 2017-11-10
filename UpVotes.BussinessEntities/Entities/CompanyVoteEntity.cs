namespace UpVotes.BusinessEntities.Entities
{
    public class CompanyVoteEntity
    {
        public int CompanyVoteID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public System.DateTime VotedDate { get; set; }
    }
}
