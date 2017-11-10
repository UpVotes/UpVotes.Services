using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.UnitOfWork;
using UpVotes.DataModel.Utility;

namespace UpVotes.BusinessServices.Service
{
    public class UserService : IUserService
    {
        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }

        private readonly UnitOfWork _unitOfWork;

        public UserService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Authenticate(string userName, string password)
        {
            try
            {
                User user = _unitOfWork.UsersRepository.Get(u => u.UserName == userName && u.UserPassword == password);
                if(user != null && user.UserID > 0)
                {
                    return user.UserID;
                }
                else
                {
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method Authenticate :- PublishMessage. Error=" + ex.Message + ""), ex);
                throw ex;
            }
        }

        public bool BlockUser(int userID)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(int userID)
        {
            throw new NotImplementedException();
        }

        public int InsertUser(UserEntity userEntity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateMemberShip(int userID, int userType)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(int userID, UserEntity userEntity)
        {
            throw new NotImplementedException();
        }
    }
}
