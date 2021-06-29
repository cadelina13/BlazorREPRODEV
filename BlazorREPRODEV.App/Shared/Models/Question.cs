using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string MyQuestion { get; set; }
        public string Choice1 { get; set; }
        public string Choice2 { get; set; }
        public string Choice3 { get; set; }
        public string Choice4 { get; set; }
        public string CorrectAnswer { get; set; }
        public DateTime DateCreated { get; set; }
        public int InActive { get; set; }
        public Question()
        {
            DateCreated = DateTime.Now;
        }
    }
}
