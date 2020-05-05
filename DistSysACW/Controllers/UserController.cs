using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DistSysACW.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DistSysACW.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(Models.UserContext context) : base(context) { }

        //GET user/new/
        [HttpGet("{UserName}"), ActionName("New")]
        public IActionResult GetNewN([FromQuery]string username)
        {
            if (username != "")
            {
                try
                {
                    if (UserDatabaseAccess.UserCheck(_context, username))
                    {
                        return StatusCode(200,"True - User Does Exist! Did you mean to do a POST to create a new user?");
                    }
                }
                catch
                {
                    return StatusCode(400, "Request formatted incorrectly. Submit a Username (string), API Key (int), or both");
                }
            }
            return StatusCode(200, "False - User Does Not Exist! Did you mean to do a POST to create a new user?");
        }

        //GET user/new/5
        [HttpGet("{APIKey}"), ActionName("New")] //UNNECESSARY?
        public IActionResult GetNewK([FromQuery]int apiKey)
        {
            try
            {
                if (UserDatabaseAccess.UserCheck(_context, apiKey.ToString()))
                {
                return StatusCode(200, "True - User Does Exist! Did you mean to do a POST to create a new user?");
                }
            }
            catch
            {
                //return StatusCode(400, "Request formatted incorrectly. Submit a Username (string), API Key (int), or both");  //doesn't fit spec, but is a better response
            }
            return StatusCode(200, "False - User Does Not Exist! Did you mean to do a POST to create a new user?");
        }

        //GET user/new/'key','username'
        [HttpGet, ActionName("New")]
        public IActionResult GetNewKN([FromQuery]string apiKey, [FromQuery]string username)
        {
            if (apiKey != "")
            {
                try
                {
                    if (UserDatabaseAccess.UserCheckKN(_context, apiKey, username))
                    {
                        return StatusCode(200, "True - User Does Exist! Did you mean to do a POST to create a new user?" );
                    }
                }
                catch
                {
                    //return StatusCode(400, "Request formatted incorrectly. Submit a Username (string), API Key (int), or both");  //doesn't fit spec, but is a better response
                }
            }
            return StatusCode(200, "False - User Does Not Exist! Did you mean to do a POST to create a new user?" );
        }

        //POST user/new
        [HttpPost, ActionName("New")]
        public IActionResult PostNew([FromBody]string username)
        {
            if (username != "")
            {
                try
                {
                    var apiKey = UserDatabaseAccess.UserAdd(_context, username);

                    if (apiKey == null)
                    {
                        return StatusCode(403, "Oops. This username is already in use. Please try again with a new username.");
                    }
                    else
                    {
                        return StatusCode(200, apiKey);
                    }
                }
                catch
                {
                    return StatusCode(400, "Oops. Make sure your body contains a string with your username and your Content - Type is Content - Type:application / json");
                }
            }
            return StatusCode(400, "Oops. Make sure your body contains a string with your username and your Content - Type is Content - Type:application / json");
        }

        public class roleParams
        {
            public string UserName { get; set; }
            public string Role { get; set; }
            public roleParams(string username, string role)
            {
                this.UserName = username;
                this.Role = role;
            }
        }

        //POST user/ChangeRole
        [HttpPost, ActionName("ChangeRole"), Authorize(Roles = "Admin")]
        public IActionResult PostChangeRole([FromHeader]string Apikey, [FromBody]roleParams rp)
        {
            try
            {
                if (Enum.TryParse(rp.Role, out Role r))
                {
                    if (UserDatabaseAccess.UserChangeRole(_context, Apikey, rp.UserName, r))
                    {
                        return StatusCode(200, "DONE" );
                    }
                    else
                    {
                        return StatusCode(400, "NOT DONE: Username does not exist");
                    }
                }
                else
                {
                    return StatusCode(400, "NOT DONE: Role does not exist");
                }
            }
            catch
            {
                return StatusCode(400, "NOT DONE: An error occured");
            }
        }

        //DELETE user/removeuser
        [HttpDelete, ActionName("RemoveUser")]
        public IActionResult DeleteRemoveUser([FromHeader]string ApiKey, [FromQuery]string username)
        {
            if (UserDatabaseAccess.UserRemove(_context, ApiKey, username))
            {
                return StatusCode(200, "User deleted");
            }
            else
            {
                return StatusCode(200, "User not found");
            }                        
        }
    }
}
