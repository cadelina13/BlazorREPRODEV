using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class ExamStudentRecord
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ExamRecordId { get; set; }
        public int IsCorrect { get; set; }
    }
}
