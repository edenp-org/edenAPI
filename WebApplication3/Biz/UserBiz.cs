using WebApplication3.Dao;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz
{
    public class UserBiz
    {
        UserDao dao = new UserDao();

        public void Add(User user)
        {
            dao.Add(user);
        }

        public User GetUserByCode(string code)
        {
            return dao.GetUserByCode(code);
        }

        public User GetUserByEmail(string email)
        {
            return dao.GetUserByEmail(email);
        }

        public User GetUserByUnamen(string email)
        {
            return dao.GetUserByUnamen(email);
        }

        public bool UserExists(string email,string uname)
        {
            return dao.UserExists(email, uname);
        }   

    }
}
