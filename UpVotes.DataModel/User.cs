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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.CompanyReviews = new HashSet<CompanyReview>();
            this.CompanyReviewThankNotes = new HashSet<CompanyReviewThankNote>();
            this.UserRegistrations = new HashSet<UserRegistration>();
            this.UserTokens = new HashSet<UserToken>();
            this.CompanyVotes = new HashSet<CompanyVote>();
        }
    
        public int UserID { get; set; }
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
        public Nullable<System.DateTime> UserLastLoginDateTime { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string ProfilePictureURL { get; set; }
        public string ProfileID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyReview> CompanyReviews { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyReviewThankNote> CompanyReviewThankNotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRegistration> UserRegistrations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserToken> UserTokens { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyVote> CompanyVotes { get; set; }
    }
}
