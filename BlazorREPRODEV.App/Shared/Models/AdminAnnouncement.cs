using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class AdminAnnouncement
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public AdminAnnouncement()
        {
            DateCreated = DateTime.Now;
        }
    }
}
