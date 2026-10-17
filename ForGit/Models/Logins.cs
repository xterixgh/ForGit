using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ForGit.Models
{
    public class Logins
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password  { get; set; }
        [ForeignKey("User")]
        [Required]
        public int User_Id { get; set; }
        public User User { get; set; }
    }
}
