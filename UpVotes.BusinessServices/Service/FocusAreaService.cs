using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.UnitOfWork;
using UpVotes.DataModel.Utility;

namespace UpVotes.BusinessServices.Service
{
    public class FocusAreaService : IFocusAreaService
    {
        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }
        
        private UpVotesEntities _context = null;

        public List<FocusAreaEntity> GetFocusAreaList()
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    IEnumerable<Sp_GetFocusArea_Result> focusAreaResult = _context.Database.SqlQuery(typeof(Sp_GetFocusArea_Result), "EXEC Sp_GetFocusArea").Cast<Sp_GetFocusArea_Result>().AsEnumerable();
                    Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetFocusArea_Result, FocusAreaEntity>(); });
                    IEnumerable<FocusAreaEntity> iefocusAreaEntity = Mapper.Map<IEnumerable<Sp_GetFocusArea_Result>, IEnumerable<FocusAreaEntity>>(focusAreaResult);

                    List<FocusAreaEntity> focusAreaEntityList = new List<FocusAreaEntity>();

                    foreach (FocusAreaEntity focusAreaObj in iefocusAreaEntity)
                    {
                        List<SubFocusAreaEntity> subFocusAreaList = (from a in _context.SubFocusArea
                                                                     where a.FocusAreaID == focusAreaObj.FocusAreaID
                                                                     select new SubFocusAreaEntity()
                                                                     {
                                                                         SubFocusAreaID = a.SubFocusAreaID,
                                                                         FocusAreaID = a.FocusAreaID,
                                                                         SubFocusAreaName = a.SubFocusAreaName
                                                                     }).ToList();

                        focusAreaObj.SubFocusAreaEntity = subFocusAreaList;

                        focusAreaEntityList.Add(focusAreaObj);
                    }


                    return focusAreaEntityList;
                }
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
        }

        public int GetFocusAreaID(string focusAreaName)
        {
            using (_context = new UpVotesEntities())
            {
                int focusAreaID = _context.FocusAreas.Where(a => a.FocusAreaName.ToUpper().Contains(focusAreaName.ToUpper())).Select(a => a.FocusAreaID).FirstOrDefault();
                return focusAreaID;
            }
        }
    }
}
