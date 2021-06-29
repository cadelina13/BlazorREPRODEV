using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.ViewModels
{
    public class UploadResult
    {
        public bool Uploaded { get; set; }
        public string OrigFileName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string StrSize { get; set; }
        public int RowCount { get; set; }
        public int ErrorCode { get; set; }
    }
}
