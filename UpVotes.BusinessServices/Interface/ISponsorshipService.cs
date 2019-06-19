using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface ISponsorshipService
    {
        bool InsertUpdateSponsorShip(SponsorshipRequestEntity sponsorshipObj);
        List<SponsorshipExpiredListEntity> GetExpiredSponsorshipList(SponsorshipRequestEntity sponsorshipObj);
        bool ScheduleForDeactivateSponsor(SponsorshipRequestEntity sponsorshipObj);
    }
}
