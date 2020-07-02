using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static OMS_Dev.Helpers.Enums;

namespace OMS_Dev.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string ModalSelector { get; set; }
        public string Contents { get; set; }
        public string Colour { get; set; }
        public Country Country { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public virtual Industry Industry { get; set; }
    }
}