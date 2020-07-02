using OMS_Dev.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static OMS_Dev.Helpers.Enums;

namespace OMS_Dev.Areas.Admin.Models
{
    public class DocumentViewModels
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Upload)]
        public string Thumbnail { get; set; }

        public string ModalSelector { get; set; }

        [AllowHtml]
        [MaxLength(6)]
        public string Colour { get; set; }

        [AllowHtml]
        public string Contents { get; set; }

        public Country Country { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}