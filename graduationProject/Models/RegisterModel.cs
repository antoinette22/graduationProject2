﻿using System.ComponentModel.DataAnnotations;

namespace graduationProject.Models
{
    public class RegisterModel
    {

        //[Required,StringLength(100)]
        //  public string FirstName { get;set;}
        //[Required,StringLength(100)]
        //public string LastName { get;set;}
        [Required, StringLength(100)]
        public string FirstName { get; set; }
        public string? LastName { get; set; }=string.Empty;

        [Required, StringLength(50)]
        public string UserName { get; set; }
        [Required, StringLength(128), EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(256)]
        public string Password { get; set; }
        public bool IsInvestor { get; set; }
    }
}
