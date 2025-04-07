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
            // 调用数据访问对象的方法添加用户
            dao.Add(user);
        }

        /// <summary>
        /// 根据用户代码获取用户
        /// </summary>
        /// <param name="code">用户代码</param>
        /// <returns>用户对象</returns>
        public User GetUserByCode(long code)
        {
            // 调用数据访问对象的方法根据用户代码获取用户
            return dao.GetUserByCode(code);
        }

        /// <summary>
        /// 根据用户邮箱获取用户
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <returns>用户对象</returns>
        public User GetUserByEmail(string email)
        {
            // 调用数据访问对象的方法根据用户邮箱获取用户
            return dao.GetUserByEmail(email);
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="email">用户名</param>
        /// <returns>用户对象</returns>
        public User GetUserByUname(string email)
        {
            // 调用数据访问对象的方法根据用户名获取用户
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
            // 调用数据访问对象的方法检查用户是否存在
            return dao.UserExists(email, uname);
        }

        /// <summary>
        /// 获取代码最大的用户
        /// </summary>
        /// <returns>用户对象</returns>
        public User GetMaxCodeUser()
        {
            // 调用数据访问对象的方法获取代码最大的用户
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
                // 返回新的用户代码
                return (maxCodeUser.Code + 1);
            }
            else
            {
                // 如果没有用户，返回初始代码 1
                return 1;
            }
        }

        /// <summary>
        /// 添加用户喜欢的标签
        /// </summary>
        /// <param name="userFavorite">用户喜欢的标签对象</param>
        public void AddUserFavoriteTag(UserFavoriteTag userFavorite)
        {
            // 调用数据访问对象的方法添加用户喜欢的标签
            dao.AddUserFavoriteTag(userFavorite);
        }

        /// <summary>
        /// 根据用户ID获取用户喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户喜欢的标签列表</returns>
        public List<UserFavoriteTag> GetUserFavoriteTagByUserId(long userId)
        {
            // 调用数据访问对象的方法根据用户ID获取用户喜欢的标签
            return dao.GetUserFavoriteTagByUserId(userId);
        }

        /// <summary>
        /// 根据用户ID和标签ID获取用户喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tagId">标签ID</param>
        /// <returns>用户喜欢的标签对象</returns>
        public UserFavoriteTag GetUserFavoriteTagByUserId(long userId, long tagId)
        {
            // 调用数据访问对象的方法根据用户ID和标签ID获取用户喜欢的标签
            return dao.GetUserFavoriteTagByUserId(userId, tagId);
        }

        /// <summary>
        /// 添加用户不喜欢的标签
        /// </summary>
        /// <param name="userDislikedTag">用户不喜欢的标签对象</param>
        public void AddUserDislikedTag(UserDislikedTag userDislikedTag)
        {
            // 调用数据访问对象的方法添加用户不喜欢的标签
            dao.AddUserDislikedTag(userDislikedTag);
        }

        /// <summary>
        /// 根据用户ID获取用户不喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户不喜欢的标签列表</returns>
        public List<UserDislikedTag> GetUserDislikedTagByUserId(long userId)
        {
            // 调用数据访问对象的方法根据用户ID获取用户不喜欢的标签
            return dao.GetUserDislikedTagByUserId(userId);
        }

        /// <summary>
        /// 根据用户ID和标签ID获取用户不喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tagId">标签ID</param>
        /// <returns>用户不喜欢的标签对象</returns>
        public UserDislikedTag GetUserDislikedTagByUserId(long userId, long tagId)
        {
            // 调用数据访问对象的方法根据用户ID和标签ID获取用户不喜欢的标签
            return dao.GetUserDislikedTagByUserId(userId, tagId);
        }

        /// <summary>
        /// 删除用户不喜欢的标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tagId"></param>
        public void DeleteUserDislikedTag(long userId, long tagId)
        {
            // 调用数据访问对象的方法删除用户不喜欢的标签
            dao.DeleteUserDislikedTag(userId, tagId);
        }

        /// <summary>
        /// 添加用户喜欢的作品
        /// </summary>
        /// <param name="userLikeWork">要添加的对象</param>
        public void AddUserLikeWork(UserLikeWork userLikeWork)
        {
            // 调用数据访问对象的方法添加用户喜欢的作品
            dao.AddUserLikeWork(userLikeWork);
        }

        /// <summary>
        /// 删除用户喜欢的标签
        /// </summary>
        /// <param name="userId">用户code</param>
        /// <param name="tagId">Tagode</param>
        public void DeleteUsersLikeTag(long userId, long tagId)
        {
            // 调用数据访问对象的方法删除用户喜欢的标签
            dao.DeleteUsersLikeTag(userId, tagId);
        }

        /// <summary>
        /// 获取用户喜欢的作品
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="workId">作品ID</param>
        public void GetUserLikeWork(long userId, long workId)
        {
            // 调用数据访问对象的方法获取用户喜欢的作品
            dao.GetUserLikeWork(userId, workId);
        }
    }
}