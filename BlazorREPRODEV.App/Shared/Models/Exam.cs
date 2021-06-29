using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class Exam
    {
        public Guid Id { get; set; }
        public string ExamName { get; set; }
        public List<ExamQuestion> ExamQuestions { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int MinutesDuration { get; set; }
        public DateTime DateCreated { get; set; }
        public int IsLive { get; set; }
        public Exam()
        {
            DateCreated = DateTime.Now;
        }
    }
}
