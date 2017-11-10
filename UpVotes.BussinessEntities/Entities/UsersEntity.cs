namespace UpVotes.BusinessEntities.Entities
{
    using System;

    public partial class UserEntity
    {
        public int UserID { get; set; }

        public string ProfileID { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserEmail { get; set; }

        public string UserMobile { get; set; }

        public int UserType { get; set; }

        public string ProfileURL { get; set; }

        public bool IsActive { get; set; }

        public bool IsBlocked { get; set; }

        public System.DateTime UserActivatedDateTime { get; set; }

        public DateTime? UserLastLoginDateTime { get; set; }

        public string Remarks { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string ProfilePictureURL { get; set; }
    }
}
