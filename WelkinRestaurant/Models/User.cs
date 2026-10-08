using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelkinRestaurant.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }  // "admin" или "waiter"
        public string FullName { get; set; }
    }
}