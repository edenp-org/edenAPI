using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Dao
{
    public class UserDao
    {
        public void Add(User user)
        {
            FreeSqlHelper.Instance.Insert(user).ExecuteAffrows();
        }

        public User GetUserByCode(long code)
        {
            return FreeSqlHelper.Instance.Select<User>().Where(u => u.Code == code).First();
        }

        public User GetUserByEmail(string email)
        {
            return FreeSqlHelper.Instance.Select<User>().Where(u => u.Email == email).First();
        }

        public User GetUserByUname(string username)
        {
            return FreeSqlHelper.Instance.Select<User>().Where(u => u.Username == username)
                .First();
        }

        public bool UserExists(string email, string uname)
        {
            return FreeSqlHelper.Instance.Select<User>().Where(u => u.Email == email || u.Username == uname)
                .Any();
        }

        public User GetMaxCodeUser()
        {
            return FreeSqlHelper.Instance.Select<User>()
                .OrderByDescending(u => Convert.ToInt32(u.Code))
                .First();
        }

        public void AddUserFavoriteTag(UserFavoriteTag userFavorite)
        {
            FreeSqlHelper.Instance.Insert(userFavorite).ExecuteAffrows();
        }

        public List<UserFavoriteTag> GetUserFavoriteTagByUserId(long userId)
        {
            return FreeSqlHelper.Instance.Select<UserFavoriteTag>()
                .Where(u => u.UserCode == userId)
                .ToList();
        }
        public UserFavoriteTag GetUserFavoriteTagByUserId(long userId,long tagId)
        {
            return FreeSqlHelper.Instance.Select<UserFavoriteTag>()
                .Where(u => u.UserCode == userId&& u.TagCode == tagId)
                .First();
        }

        public void AddUserDislikedTag(UserDislikedTag userDislikedTag)
        {
            FreeSqlHelper.Instance.Insert(userDislikedTag).ExecuteAffrows();
        }

        public List<UserDislikedTag> GetUserDislikedTagByUserId(long userId)
        {
            return FreeSqlHelper.Instance.Select<UserDislikedTag>()
                .Where(u => u.UserCode == userId)
                .ToList();
        }

        public UserDislikedTag GetUserDislikedTagByUserId(long userId,long tagId)
        {
            return FreeSqlHelper.Instance.Select<UserDislikedTag>()
                .Where(u => u.UserCode == userId && u.TagCode == tagId)
                .First();
        }
        public void AddUserLikeWork(UserLikeWork userLikeWork)
        {
            FreeSqlHelper.Instance.Insert(userLikeWork).ExecuteAffrows();
        }

        public void DeleteUsersLikeTag(long userId, long tagId)
        {
            var cont = FreeSqlHelper.Instance.Delete<UserFavoriteTag>()
                .Where(i => i.UserCode == userId && i.TagCode == tagId)
                .ExecuteAffrows();
        }
    }
}
