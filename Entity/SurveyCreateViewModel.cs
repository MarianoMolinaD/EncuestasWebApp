using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class SurveyCreateViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? UniqueLink { get; set; }
        public int UserId { get; set; }
        public List<FieldViewModel> Fields { get; set; } = new();
    }

    public class FieldViewModel
    {
        public int? SurveyId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string FieldTitle { get; set; } = string.Empty;
        public string FieldType { get; set; } = "Text";
        public bool IsRequired { get; set; } = true;
    }
}
