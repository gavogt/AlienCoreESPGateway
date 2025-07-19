using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienCoreESPGateway
{
    internal class RegisterDBContextFactory : IDesignTimeDbContextFactory<RegisterDBContext>
    {
        public RegisterDBContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<RegisterDBContext>();
            builder.UseSqlServer(
              @"Server=.\SQLEXPRESS;Database=AlienCoreESPGateway;Trusted_Connection=True;TrustServerCertificate=True"
            );
            return new RegisterDBContext(builder.Options);
        }
    }
}

