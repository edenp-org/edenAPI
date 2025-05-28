using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Dao
{
    public class UserDao
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户对象</param>
        public void Add(User user)
        {
            // 使用 FreeSqlHelper 插入用户数据
            FreeSqlHelper.Instance
                .Insert(user)
                .ExecuteAffrows();
        }

        /// <summary>
        /// 根据用户代码获取用户
        /// </summary>
        /// <param name="code">用户代码</param>
        /// <returns>用户对象</returns>
        public User GetUserByCode(long code)
        {
            // 使用 FreeSqlHelper 根据用户代码查询用户
            return FreeSqlHelper.Instance
                .Select<User>()
                .Where(u => u.Code == code)
                .First();
        }

        /// <summary>
        /// 根据用户邮箱获取用户
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <returns>用户对象</returns>
        public User GetUserByEmail(string email)
        {
            // 使用 FreeSqlHelper 根据用户邮箱查询用户
            return FreeSqlHelper.Instance
                .Select<User>()
                .Where(u => u.Email == email)
                .First();
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户对象</returns>
        public User GetUserByUname(string username)
        {
            // 使用 FreeSqlHelper 根据用户名查询用户
            return FreeSqlHelper.Instance
                .Select<User>()
                .Where(u => u.Username == username)
                .First();
        }

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <param name="uname">用户名</param>
        /// <returns>是否存在</returns>
        public bool UserExists(string email, string uname)
        {
            // 使用 FreeSqlHelper 检查用户是否存在
            return FreeSqlHelper.Instance
                .Select<User>()
                .Where(u => u.Email == email || u.Username == uname)
                .Any();
        }

        /// <summary>
        /// 获取代码最大的用户
        /// </summary>
        /// <returns>用户对象</returns>
        public User GetMaxCodeUser()
        {
            // 使用 FreeSqlHelper 获取代码最大的用户
            return FreeSqlHelper.Instance
                .Select<User>()
                .OrderByDescending(u => Convert.ToInt32(u.Code))
                .First();
        }

        /// <summary>
        /// 添加用户喜欢的标签
        /// </summary>
        /// <param name="userFavorite">用户喜欢的标签对象</param>
        public void AddUserFavoriteTag(UserFavoriteTag userFavorite)
        {
            // 使用 FreeSqlHelper 插入用户喜欢的标签
            FreeSqlHelper.Instance
                .Insert(userFavorite)
                .ExecuteAffrows();
        }

        /// <summary>
        /// 根据用户ID获取用户喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户喜欢的标签列表</returns>
        public List<UserFavoriteTag> GetUserFavoriteTagByUserId(long userId)
        {
            // 使用 FreeSqlHelper 根据用户ID查询用户喜欢的标签
            return FreeSqlHelper.Instance
                .Select<UserFavoriteTag>()
                .Where(u => u.UserCode == userId)
                .ToList();
        }

        /// <summary>
        /// 根据用户ID和标签ID获取用户喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tagId">标签ID</param>
        /// <returns>用户喜欢的标签对象</returns>
        public UserFavoriteTag GetUserFavoriteTagByUserId(long userId, long tagId)
        {
            // 使用 FreeSqlHelper 根据用户ID和标签ID查询用户喜欢的标签
            return FreeSqlHelper.Instance
                .Select<UserFavoriteTag>()
                .Where(u => u.UserCode == userId && u.TagCode == tagId)
                .First();
        }

        /// <summary>
        /// 添加用户不喜欢的标签
        /// </summary>
        /// <param name="userDislikedTag">用户不喜欢的标签对象</param>
        public void AddUserDislikedTag(UserDislikedTag userDislikedTag)
        {
            // 使用 FreeSqlHelper 插入用户不喜欢的标签
            FreeSqlHelper.Instance
                .Insert(userDislikedTag)
                .ExecuteAffrows();
        }

        /// <summary>
        /// 根据用户ID获取用户不喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户不喜欢的标签列表</returns>
        public List<UserDislikedTag> GetUserDislikedTagByUserId(long userId)
        {
            // 使用 FreeSqlHelper 根据用户ID查询用户不喜欢的标签
            return FreeSqlHelper.Instance
                .Select<UserDislikedTag>()
                .Where(u => u.UserCode == userId)
                .ToList();
        }

        /// <summary>
        /// 根据用户ID和标签ID获取用户不喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tagId">标签ID</param>
        /// <returns>用户不喜欢的标签对象</returns>
        public UserDislikedTag GetUserDislikedTagByUserId(long userId, long tagId)
        {
            // 使用 FreeSqlHelper 根据用户ID和标签ID查询用户不喜欢的标签
            return FreeSqlHelper.Instance
                .Select<UserDislikedTag>()
                .Where(u => u.UserCode == userId && u.TagCode == tagId)
                .First();
        }

        /// <summary>
        /// 删除用户不喜欢的标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tagId"></param>
        public void DeleteUserDislikedTag(long userId, long tagId)
        {
            // 使用 FreeSqlHelper 删除用户不喜欢的标签
            FreeSqlHelper.Instance
                .Delete<UserDislikedTag>()
                .Where(i => i.UserCode == userId && i.TagCode == tagId)
                .ExecuteAffrows();
        }

        /// <summary>
        /// 添加用户喜欢的作品
        /// </summary>
        /// <param name="userLikeWork">用户喜欢的作品对象</param>
        public void AddUserLikeWork(UserLikeWork userLikeWork)
        {
            // 使用 FreeSqlHelper 插入用户喜欢的作品
            FreeSqlHelper.Instance
                .Insert(userLikeWork)
                .ExecuteAffrows();
        }

        /// <summary>
        /// 删除用户喜欢的标签
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tagId">标签ID</param>
        public void DeleteUsersLikeTag(long userId, long tagId)
        {
            // 使用 FreeSqlHelper 删除用户喜欢的标签
            var cont = FreeSqlHelper.Instance
                .Delete<UserFavoriteTag>()
                .Where(i => i.UserCode == userId && i.TagCode == tagId)
                .ExecuteAffrows();
        }

        /// <summary>
        /// 获取用户喜欢的作品
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="workId">作品ID</param>
        public void GetUserLikeWork(long userId, long workId)
        {
            // 使用 FreeSqlHelper 获取用户喜欢的作品
            var cont = FreeSqlHelper.Instance
                .Delete<UserLikeWork>()
                .Where(i => i.UserCode == userId && i.WorkCode == workId)
                .ExecuteAffrows();
        }

        public void IncreasExamineCount(long userCode,int incrementBy)
        {
            var cont = FreeSqlHelper.Instance
                .Update<User>()
                .Set(u => u.ExamineCount + incrementBy)
                .Set(u => u.LastExamineTime, DateTime.UtcNow)
                .Where(u => u.Code == userCode)
                .ExecuteAffrows();
        }

        public void ResetExamineCount(long userCode)
        {
            FreeSqlHelper.Instance
                .Update<User>()
                .Set(u => u.ExamineCount, 0)
                .Where(u => u.Code == userCode)
                .ExecuteAffrows();
        }

        public List<User> GetUsers(string urername, string email, long ucode)
        {
            var select = FreeSqlHelper.Instance.Select<User>();
            if (!string.IsNullOrEmpty(urername)) select.Where(u => u.Username.Contains(urername));
            if (!string.IsNullOrEmpty(email)) select.Where(u => u.Email == email);
            if (ucode != 0) select.Where(u => u.Code == ucode);
            return select.ToList();
        }

        public void RetrievePassword(long code,string password)
        {
            FreeSqlHelper.Instance.Update<User>().Where(u=> u.Code == code)
                .Set(u => u.Password, password)
                .ExecuteAffrows();
        }
    }
}