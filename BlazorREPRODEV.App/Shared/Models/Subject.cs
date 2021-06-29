using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Percentage { get; set; }
        public ICollection<ComponentSubject> ComponentSubjects { get; set; }

    }
}
