using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysACW.Models
{
    public enum Role { user, admin };
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
        public static bool UserCheck(UserContext context, string input) //test
        {
            var result = context.Find<string>(input); //Find will return a value or null

            if (result == null)
                return false;
            else
                return true;
        }

        //check user /User/New/ GET (APIKey, UserName)(return string(true/false))
        public static bool UserCheckKN(UserContext context, string apiKey, string userName) //test
        {
            var result = context.Users.Find(apiKey, userName); //Find will return either a User or null

            if (result == null)
                return false;
            else
                return true;
        }

        //check user /User/New/ GET (APIKey)(return User object)
        public static User UserCheck_rObj(UserContext context, string apiKey) //test
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
        public static string UserAdd(UserContext context, string username) //test
        {
            if (!UserCheck(context, username)) //if username isn't taken
            {
                var apiKey = new Guid().ToString();
                Role r;
                var count = context.Users.Count();

                if (count < 1)//db is empty
                {
                    r = Role.admin;
                }
                else
                {
                    r = Role.user;
                }

                var tempUser = new User(username, apiKey, r);

                context.Users.Add(tempUser);
                context.SaveChanges();

                return apiKey;
            }
            else
            {
                return null;
            }
        }

        public static bool UserChangeRole(UserContext context, string AdminApiKey, string username, Role role)
        {
            //var adminCheck = context.Users.Find(AdminApiKey);
            //if (adminCheck.Role != Role.admin) //if user is an admin
            //{
                var rChange = context.Users.Find(username);
                if (rChange != null)
                {
                    rChange.Role = role; //change supplied user's role to supplied role
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
        public static bool UserRemove(UserContext context, string apiKey, string username) //test
        {
            var result = context.Users.Find(apiKey, username); //Find will return either a User or null

            if (result != null)
            {
                context.Users.Remove(result);
                context.SaveChanges();
                return true;
            }
            else return false;
        }

        #endregion
    }
}