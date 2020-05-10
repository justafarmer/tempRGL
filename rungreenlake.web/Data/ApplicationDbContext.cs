using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rungreenlake.Models;
using rungreenlake.web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace rungreenlake.web.Data
{
    public class ApplicationDbContext : IdentityDbContext<RungreenlakeUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RungreenlakeUser> RunGreenLakeUsers { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<RaceRecord> RaceRecords { get; set; }
        public DbSet<BuddyState> BuddyList { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<User>().HasOne(p => p.Profile).WithOne(u => u.LoginUser).HasForeignKey<Profile>(u => u.UserID).HasPrincipalKey<User>(p => p.UserID);


            //RunGreenLakeUser (to create simple integer profileID without messing with aspnetuser table)
            builder.Entity<RungreenlakeUser>().ToTable("RunGreenLakeUsers");
            builder.Entity<RungreenlakeUser>()
                .HasOne(p => p.UserProfile)
                .WithOne(u => u.LoginUser)
                .HasForeignKey<Profile>(u => u.LinkID)
                .HasPrincipalKey<RungreenlakeUser>(p => p.LinkID);

            //Profile table creation.
            builder.Entity<Profile>().ToTable("Profiles");

            //RaceRecord table creation.
            builder.Entity<RaceRecord>().ToTable("RaceRecords"); 

            //BuddyList table creation.
            builder.Entity<BuddyState>().ToTable("BuddyList");
            builder.Entity<BuddyState>().HasKey(b => new { b.FirstProfileID, b.SecondProfileID });
            builder.Entity<BuddyState>()
                .HasOne(p => p.FirstProfile)
                .WithMany()
                .HasForeignKey(p => p.FirstProfileID)
                ;
            builder.Entity<BuddyState>()
                .HasOne(p => p.SecondProfile)
                .WithMany()
                .HasForeignKey(p => p.SecondProfileID)
                .OnDelete(DeleteBehavior.NoAction)
                ;
            /*
            builder.Entity<BuddyState>()
                .HasOne(p => p.FirstProfile)
                .WithOne()
                .HasForeignKey<BuddyState>(p => p.FirstProfileID)
                ;
            builder.Entity<BuddyState>()
                .HasOne(p => p.SecondProfile)
                .WithOne()
                .HasForeignKey<BuddyState>(p => p.SecondProfileID)
                ;
                */

            //Thread table creation.
            builder.Entity<Thread>().ToTable("Threads");
            builder.Entity<Thread>()
                .HasOne(p => p.InitiatorProfile)
                .WithMany()
                .HasForeignKey(p => p.InitiatorID);

            builder.Entity<Thread>()
                .HasOne(p => p.ReceiverProfile)
                .WithMany()
                .HasForeignKey(p => p.ReceiverID)
                .OnDelete(DeleteBehavior.NoAction)
                ;

            //Message table creation.
            builder.Entity<Message>().ToTable("Messages");

            //Conversation table creation (links messages and threads).
            builder.Entity<Conversation>().ToTable("Conversations");
            builder.Entity<Conversation>().HasKey(b => new { b.ThreadID, b.MessageID });

        }

    }


}
