using OMS_Dev.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace OMS_Dev.Extensions
{
    /// <summary>
    /// Extends the functionality of ASP.Net Identity Framework by returning custom user data.
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Find the first name of a user and display it.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>The first name of a user.</returns>
        public static string FirstName(this IIdentity identity)
        {
            var db = ApplicationDbContext.Create();
            var user = db.Users.FirstOrDefault(u => u.UserName.Equals(identity.Name));
            return user != null ? user.FirstName : string.Empty;
        }
    }
}