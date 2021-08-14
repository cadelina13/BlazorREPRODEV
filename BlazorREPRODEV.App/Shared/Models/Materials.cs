using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class Materials
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public int IsRemoved { get; set; }
        public DateTime DateCreated { get; set; }
        public Materials()
        {
            DateCreated = DateTime.Now;
        }
    }
}
