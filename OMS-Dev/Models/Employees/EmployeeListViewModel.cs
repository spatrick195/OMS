using OMS_Dev.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMS_Dev.Models.Employees
{
    public class EmployeeListViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }

    public class EditEmployeeViewModel : EmployeeListViewModel
    {
        public string Password { get; set; }
    }
}