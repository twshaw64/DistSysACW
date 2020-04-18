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
                        return new OkObjectResult(new { message = "OK (200)", content = "True - User Does Exist! Did you mean to do a POST to create a new user?" });
                    }
                }
                catch
                {
                    return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "Request formatted incorrectly. Submit a Username (string), API Key (int), or both" });
                }
            }
            return new OkObjectResult(new { message = "OK (200)", content = "False - User Does Not Exist! Did you mean to do a POST to create a new user?" });
        }

        //GET user/new/5
        [HttpGet("{APIKey}"), ActionName("New")] //UNNECESSARY?
        public IActionResult GetNewK([FromQuery]int apiKey)
        {
            try
            {
                if (UserDatabaseAccess.UserCheck(_context, apiKey.ToString()))
                {
                    return new OkObjectResult(new { message = "OK (200)", content = "True - User Does Exist! Did you mean to do a POST to create a new user?" });
                }
            }
            catch
            {
                return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "Request formatted incorrectly. Submit a Username (string), API Key (int), or both" });
            }
            return new OkObjectResult(new { message = "OK (200)", content = "False - User Does Not Exist! Did you mean to do a POST to create a new user?" });
        }

        //GET user/new/'key','username'
        [HttpGet, ActionName("New")]
        public IActionResult GetNewKN([FromQuery]string apiKey, string username)
        {
            if (apiKey != "")
            {
                try
                {
                    if (UserDatabaseAccess.UserCheckKN(_context, apiKey, username))
                    {
                        return new OkObjectResult(new { message = "OK (200)", content = "True - User Does Exist! Did you mean to do a POST to create a new user?" });
                    }
                }
                catch
                {
                    return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "Request formatted incorrectly. Submit a Username (string), API Key (int), or both" });
                }
            }
            return new OkObjectResult(new { message = "OK (200)", content = "False - User Does Not Exist! Did you mean to do a POST to create a new user?" });
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
                        return new UnauthorizedObjectResult(new { message = "FORBIDDEN (403)", content = "Oops. This username is already in use. Please try again with a new username." }); //check returns correct status code
                    }
                    else
                    {
                        return new OkObjectResult(new { message = "OK (200)", content = apiKey });
                    }
                }
                catch
                {
                    return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "Oops. Make sure your body contains a string with your username and your Content - Type is Content - Type:application / json" });
                }
            }
            return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "Oops. Make sure your body contains a string with your username and your Content - Type is Content - Type:application / json" });
        }

        //POST user/ChangeRole
        [HttpPost, ActionName("ChangeRole"), Authorize(Roles = "Admin")]
        public IActionResult PostChangeRole([FromQuery]string AdminApikey, [FromBody]string username, [FromBody] string role)
        {
            try
            {
                if (Enum.TryParse(role, out Role r))
                {
                    if (UserDatabaseAccess.UserChangeRole(_context, AdminApikey, username, r))
                    {
                        return new OkObjectResult(new { message = "OK (200)", content = "Done" });
                    }
                    else
                    {
                        return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "NOT DONE: Username does not exist" });
                    }
                }
                else
                {
                    return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "NOT DONE: Role does not exist" });
                }
            }
            catch
            {
                return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "NOT DONE: An error occured" });
            }
        }

        //DELETE user/removeuser
        [HttpDelete, ActionName("RemoveUser")]
        public IActionResult DeleteRemoveUser([FromQuery]string apikey, [FromBody]string username)
        {
            if (UserDatabaseAccess.UserRemove(_context, apikey, username))
            {
                return new OkObjectResult(new { message = "OK (200)", content = "User deleted" });
            }
            else
            {
                return new OkObjectResult(new { message = "OK (200)", content = "User not found" });
            }                        
        }
    }
}
