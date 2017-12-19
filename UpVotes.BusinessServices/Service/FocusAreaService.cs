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
            using (_context = new UpVotesEntities())
            {
                FocusAreaDetail focusArea = new FocusAreaDetail();

                IEnumerable<Sp_GetFocusArea_Result> focusAreaResult = _context.Database.SqlQuery(typeof(Sp_GetFocusArea_Result), "EXEC Sp_GetFocusArea").Cast<Sp_GetFocusArea_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetFocusArea_Result, FocusAreaEntity>(); });
                IEnumerable<FocusAreaEntity> focusAreaEntity = Mapper.Map<IEnumerable<Sp_GetFocusArea_Result>, IEnumerable<FocusAreaEntity>>(focusAreaResult);
                return focusAreaEntity.ToList();
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
