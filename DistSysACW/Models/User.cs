using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysACW.Models
{
    public enum Role { User, Admin };
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 

        [Key]
        public string APIKey { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }

        public User(string APIKey, string UserName, Role Role)
        {
            this.APIKey = APIKey;
            this.UserName = UserName;
            this.Role = Role;
        }        
        #endregion
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion

    public static class UserDatabaseAccess //TODO Make more abstract
    {
        #region Task3 
        //TODO: Make methods which allow us to read from/write to the database 

        //check user /User/New/ GET (APIKey)(return true/false)
        public static bool UserCheck(UserContext context, string username)
        {
            var result = context.Users.Where(u => u.UserName == username).FirstOrDefault(); //Will either be a User or null

            if (result == null)
                return false;
            else
                return true;
        }

        //check user /User/New/ GET (APIKey, UserName)(return string(true/false))
        public static bool UserCheckKN(UserContext context, string apiKey, string username)
        {
            var result = context.Users.Find(apiKey); //Will either be a User or null

            if (result == null) //if APIKey matched
                return false;
            else if(result.UserName == username) //if username matches
                return true;
            else
                return false;
        }

        //check user /User/New/ GET (APIKey)(return User object)
        public static User UserCheck_rObj(UserContext context, string apiKey)
        {
            var rUser = context.Users.Find(apiKey); //Find will return either a User or null
            return rUser;
        }

        #region Unused methods
        ////new user /User/New/ POST {generate GUID for APIKey store in DB}(return User object)       -unused
        //public static User UserAdd_User(UserContext context, string userName) //test            How does the user choose their return type in the request?
        //{
        //    //check if user exists

        //    var apiKey = new Guid().ToString();
        //    Role r;
        //    var count = context.Users.Count();

        //    if (count < 1)//db is empty
        //    {
        //        r = Role.admin;
        //    }
        //    else
        //    {
        //        r = Role.user;
        //    }

        //    var tempUser = new User(userName, apiKey, r);

        //    context.Users.Add(tempUser);

        //    return tempUser;
        //}
        #endregion

        //new user /User/New/ POST {generate GUID for APIKey store in DB}(return APIKey)
        public static string UserAdd(UserContext context, string username)
        {
            if (!UserCheck(context, username)) //if username isn't taken
            {
                string apiKey = Guid.NewGuid().ToString();
                Role r;
                int count = context.Users.Count();

                if (count < 1)//if db is empty
                {
                    r = Role.Admin;
                }
                else
                {
                    r = Role.User;
                }

                User tempUser = new User(apiKey, username, r);

                context.Users.Add(tempUser);
                context.SaveChanges();

                return apiKey;
            }
            else
            {
                return null;
            }
        }

        //change user role /User/changerole/ POST (return null)
        public static bool UserChangeRole(UserContext context, string AdminApiKey, string username, Role role)
        {
            //var adminCheck = context.Users.Find(AdminApiKey);
            //if (adminCheck.Role != Role.admin) //if user is an admin
            //{
            var rChange = context.Users.Where(u => u.UserName == username).FirstOrDefault(); //Will either be a User or null
            if (rChange != null)// && context.Users.Count() > 1) ?
                {
                    rChange.Role = role; //change supplied user's role to supplied role
                    context.SaveChanges();
                    return true;
                }
                return false;
            //}
            //else
            //{
            //    throw new Exception("Supplied API Key does not correspond to an admin account"); //might be unnecessary, if auth works
            //}
        }

        //delete user /User/RemoveUser/ DELETE (return null)
        public static bool UserRemove(UserContext context, string apiKey, string username)
        {
            var result = context.Users.Find(apiKey); //Will either be a User or null

            if (result == null) //if APIKey matched
                return false;
            else if (result.UserName == username) //if username matches
            {
                try
                {
                    context.Users.Remove(result);
                    context.SaveChanges();
                    return true;
                }
                catch { return false; } //delete failed
            }
            else return false; //user does not exist or does not correspond to apikey
        }

        #endregion
    }
}