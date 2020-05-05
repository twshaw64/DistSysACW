using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Authorization;
using DistSysACW.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DistSysACW.Controllers
{
    public class ProtectedController : BaseController
    {
        public ProtectedController(Models.UserContext context) : base(context) { }

        [ActionName("Hello"), Authorize(Roles = "Admin,User")]
        public IActionResult GetHello([FromHeader]string apiKey)
        {
            User currentuser = UserDatabaseAccess.UserCheck_rObj(_context, apiKey);
            return StatusCode(200, "Hello " + currentuser.UserName);
        }

        [ActionName("SHA1"), Authorize(Roles = "Admin,User")]
        public IActionResult GetSHA1([FromHeader]string apiKey, [FromQuery]string message)
        {
            if (message != "")
            {
                byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message); //string > ascii byte array

                SHA1 sha1Provider = new SHA1CryptoServiceProvider();
                byte[] sha1ByteMessage = sha1Provider.ComputeHash(asciiByteMessage); //ascii > sha1
                string hash = ByteArrayToHexString(sha1ByteMessage); //sha1 > hex string

                return StatusCode(200, hash);
            }
            else
            {
                return StatusCode(400, "No string submitted");
            }
        }

        [ActionName("SHA256"), Authorize(Roles = "Admin,User")]
        public IActionResult GetSHA256([FromHeader]string apiKey, [FromQuery]string message)
        {
            if (message != "")
            {
                byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message); //string > ascii byte array

                SHA256 sha256Provider = new SHA256CryptoServiceProvider();
                byte[] sha1ByteMessage = sha256Provider.ComputeHash(asciiByteMessage); //ascii > sha1
                string hash = ByteArrayToHexString(sha1ByteMessage); //sha1 > hex string

                return StatusCode(200, hash);
            }
            else
            {
                return StatusCode(400, "No string submitted");
            }
        }

        static string ByteArrayToHexString(byte[] byteArray)
        {
            string hexString = "";
            if (byteArray != null)
            {
                foreach (byte b in byteArray)
                {
                    hexString += b.ToString("x2");
                }
            }
            return hexString;
        }
    }
}
