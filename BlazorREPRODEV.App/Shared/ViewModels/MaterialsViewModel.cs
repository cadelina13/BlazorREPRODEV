using BlazorREPRODEV.App.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.ViewModels
{
    public class MaterialsViewModel
    {
        public Materials Materials { get; set; }
        public int NumberOfAttachement { get; set; }
        public UserAccount Instructor { get; set; }
    }
}
