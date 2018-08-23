namespace UpVotes.BusinessEntities.Entities
{
    using System;

    public partial class ChangePassword
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public int UserID { get; set; }
    }
    
}
