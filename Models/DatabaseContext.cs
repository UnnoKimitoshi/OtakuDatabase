using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otaku_Database.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {
            option.UseSqlite(@"Data Source=AppDB.db");
        }
    }
}
