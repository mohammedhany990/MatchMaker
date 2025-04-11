using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchMaker.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Infrastructure.Identity
{
   public class AppIdentityDbContext : IdentityDbContext<AppUser>
   {
       public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
       :base(options)
       {

       }
       public DbSet<Photo> Photos { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    var hasher = new PasswordHasher<AppUser>();
        //    var adminId = Guid.NewGuid().ToString();

        //     builder.Entity<AppUser>().HasData(
        //        new AppUser
        //        {
        //            Id = adminId,
        //            Email = "mohammedhanymaher1163@gmail.com",
        //            UserName = "mohammedhanymaher1163",
        //            NormalizedUserName = "MOHAMMEDHANYMAHER1163",
        //            NormalizedEmail = "MOHAMMEDHANYMAHER1163@GMAIL.COM",
        //            PasswordHash = hasher.HashPassword(null, "Abcd@1234"),
        //        }
        //        );
        //}
    }
}
