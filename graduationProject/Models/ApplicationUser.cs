using graduationProject.DTOs.OfferDtos;
using Microsoft.AspNetCore.Identity;
using System;

namespace graduationProject.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PictureUrl { get; set; }
        public bool Status { get; set; } = true;//true=active,false=disactive
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Connection> Connections { get; set; } = new List<Connection>(); // for all users
        public IEnumerable<Chat> Chats { get; set; } = new List<Chat>();
    }
}
