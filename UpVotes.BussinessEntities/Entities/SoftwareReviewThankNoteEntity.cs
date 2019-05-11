using System;

namespace UpVotes.BusinessEntities.Entities
{
    public class SoftwareReviewThankNoteEntity
    {
        public int SoftwareID { get; set; }

        public int SoftwareReviewID { get; set; }

        public int SoftwareReviewThankNoteID { get; set; }

        public int UserID { get; set; }

        public string ThankNotedFullName { get; set; }

        public DateTime ThankNoteDate { get; set; }
    }
}
