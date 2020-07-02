using OMS_Dev.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMS_Dev.Entities
{
    public class Business
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Incorporation { get; set; }
        public int EmployeeCount { get; set; }
        public int IndustryId { get; set; }
        public virtual Industry Industry { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}