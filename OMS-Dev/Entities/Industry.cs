using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OMS_Dev.Entities
{
    public class Industry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Business> Businesses { get; set; }
    }
}