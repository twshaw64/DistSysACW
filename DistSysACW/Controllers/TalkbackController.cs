using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistSysACW.Controllers
{
    public class TalkBackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkBackController(Models.UserContext context) : base(context) { }


        [ActionName("Hello")]
        public IActionResult GetHello()
        {
            #region TASK1
            // api/talkback/hello response
            #endregion
            return StatusCode(200,"Hello World");
        }

        [ActionName("Sort")]
        public IActionResult GetSort([FromQuery]int[] integers)
        {
            #region TASK1
            // TODO: 
            // sort the integers into ascending order
            // send the integers back as the api/talkback/sort response

            try
            {
                for (int i = 0; i < integers.Length; i++)
                {
                    for (int j = i + 1; j < integers.Length; j++)
                    {
                        if (integers[i] > integers[j])
                        {
                            int a = integers[i];
                            integers[i] = integers[j];
                            integers[j] = a;
                        }
                    }
                }

                string sortResult = "";
                for (int i = 0; i < integers.Length; i++)
                {
                    sortResult += (integers[i] + " ");
                }

                return StatusCode(200,sortResult);
            }
            catch
            {
                return StatusCode(400, "BAD REQUEST");
            }
            #endregion
        }
    }
}
