using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class DatabaseConfig
    {
        public Guid Id { get; set; }
        public int InitialDataLoaded { get; set; }
        public DateTime DateLoaded { get; set; }
        public DatabaseConfig()
        {
            DateLoaded = DateTime.Now;
        }
    }
}
