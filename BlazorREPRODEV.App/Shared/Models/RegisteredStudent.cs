using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class RegisteredStudent
    {
        public Guid Id { get; set; }
        public UserAccount Account { get; set; }
        public int IsAccepted { get; set; }
    }
}
