using graduationProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace graduationProject.core.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Post>()
                .HasOne(x => x.User)
                .WithMany(x => x.Posts)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(x => x.Post)
                .WithMany(x => x.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Reply>()
                .HasOne(x => x.Comment)
                .WithMany(x => x.Replies)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<React>()
                .HasOne(x => x.Post)
                .WithMany(x => x.Reacts)
                .OnDelete(DeleteBehavior.Cascade);
            //builder.Entity<Chat>()
            //    .HasOne(p => p.SendUser)
            //    .WithMany(b => b.Chats)
            //    .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<Chat>().
            //    HasOne(p => p.ReceiveUser)
            //    .WithMany(b => b.Chats);
            //builder.Entity<Connection>()
            //    .HasOne(p => p.User2)
            //    .WithMany(b => b.Connections);
            builder.Entity<Chat>().
                HasOne(p => p.ReceiveUser)
                .WithMany(b => b.Chats)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Connection>()
                .HasOne(p => p.User2)
                .WithMany(b => b.Connections)
                .OnDelete(DeleteBehavior.NoAction);


        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<React> Reacts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<offer> Offers { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public virtual DbSet<UserConnection> UserConnections { get; set; }
    }
}
