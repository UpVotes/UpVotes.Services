using System;

namespace UpVotes.BusinessEntities.Entities
{
    public class CompanyReviewThankNoteEntity
    {
        public int CompanyID { get; set; }

        public int CompanyReviewID { get; set; }

        public int CompanyReviewThankNoteID { get; set; }

        public int UserID { get; set; }

        public string ThankNotedFullName { get; set; }

        public DateTime ThankNoteDate { get; set; }
    }
}
