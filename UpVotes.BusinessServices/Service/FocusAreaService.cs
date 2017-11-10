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

        private readonly UnitOfWork _unitOfWork;

        public FocusAreaService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<FocusAreaEntity> GetFocusAreaList()
        {
            FocusAreaDetail focusArea = new FocusAreaDetail();

            IEnumerable<Sp_GetFocusArea_Result> focusAreaResult = _unitOfWork.SpGetFocusArea.ExecWithStoreProcedure("EXEC Sp_GetFocusArea");
            Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetFocusArea_Result, FocusAreaEntity>(); });
            IEnumerable<FocusAreaEntity> focusAreaEntity = Mapper.Map<IEnumerable<Sp_GetFocusArea_Result>, IEnumerable<FocusAreaEntity>>(focusAreaResult);
            return focusAreaEntity.ToList();
        }

        public int GetFocusAreaID(string focusAreaName)
        {
            int focusAreaID = _unitOfWork.FocusAreaRepository.GetAll().Where(i => i.FocusAreaName.ToUpper().Contains(focusAreaName.ToUpper())).Select(i => i.FocusAreaID).FirstOrDefault();
            return focusAreaID;
        }
    }
}
