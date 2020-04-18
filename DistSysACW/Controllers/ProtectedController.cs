using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DistSysACW.Controllers
{
    public class ProtectedController : Controller
    {
        [ActionName("Hello")]
        public IActionResult GetHello(string username)
        {
            return new OkObjectResult(new { message = "OK (200)", content = "Hello " + username });
        }

        [ActionName("SHA1")]
        public IActionResult GetSHA1(string s)
        {
            if (s != "")
            {
                byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(s); //string > ascii byte array

                SHA1 sha1Provider = new SHA1CryptoServiceProvider();
                byte[] sha1ByteMessage = sha1Provider.ComputeHash(asciiByteMessage); //ascii > sha1
                string hash = ByteArrayToHexString(sha1ByteMessage); //sha1 > hex string

                return new OkObjectResult(new { message = "OK (200)", content = hash });
            }
            else
            {
                return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "No string submitted" });
            }
        }

        [ActionName("SHA256")]
        public IActionResult GetSHA256(string s)
        {
            if (s != "")
            {
                byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(s); //string > ascii byte array

                SHA256 sha256Provider = new SHA256CryptoServiceProvider();
                byte[] sha1ByteMessage = sha256Provider.ComputeHash(asciiByteMessage); //ascii > sha1
                string hash = ByteArrayToHexString(sha1ByteMessage); //sha1 > hex string

                return new OkObjectResult(new { message = "OK (200)", content = hash });
            }
            else
            {
                return new BadRequestObjectResult(new { message = "BAD REQUEST (400)", content = "No string submitted" });
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
