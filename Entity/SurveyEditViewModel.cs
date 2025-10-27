using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class SurveyEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }

        public List<SurveyFieldEditViewModel> Fields { get; set; } = new();
    }

    public class SurveyFieldEditViewModel
    {
        public int Id { get; set; }
        public string FieldName { get; set; } = "";
        public string FieldTitle { get; set; } = "";
        public string FieldType { get; set; } = "";
        public bool IsRequired { get; set; }
    }

}
