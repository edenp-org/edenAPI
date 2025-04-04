using WebApplication3.Dao;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz
{
    /// <summary>
    /// 用户业务逻辑类
    /// </summary>
    public class UserBiz
    {
        // 用户数据访问对象
        UserDao dao = new UserDao();

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户对象</param>
        public void Add(User user)
        {
            dao.Add(user);
        }

        /// <summary>
        /// 根据用户代码获取用户
        /// </summary>
        /// <param name="code">用户代码</param>
        /// <returns>用户对象</returns>
        public User GetUserByCode(long code)
        {
            return dao.GetUserByCode(code);
        }

        /// <summary>
        /// 根据用户邮箱获取用户
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <returns>用户对象</returns>
        public User GetUserByEmail(string email)
        {
            return dao.GetUserByEmail(email);
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="email">用户名</param>
        /// <returns>用户对象</returns>
        public User GetUserByUname(string email)
        {
            return dao.GetUserByUname(email);
        }

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <param name="uname">用户名</param>
        /// <returns>是否存在</returns>
        public bool UserExists(string email, string uname)
        {
            return dao.UserExists(email, uname);
        }

        /// <summary>
        /// 获取代码最大的用户
        /// </summary>
        /// <returns>用户对象</returns>
        public User GetMaxCodeUser()
        {
            return dao.GetMaxCodeUser();
        }

        /// <summary>
        /// 生成新的用户代码
        /// </summary>
        /// <returns>新的用户代码</returns>
        public long GetNewCode()
        {
            // 查询当前数据库中最大的 Code
            var maxCodeUser = GetMaxCodeUser();
            if (maxCodeUser != null && maxCodeUser.Code != 0)
            {
                return (maxCodeUser.Code + 1);
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 添加用户喜欢的标签
        /// </summary>
        /// <param name="userFavorite">用户喜欢的标签对象</param>
        public void AddUserFavoriteTag(UserFavoriteTag userFavorite)
        {
            dao.AddUserFavoriteTag(userFavorite);
        }

        /// <summary>
        /// 根据用户ID获取用户喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户喜欢的标签列表</returns>
        public List<UserFavoriteTag> GetUserFavoriteTagByUserId(long userId)
        {
            return dao.GetUserFavoriteTagByUserId(userId);
        }

        /// <summary>
        /// 添加用户不喜欢的标签
        /// </summary>
        /// <param name="userDislikedTag">用户不喜欢的标签对象</param>
        public void AddUserDislikedTag(UserDislikedTag userDislikedTag)
        {
            dao.AddUserDislikedTag(userDislikedTag);
        }

        /// <summary>
        /// 根据用户ID获取用户不喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户不喜欢的标签列表</returns>
        public List<UserDislikedTag> GetUserDislikedTagByUserId(long userId)
        {
            return dao.GetUserDislikedTagByUserId(userId);
        }
    }
}
