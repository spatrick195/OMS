using OMS_Dev.Entities;
using OMS_Dev.Models.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMS_Dev.Models.Home
{
    public class IndexVM
    {
        public List<Employee> Employees { get; set; }
        public List<Plan> Plans { get; set; }

        public IndexVM()
        {
            Employees = new List<Employee>();
            Plans = new List<Plan>();
        }
    }   
}