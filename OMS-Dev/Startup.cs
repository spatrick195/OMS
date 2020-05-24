using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(OMS_Dev.Startup))]

namespace OMS_Dev
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
            // test code below
            //ApplicationDbContext context = new ApplicationDbContext();
            //var EmployeeManager = new UserManager<Employee>(new UserStore<Employee>(context));
            //var employee = new Employee();
            //EmployeeManager.AddToRole(employee.Id, "Employee");
        }

        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // creating first Admin Role and creating a default Admin User
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin role
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin user who will maintain the website

                var user = new ApplicationUser();
                user.Email = "jacobhallgarth2@gmail.com";
                user.UserName = "jacobhallgarth2@gmail.com";
                string password = "Jacob2713";

                var chkUser = UserManager.Create(user, password);
                //Add default User to Role Admin
                if (chkUser.Succeeded)
                {
                    _ = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            // creating Creating Manager role
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);
            }

            // creating Creating Employee role
            if (!roleManager.RoleExists("Employee"))
            {
                var role = new IdentityRole();
                role.Name = "Employee";
                roleManager.Create(role);
            }
        }
    }
}