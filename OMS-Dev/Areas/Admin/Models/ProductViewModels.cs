using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMS_Dev.Areas.Admin.Models
{
    public class ProductListViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public bool Active { get; set; }
        public string Description { get; set; }

        [Display(Name = "Statement Descriptor")]
        public string StatementDescriptor { get; set; }
    }

    public class ProductCreateViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Statement Descriptor")]
        public string StatementDescriptor { get; set; }

        public DateTime? Created { get; set; }
    }

    public class ProductDetailViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }

        [Display(Name = "Statement Descriptor")]
        public string StatementDescriptor { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }

    public class ProductEditViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Statement Descriptor")]
        public string StatementDescriptor { get; set; }

        public DateTime? Updated { get; set; }
    }
}