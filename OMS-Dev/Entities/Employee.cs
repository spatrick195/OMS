using Microsoft.AspNet.Identity.EntityFramework;
using OMS_Dev.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OMS_Dev.Entities
{
    public class Employee : IdentityUser
    {
        public Business Business { get; set; }
        public ApplicationUser User { get; set; }
    }
}