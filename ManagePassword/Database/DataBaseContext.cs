using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagePassword.Database.Entity;
using Microsoft.EntityFrameworkCore;
namespace ManagePassword.Database
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext()
        {
        }

      protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options.UseSqlite("Data Source= manage.db");
    }

        public DbSet<General>Generals {get; set;}
    }
}