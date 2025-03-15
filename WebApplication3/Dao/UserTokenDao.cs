using WebApplication3.Foundation.Helper;

namespace WebApplication3.Models.DB
{
    public class UserTokenDao
    {
        public void Add(UserToken userToken)
        {
            FreeSqlHelper.Instance.Insert(userToken).ExecuteAffrows();
        }

        public List<UserToken> GetTokenByUserAndPurpose(string user, string Purpose)
        {
            return FreeSqlHelper.Instance
                 .Select<UserToken>()
                 .Where(u => u.Username == user && u.Purpose == Purpose && u.Expiration >= DateTime.Now)
                 .ToList();
        }

    }
}
