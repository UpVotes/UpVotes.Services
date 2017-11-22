using AutoMapper;
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
                if (user != null && user.UserID > 0)
                {
                    return user.UserID;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
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

        public bool UpdateMemberShip(int userID, int userType)
        {
            throw new NotImplementedException();
        }

        public UserEntity UpdateUser(UserEntity userEntity, User currentUser)
        {
            currentUser.UserName = userEntity.UserName;
            currentUser.UserPassword = userEntity.UserPassword;
            currentUser.FirstName = userEntity.FirstName;
            currentUser.LastName = userEntity.LastName;
            currentUser.UserEmail = userEntity.UserEmail;
            currentUser.UserMobile = userEntity.UserMobile;
            currentUser.UserType = userEntity.UserType;
            currentUser.ProfileURL = userEntity.ProfileURL;
            currentUser.UserLastLoginDateTime = DateTime.Now;
            currentUser.Remarks = userEntity.Remarks;
            currentUser.DateOfBirth = userEntity.DateOfBirth;
            currentUser.ProfilePictureURL = userEntity.ProfilePictureURL;
            currentUser.ProfileID = userEntity.ProfileID;

            _unitOfWork.UsersRepository.Update(currentUser);
            _unitOfWork.Save();

            currentUser = new User();
            currentUser = _unitOfWork.UsersRepository.Get(u => u.ProfileID == userEntity.ProfileID && u.UserType == userEntity.UserType);
            Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
            UserEntity userObj = Mapper.Map<User, UserEntity>(currentUser);

            return userObj;
        }

        public UserEntity InsertUser(UserEntity userEntity)
        {
            User newUserObj = new User
            {
                UserName = userEntity.UserName,
                UserPassword = userEntity.UserPassword,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                UserEmail = userEntity.UserEmail,
                UserMobile = userEntity.UserMobile,
                UserType = userEntity.UserType,
                ProfileURL = userEntity.ProfileURL,
                IsActive = true,
                IsBlocked = false,
                UserActivatedDateTime = DateTime.Now,
                UserLastLoginDateTime = DateTime.Now,
                Remarks = userEntity.Remarks,
                DateOfBirth = userEntity.DateOfBirth,
                ProfilePictureURL = userEntity.ProfilePictureURL,
                ProfileID = userEntity.ProfileID,
            };

            _unitOfWork.UsersRepository.Add(newUserObj);
            _unitOfWork.Save();

            User currentUser = new User();
            currentUser = _unitOfWork.UsersRepository.Get(u => u.ProfileID == userEntity.ProfileID && u.UserType == userEntity.UserType);
            Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
            UserEntity userObj = Mapper.Map<User, UserEntity>(currentUser);

            return userObj;
        }

        public UserEntity AddOrUpdateUser(UserEntity userObj)
        {
            UserEntity userEntityObj = new UserEntity();
            User currentUser = _unitOfWork.UsersRepository.Get(u => u.ProfileID == userObj.ProfileID && u.UserType == userObj.UserType);
            if (currentUser != null && currentUser.UserID > 0)
            {
                userEntityObj = UpdateUser(userObj, currentUser);
            }
            else
            {
                userEntityObj = InsertUser(userObj);
            }

            return userEntityObj;
        }
    }
}
