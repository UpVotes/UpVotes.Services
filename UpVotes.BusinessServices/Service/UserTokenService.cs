using System;
using System.Configuration;
using System.Linq;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.UnitOfWork;
using UpVotes.DataModel.Utility;

namespace UpVotes.BusinessServices.Service
{
    public class UserTokenService : IUserTokenService
    {
        private readonly UnitOfWork _unitOfWork;

        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }

        public UserTokenService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool DeleteByUserId(int userId)
        {
            try
            {
                _unitOfWork.UserTokensRepository.Delete(x => x.UserID == userId);
                _unitOfWork.Save();

                var isNotDeleted = _unitOfWork.UserTokensRepository.GetMany(x => x.UserID == userId).Any();
                return !isNotDeleted;
            }
            catch(Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method DeleteByUserId :- PublishMessage. Error=" + ex.Message + ""), ex);
                return false;
            }
        }

        public UserTokensEntity GenerateToken(int userId)
        {
            try
            {
                string token = Guid.NewGuid().ToString();
                DateTime issuedOn = DateTime.Now;
                DateTime expiredOn = DateTime.Now.AddSeconds(
                                                  Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
                UserToken tokendomain = new UserToken
                {
                    UserID = userId,
                    AuthToken = token,
                    IssuedOn = issuedOn,
                    ExpiresOn = expiredOn
                };

                _unitOfWork.UserTokensRepository.Add(tokendomain);
                _unitOfWork.Save();

                UserTokensEntity tokenModel = new UserTokensEntity()
                {
                    UserID = userId,
                    IssuedOn = issuedOn,
                    ExpiresOn = expiredOn,
                    AuthToken = token
                };

                return tokenModel;
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method GenerateToken :- PublishMessage. Error=" + ex.Message + ""), ex);
                throw ex;
            }
        }

        public bool Kill(string tokenId)
        {
            try
            {
                _unitOfWork.UserTokensRepository.Delete(x => x.AuthToken == tokenId);
                _unitOfWork.Save();
                var isNotDeleted = _unitOfWork.UserTokensRepository.GetMany(x => x.AuthToken == tokenId).Any();
                if (isNotDeleted) { return false; }
                return true;
            }
            catch(Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method Kill :- PublishMessage. Error=" + ex.Message + ""), ex);
                return false;
            }
        }

        public bool ValidateToken(string tokenId)
        {
            try
            {
                UserToken token = _unitOfWork.UserTokensRepository.Get(t => t.AuthToken == tokenId && t.ExpiresOn > DateTime.Now);
                if (token != null && !(DateTime.Now > token.ExpiresOn))
                {
                    token.ExpiresOn = token.ExpiresOn.AddSeconds(
                                                  Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
                    _unitOfWork.UserTokensRepository.Update(token);
                    _unitOfWork.Save();
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method ValidateToken :- PublishMessage. Error=" + ex.Message + ""), ex);
                throw ex;
            }
        }
    }
}
