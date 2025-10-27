using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class SurveyFillViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public List<SurveyFieldViewModel> Fields { get; set; } = new();
    }

    public class SurveyFieldViewModel
    {
        public int Id { get; set; }
        public string FieldName { get; set; } = "";
        public string FieldTitle { get; set; } = "";
        public string FieldType { get; set; } = "";
        public bool IsRequired { get; set; }
    }

    public class SurveyResponseViewModel
    {
        public int SurveyId { get; set; }
        public List<SurveyAnswerViewModel> Answers { get; set; } = new();
    }

    public class SurveyAnswerViewModel
    {
        public int FieldId { get; set; }
        public string Value { get; set; } = "";
    }
}
