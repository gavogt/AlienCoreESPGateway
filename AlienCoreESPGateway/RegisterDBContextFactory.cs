using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace AlienCoreESPGateway
{
    public class RegisterDBContextFactory : IDesignTimeDbContextFactory<RegisterDBContext>
    {
        public RegisterDBContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<RegisterDBContext>();
            builder.UseSqlServer(
                $"Server=.\\SQLEXPRESS;Database=AlienCoreESPGateway;Trusted_Connection=True;MultipleActiveResultSets=true"
            );

            return new RegisterDBContext(builder.Options);
        }
    }
}
