using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class SurveyShowViewModel
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public string Description { get; set; }
        public string UniqueLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Responses { get; set; }
    }
}
