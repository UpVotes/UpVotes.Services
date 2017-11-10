namespace UpVotes.BusinessEntities.Entities
{
    public class FocusAreaEntity
    {
        public int FocusAreaID { get; set; }

        public string FocusAreaName { get; set; }

        public bool IsActive { get; set; }
    }

    public  class FocusAreaDetail
    {
        public System.Collections.Generic.List<FocusAreaEntity> FocusAreaList { get; set; }
    }
}
