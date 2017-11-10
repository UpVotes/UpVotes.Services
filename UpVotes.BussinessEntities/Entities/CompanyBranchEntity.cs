namespace UpVotes.BusinessEntities.Entities
{
    using System;

    public class CompanyBranchEntity
    {
        public int BranchID { get; set; }

        public string BranchName { get; set; }

        public int CompanyID { get; set; }

        public int CountryID { get; set; }

        public string CountryName { get; set; }

        public int StateID { get; set; }

        public string StateName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public bool IsHeadQuarters { get; set; }

        public bool IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
