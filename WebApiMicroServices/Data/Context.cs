using WebApiMicroServices.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace WebApiMicroServices.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Sock> Socks { get; set; }

    }

}
