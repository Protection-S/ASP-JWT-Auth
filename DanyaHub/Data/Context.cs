using DanyaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DanyaHub.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FileModel> Files { get; set; }

    }

}
