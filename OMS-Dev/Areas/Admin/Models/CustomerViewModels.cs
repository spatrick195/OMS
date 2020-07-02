using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static OMS_Dev.Helpers.Enums;

namespace OMS_Dev.Areas.Admin.Models
{
    public class CustomerViewModels
    {
        public string Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public int AccountBalance { get; set; }

        public bool Delinquent { get; set; }

        public DateTime Created { get; set; }

        public bool? Deleted { get; set; }
    }

    public class CustomerBasicViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }
    }

    public class CustomerCreateViewModel : CustomerBasicViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        [Required]
        public string Address { get; set; }

        [Display(Name = "Address (line 2)")]
        public string Address2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public Province Province { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }
    }

    public class CustomerDeleteViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + LastName;
            }
        }

        [Required]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Display(Name = "Address (line 2)")]
        public string Address2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public Province Province { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }
    }
}