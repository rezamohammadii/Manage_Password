using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ManagePassword.Database.Entity
{
    public class General
    {
        [Key]
        public int Id { get; set; }
        public string? Website { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}