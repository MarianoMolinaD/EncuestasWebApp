using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;    
        public string PasswordHash { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
