using WebApplication3.Dao;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz
{
    public class UserTokenBiz
    {
        UserTokenDao dao = new UserTokenDao();

        public void Add(UserToken user)
        {
            dao.Add(user);
        }

        public List<UserToken> GetTokenByUserAndPurpose(string user, string Purpose)
        {
            return dao.GetTokenByUserAndPurpose(user, Purpose);
        }

        public bool IsExist(string uname, string Token, string Purpose)
        {
            return dao.IsExist(uname, Token, Purpose);
        }
    }
}
