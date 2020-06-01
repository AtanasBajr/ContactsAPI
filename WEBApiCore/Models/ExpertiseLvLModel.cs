using System;
using System.Collections.Generic;

namespace WEBApiCore.Models
{
    public partial class ExpertiseLvLModel
    {
        public ExpertiseLvLModel()
        {
            ContactSkillExpertise = new HashSet<ContactSkillExpertiseModel>();
        }

        public int ExpertiseLvlid { get; set; }
        public string ExpertiseLevel { get; set; }

        public ICollection<ContactSkillExpertiseModel> ContactSkillExpertise { get; set; }
    }
}
