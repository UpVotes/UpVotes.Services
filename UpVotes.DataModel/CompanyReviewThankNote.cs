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
    
    public partial class CompanyReviewThankNote
    {
        public int CompanyReviewThankNoteID { get; set; }
        public int CompanyReviewID { get; set; }
        public int UserID { get; set; }
        public System.DateTime ThankNoteDate { get; set; }
    
        public virtual CompanyReview CompanyReview { get; set; }
        public virtual User Users { get; set; }
    }
}
