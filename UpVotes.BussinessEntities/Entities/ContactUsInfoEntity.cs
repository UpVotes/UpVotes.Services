using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    public class ContactUsInfoEntity
    {
        public int ContactUsInfoID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string ContactMessage { get; set; }
        public int AddedBy { get; set; }        
    }
}
