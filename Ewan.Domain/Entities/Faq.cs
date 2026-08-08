using System;
using System.Collections.Generic;
using System.Text;
using Ewan.Domain.Common;
using Ewan.Domain.Enums;

namespace Ewan.Domain.Entities
{
    public class Faq : BaseEntity
    {
        public Sector Sector { get; set; }
        public string QuestionAr { get; set; } = string.Empty;
        public string QuestionEn { get; set; } = string.Empty;
        public string AnswerAr { get; set; } = string.Empty;
        public string AnswerEn { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}