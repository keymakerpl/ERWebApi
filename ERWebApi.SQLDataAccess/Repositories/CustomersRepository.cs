using ERService.Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ERWebApi.SQLDataAccess.Repositories
{
    public class CustomersRepository : GenericRepository<Customer, ERWebApiDbContext>, ICustomersRepository
    {
        public CustomersRepository(ERWebApiDbContext context) : base(context)
        {
            
        }

        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException();

            return await Context
                .Set<Customer>()
                .Include(a => a.CustomerAddresses)
                .SingleAsync(c => c.Id == id);
        }
    }
}
