using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBApiCore.Models
{
    public partial class ContactInputModel
    {
        public ContactInputModel()
        {
            ContactSkillExpertise = new HashSet<ContactSkillExpertiseInputModel>();
        }
        [Required]
        [StringLength(20)]
        public string Firstname { get; set; }
        [Required]
        [StringLength(20)]
        public string Lastname { get; set; }
        [Required]
        [StringLength(40)]
        public string Fullname { get; set; }
        [Required]
        [StringLength(30)]
        public string Address { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(30)]
        public string MobileNum { get; set; }

        public ICollection<ContactSkillExpertiseInputModel> ContactSkillExpertise { get; set; }
    }
}
