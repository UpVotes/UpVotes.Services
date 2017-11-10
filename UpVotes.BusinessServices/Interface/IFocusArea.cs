using System.Collections.Generic;

namespace UpVotes.BusinessServices.Interface
{
    public interface IFocusAreaService
    {
        List<BusinessEntities.Entities.FocusAreaEntity> GetFocusAreaList();

        int GetFocusAreaID(string focusAreaName);
    }
}
