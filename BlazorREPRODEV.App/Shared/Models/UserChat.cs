using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class UserChat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ChatMessage { get; set; }
        public DateTime DateCreated { get; set; }
        public UserChat()
        {
            DateCreated = DateTime.Now;
        }
    }
}
