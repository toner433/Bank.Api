using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public bool   IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        

    }
}
