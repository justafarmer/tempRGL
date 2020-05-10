using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rungreenlake.web.Areas.Identity.Data;
using rungreenlake.Models;

namespace rungreenlake.web.Areas.Identity.Data
{
    public class Context : IdentityDbContext<RungreenlakeUser>
    {
        public Context()
        {
        }
        
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
