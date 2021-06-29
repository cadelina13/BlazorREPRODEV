using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class ExamRecord
    {
        public Guid Id { get; set; }
        public Exam ExamData { get; set; }
        public RegisteredStudent Student { get; set; }
        public DateTime DateTaken { get; set; }
        public DateTime? DateFinished { get; set; }
        public int Score { get; set; }
    }
}
