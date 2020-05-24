using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using OMS_Dev.Entities;

namespace OMS_Dev.Models
{
    public class EmployeeViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class EmployeeListViewModel
    {
        public List<EmployeeViewModel> UsersToRegister { get; set; }
    }
}