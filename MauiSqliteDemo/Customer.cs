using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiSqliteDemo
{
    [Table("Customer")]
    public class Customer
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]

        public int Id { get; set; }
        [Column("customer_name")]
        public string CustomerName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("mobile")]
        public string Mobile { get; set; }
    }
}
