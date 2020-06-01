using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WEBApiCore.Models
{
    public class InputSkillModel
    {
        
        [Required]
        [StringLength(30)]
        public string SkillName { get; set; }
        
    }
}
