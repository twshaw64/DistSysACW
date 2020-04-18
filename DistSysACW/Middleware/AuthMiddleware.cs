using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DistSysACW.Models;
using System.Collections.Generic;
using System.Collections;

namespace DistSysACW.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, Models.UserContext dbContext)
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then set the correct roles for the User, using claims

            string apiKey = context.Request.Headers["ApiKey"].FirstOrDefault();

            var currentUser = UserDatabaseAccess.UserCheck_rObj(dbContext, apiKey);
            if (currentUser != null) //if user exists
            {
                var claimList = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, currentUser.UserName),
                    new Claim(ClaimTypes.Role, currentUser.Role.ToString())
                };
                var userId = new ClaimsIdentity(claimList, apiKey);
                context.User.AddIdentity(userId);
            }
            #endregion

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

    }
}
