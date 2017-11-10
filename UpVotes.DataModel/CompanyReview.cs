//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UpVotes.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class CompanyReview
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompanyReview()
        {
            this.CompanyReviewThankNotes = new HashSet<CompanyReviewThankNote>();
        }
    
        public int CompanyReviewID { get; set; }
        public int CompanyID { get; set; }
        public int FocusAreaID { get; set; }
        public int UserID { get; set; }
        public string ReviewerCompanyName { get; set; }
        public string Designation { get; set; }
        public string ProjectName { get; set; }
        public string FeedBack { get; set; }
        public short Rating { get; set; }
        public System.DateTime ReviewDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    
        public virtual Company Company { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyReviewThankNote> CompanyReviewThankNotes { get; set; }
        public virtual FocusArea FocusArea { get; set; }
        public virtual User User { get; set; }
    }
}
