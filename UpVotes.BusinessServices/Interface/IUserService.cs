using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface IUserService
    {
        int Authenticate(string userName, string password);

        int InsertUser(UserEntity userEntity);

        bool UpdateUser(int userID, UserEntity userEntity);

        bool DeleteUser(int userID);

        bool BlockUser(int userID);

        bool UpdateMemberShip(int userID, int userType);
    }
}
