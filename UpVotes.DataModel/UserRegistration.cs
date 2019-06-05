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
    
    public partial class UserRegistration
    {
        public int UserRegistrationID { get; set; }
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public System.DateTime UserRegistrationDate { get; set; }
        public System.DateTime UserValidFrom { get; set; }
        public Nullable<System.DateTime> UserValidTo { get; set; }
        public bool IsSilver { get; set; }
        public bool IsGold { get; set; }
        public bool IsPlatinum { get; set; }
        public string Remarks { get; set; }
    
        public virtual UserType UserType { get; set; }
        public virtual User Users { get; set; }
    }
}
