using System;
using System.Collections.Generic;

namespace WEBApiCore.Models
{
    public partial class ContactSkillExpertiseModel
    {
        public int ContactSkillId { get; set; }
        public int ContactId { get; set; }
        public int ExpertiseLvlid { get; set; }
        public int SkillId { get; set; }
        //public string ExpertiseLVL { get; set; }
        //public string Skil { get; set; }
        //public Contact Contact { get; set; }
        //public ExpertiseLev ExpertiseLvl { get; set; }
        //public Skills Skill { get; set; }
    }
}
