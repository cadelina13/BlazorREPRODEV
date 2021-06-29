using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Shared.Models
{
    public class UserAccount
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string UserType { get; set; }
        public int InActive { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime? DateLastUpdate { get; set; }
        public UserAccount()
        {
            DateRegistered = DateTime.Now;
        }
    }
}
