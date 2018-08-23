using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessEntities.Helper;
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

        public UserEntity UpdateUser(UserEntity userEntity, User currentUser, UpVotesEntities _upvotesContext)
        {
            int currentUserID = currentUser.UserID;
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

            _upvotesContext.SaveChanges();

            currentUser = new User();
            ////currentUser = _unitOfWork.UsersRepository.Get(u => u.ProfileID == userEntity.ProfileID && u.UserType == userEntity.UserType);
            ////currentUser = _upvotesContext.Users.Where(u => u.ProfileID == userEntity.ProfileID && u.UserType == userEntity.UserType).FirstOrDefault();
            //if(userEntity.ProfileID == null)
            //{
            //    currentUser = _upvotesContext.Users.Where(u => u.UserName == userEntity.UserName && u.UserType == 4).FirstOrDefault();
            //}
            //else
            //{
            //    currentUser = _upvotesContext.Users.Where(u => u.ProfileID == "KxFiKBWaUu" && u.UserType == 4).FirstOrDefault();
            //}

            currentUser = _upvotesContext.Users.Where(a => a.UserID == currentUserID).FirstOrDefault();
            
            currentUser.ProfileID = userEntity.ProfileID;
            currentUser.ProfilePictureURL = userEntity.ProfilePictureURL;
            Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
            UserEntity userObj = Mapper.Map<User, UserEntity>(currentUser);

            return userObj;
        }

        public UserEntity InsertUser(UserEntity userEntity, UpVotesEntities _upvotesContext)
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

            _upvotesContext.Users.Add(newUserObj);
            _upvotesContext.SaveChanges();

            User currentUser = _upvotesContext.Users.Where(u => u.ProfileID == userEntity.ProfileID && u.UserType == userEntity.UserType).FirstOrDefault();
            Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
            UserEntity userObj = Mapper.Map<User, UserEntity>(currentUser);

            return userObj;
        }

        public UserEntity AddOrUpdateUser(UserEntity userObj)
        {
            using (UpVotesEntities _upVotesContext = new UpVotesEntities())
            {
                UserEntity userEntityObj = new UserEntity();
                User currentUser = _upVotesContext.Users.Where(u => u.FirstName == userObj.FirstName && u.LastName == userObj.LastName && u.ProfileURL == userObj.ProfileURL && u.UserType == userObj.UserType).FirstOrDefault();
                if (currentUser != null && currentUser.UserID > 0)
                {
                    userEntityObj = UpdateUser(userObj, currentUser, _upVotesContext);
                }
                else
                {
                    userEntityObj = InsertUser(userObj, _upVotesContext);
                }

                return userEntityObj;
            }
        }
        public UserEntity LoginRegisteredUser(UserEntity userObj)
        {
            using (UpVotesEntities _upVotesContext = new UpVotesEntities())
            {
                UserEntity userEntityObj = new UserEntity();
                string EncryptPwd = EncryptionAndDecryption.Encrypt(userObj.UserPassword);
                User currentUser = _upVotesContext.Users.Where(u => u.UserName == userObj.UserName && u.UserPassword == EncryptPwd).FirstOrDefault();
                if (currentUser != null && currentUser.UserID > 0)
                {                    
                    Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
                    userEntityObj = Mapper.Map<User, UserEntity>(currentUser);                    
                }
                return userEntityObj;
            }
        }
        public UserEntity ForgotPassword(UserEntity userObj)
        {
            using (UpVotesEntities _upVotesContext = new UpVotesEntities())
            {
                UserEntity userEntityObj = new UserEntity();
                //string EncryptPwd = EncryptionAndDecryption.Encrypt(userObj.UserPassword);
                User currentUser = _upVotesContext.Users.Where(u => u.UserName == userObj.UserName).FirstOrDefault();
                if (currentUser != null)
                {
                    string pwdgen = EncryptionAndDecryption.GenRandomAlphaNum(8);
                    currentUser.UserPassword = EncryptionAndDecryption.Encrypt(pwdgen);
                    _upVotesContext.SaveChanges();                    
                    Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
                    userEntityObj = Mapper.Map<User, UserEntity>(currentUser);
                    Thread thread = new Thread(() => SendForgotPasswordEmail(currentUser.UserName, pwdgen));
                    thread.Start();
                }
                return userEntityObj;
            }
        }
        public UserEntity ChangePassword(ChangePassword changeObj)
        {
            using (UpVotesEntities _upVotesContext = new UpVotesEntities())
            {
                UserEntity userEntityObj = new UserEntity();
                //string EncryptPwd = EncryptionAndDecryption.Encrypt(userObj.UserPassword);
                User currentUser = _upVotesContext.Users.Where(u => u.UserID == changeObj.UserID).FirstOrDefault();
                if (currentUser != null && (changeObj.Password == EncryptionAndDecryption.Decrypt(currentUser.UserPassword)))
                {                    
                    currentUser.UserPassword = EncryptionAndDecryption.Encrypt(changeObj.NewPassword);
                    _upVotesContext.SaveChanges();                    
                    Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
                    userEntityObj = Mapper.Map<User, UserEntity>(currentUser);                    
                }
                return userEntityObj;
            }
        }

        private void SendForgotPasswordEmail(string workEmail, string pwd)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = workEmail;
            emailProperties.EmailSubject = "Reset Password Successful";
            emailProperties.EmailBody = ForgetEmailContent(workEmail, pwd);
            EmailHelper.SendEmail(emailProperties);
        }
        private string ForgetEmailContent(string workEmail, string pwd)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p><p>As you requested password has been reset successfully</p>");
             sb.Append("<p> Below is your new password</p>");
            sb.Append("<p> Password:-" + pwd + "</p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }
    }
}
