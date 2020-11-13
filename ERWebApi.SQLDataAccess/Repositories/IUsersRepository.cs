using ERService.Business;
using System;
using System.Threading.Tasks;

namespace ERWebApi.SQLDataAccess.Repositories
{
    public interface IUsersRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByLoginAsync(string login);
    }
}