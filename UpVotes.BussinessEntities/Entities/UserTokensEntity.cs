namespace UpVotes.BusinessEntities.Entities
{
    public class UserTokensEntity
    {
        public int UserTokenID { get; set; }

        public int UserID { get; set; }

        public string AuthToken { get; set; }

        public System.DateTime IssuedOn { get; set; }

        public System.DateTime ExpiresOn { get; set; }
    }
}
