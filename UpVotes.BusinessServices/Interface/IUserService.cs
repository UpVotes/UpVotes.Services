using UpVotes.BusinessEntities.Entities;
using UpVotes.DataModel;

namespace UpVotes.BusinessServices.Interface
{
    public interface IUserService
    {
        int Authenticate(string userName, string password);

        UserEntity InsertUser(UserEntity userEntity, UpVotesEntities _upvotesContex);

        UserEntity UpdateUser(UserEntity userEntity, User currentUse, UpVotesEntities _upvotesContex);

        bool DeleteUser(int userID);

        bool BlockUser(int userID);

        bool UpdateMemberShip(int userID, int userType);

        UserEntity AddOrUpdateUser(UserEntity userObj);
        UserEntity LoginRegisteredUser(UserEntity userObj);
        UserEntity ForgotPassword(UserEntity userObj);
        UserEntity ChangePassword(ChangePassword userObj);
    }
}
