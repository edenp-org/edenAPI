﻿using WebApplication3.Foundation.Helper;
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

        public User GetUserByCode(string code)
        {
            return FreeSqlHelper.Instance.Select<Models.DB.User>().Where(u => u.Code == code).First();
        }

        public User GetUserByEmail(string email)
        {
            return FreeSqlHelper.Instance.Select<Models.DB.User>().Where(u => u.Email == email).First();
        }
        
        public User GetUserByUname(string username)
        {
            return FreeSqlHelper.Instance.Select<Models.DB.User>().Where(u => u.Username == username).First();
        }

        public bool UserExists(string email, string uname)
        {
            return FreeSqlHelper.Instance.Select<Models.DB.User>().Where(u => u.Email == email || u.Username == uname).Any();
        }
        public User GetMaxCodeUser()
        {
            return FreeSqlHelper.Instance.Select<Models.DB.User>()
                .OrderByDescending(u => Convert.ToInt32(u.Code))
                .First();
        }
    }
}
