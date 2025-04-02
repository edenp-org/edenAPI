using WebApplication3.Dao;
using WebApplication3.Foundation.Helper;
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

        public User GetUserByUname(string email)
        {
            return dao.GetUserByUname(email);
        }

        public bool UserExists(string email,string uname)
        {
            return dao.UserExists(email, uname);
        }
        public User GetMaxCodeUser()
        {
            return dao.GetMaxCodeUser();
        }

        public string GetNewCode() 
        {
            // 查询当前数据库中最大的 Code
            var maxCodeUser = GetMaxCodeUser();
            if (maxCodeUser != null && int.TryParse(maxCodeUser.Code, out int maxCode))
            {
                return (maxCode + 1).ToString(); // 生成新的 Code，格式为8位数字
            }
            else
            {
                return "1"; // 如果没有用户，初始化为00000001
            }

        }
        public void AddUserFavoriteTag(UserFavoriteTag userFavorite)
        {
            dao.AddUserFavoriteTag(userFavorite);
        }

        public List<UserFavoriteTag> GetUserFavoriteTagByUserId(string userId)
        {
            return dao.GetUserFavoriteTagByUserId(userId);
        }

        public void AddUserDislikedTag(UserDislikedTag userDislikedTag)
        {
            dao.AddUserDislikedTag(userDislikedTag);
        }
        public List<UserDislikedTag> GetUserDislikedTagByUserId(string userId)
        {
            return dao.GetUserDislikedTagByUserId(userId);
        }


    }
}
