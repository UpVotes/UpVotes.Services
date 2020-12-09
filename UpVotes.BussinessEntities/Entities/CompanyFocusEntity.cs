namespace UpVotes.BusinessEntities.Entities
{
    using System;

    public class CompanyFocusEntity
    {
        public int CompanyFocusID { get; set; }

        public int CompanyID { get; set; }

        public int FocusAreaID { get; set; }

        public int SubFocusAreaID { get; set; }

        public string FocusAreaName { get; set; }

        public string SubFocusAreaName { get; set; }

        public double SubFocusAreaPercentage { get; set; }

        public double FocusAreaPercentage { get; set; }

        public bool IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string URLFocusAreaname { get; set; }

        public System.Collections.Generic.List<CompanySubFocusEntity> CompanySubFocus { get; set; }
    }
}
