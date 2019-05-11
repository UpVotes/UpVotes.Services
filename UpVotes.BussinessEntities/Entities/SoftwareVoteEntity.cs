namespace UpVotes.BusinessEntities.Entities
{
    public class SoftwareVoteEntity
    {
        public int SoftwareVoteID { get; set; }
        public int SoftwareID { get; set; }
        public int UserID { get; set; }
        public System.DateTime VotedDate { get; set; }
    }
}
