using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WEBApiCore.Models
{
    public partial class SkillModel
    {
        public SkillModel()
        {
            ContactSkillExpertise = new HashSet<ContactSkillExpertiseModel>();
        }
        
        public int SkillId { get; set; }
        [Required]
        [StringLength(30)]
        public string SkillName { get; set; }

        public ICollection<ContactSkillExpertiseModel> ContactSkillExpertise { get; set; }
    }
}
