using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienCoreESPGateway
{
    public class RegisterDatabaseService
    {
        private readonly IDbContextFactory<RegisterDBContext> _dbContextFactory;

        public RegisterDatabaseService(IDbContextFactory<RegisterDBContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<bool> RegisterUserAsync(RegistrationModel reg)
        {
            using var db = _dbContextFactory.CreateDbContext();

            try
            {
                db.Users.Add(new User(reg.Email, reg.Password, reg.FirstName, reg.LastName));

                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register user", ex);
            }
        }
    }
}
