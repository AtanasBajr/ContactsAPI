using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WEBApiCore.Models
{
    public class ExpertiseLvLInputModel
    {
       
        [Required]
        [StringLength(30)]
        public string ExpertiseLevel { get; set; }
    }
}
