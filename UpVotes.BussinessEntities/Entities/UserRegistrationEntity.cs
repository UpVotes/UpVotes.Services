namespace UpVotes.BusinessEntities.Entities
{
    using System;

    public class UserRegistrationEntity
    {
        public int UserRegistrationID { get; set; }

        public int UserID { get; set; }

        public int UserTypeID { get; set; }

        public DateTime UserRegistrationDate { get; set; }

        public DateTime UserValidFrom { get; set; }

        public DateTime? UserValidTo { get; set; }

        public bool IsSilver { get; set; }

        public bool IsGold { get; set; }

        public bool IsPlatinum { get; set; }

        public string Remarks { get; set; }
    }
}
