using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class SurveyResultsViewModel
    {
        public string SurveyName { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public List<FieldResultViewModel> Fields { get; set; } = new();
    }

    public class FieldResultViewModel
    {
        public string FielTitle { get; set; } = "";
        public string FieldType { get; set; } = "";
        public List<string> Responses { get; set; } = new();
    }

    public class FieldResultRow
    {
        public int FieldId { get; set; }
        public string FielTitle { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
}
