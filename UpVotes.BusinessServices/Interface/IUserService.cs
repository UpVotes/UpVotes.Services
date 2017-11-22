using UpVotes.BusinessEntities.Entities;
using UpVotes.DataModel;

namespace UpVotes.BusinessServices.Interface
{
    public interface IUserService
    {
        int Authenticate(string userName, string password);

        UserEntity InsertUser(UserEntity userEntity);

        UserEntity UpdateUser(UserEntity userEntity, User currentUse);

        bool DeleteUser(int userID);

        bool BlockUser(int userID);

        bool UpdateMemberShip(int userID, int userType);

        UserEntity AddOrUpdateUser(UserEntity userObj);
    }
}
