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
    
    public partial class CompanyPortFolio
    {
        public int CompanyPortFolioID { get; set; }
        public int CompanyID { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string VideoURL { get; set; }
        public string Description { get; set; }
    
        public virtual Company Company { get; set; }
    }
}
