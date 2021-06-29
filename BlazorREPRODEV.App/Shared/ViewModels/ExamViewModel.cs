using BlazorREPRODEV.App.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.ViewModels
{
    public class ExamViewModel
    {
        public Guid ExamId { get; set; }
        public List<Question> ListOfQuestions { get; set; }

    }
}
