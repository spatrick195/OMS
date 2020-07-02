using Microsoft.AspNet.Identity.EntityFramework;
using OMS_Dev.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace OMS_Dev.Entities
{
    public class Employee : IdentityUser
    {
        public string OriginalUser { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? RegisteredOn { get; set; }
        public virtual Business Business { get; set; }
        public virtual ApplicationUser User { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Employee> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}