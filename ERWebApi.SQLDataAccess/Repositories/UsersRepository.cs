using ERService.Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ERWebApi.SQLDataAccess.Repositories
{
    public class UsersRepository : GenericRepository<User, ERWebApiDbContext>, IUsersRepository
    {
        public UsersRepository(ERWebApiDbContext context) : base(context)
        {
        }

        public override async Task<User> GetByIdAsync(Guid id)
        {
            return await Context
                .Set<User>()
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetByLoginAsync(string login)
        {
            return await Context
                .Set<User>()
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}
