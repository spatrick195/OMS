using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OMS_Dev.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using OMS_Dev.Helpers;
using System.Web.Http.Results;
using Microsoft.Ajax.Utilities;
using Stripe;

namespace OMS_Dev.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
        }
    }
}