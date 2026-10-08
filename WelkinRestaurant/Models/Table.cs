using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelkinRestaurant.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Status { get; set; }  // "free" или "opened"
        public int? CurrentWaiterId { get; set; }
        public int? CurrentOrderId { get; set; }
        public string WaiterName { get; set; }  // Для отображения
    }
}