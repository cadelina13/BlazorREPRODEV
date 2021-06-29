using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class ExamQuestion
    {
        public Guid Id { get; set; }
        public Question QuestionData { get; set; }
        public DateTime DateAdded { get; set; }
        public ExamQuestion()
        {
            DateAdded = DateTime.Now;
        }

    }
}
