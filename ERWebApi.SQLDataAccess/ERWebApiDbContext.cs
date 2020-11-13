using ERService.Business;
using Microsoft.EntityFrameworkCore;

namespace ERWebApi.SQLDataAccess
{
    public class ERWebApiDbContext : DbContext
    {
        public ERWebApiDbContext(DbContextOptions<ERWebApiDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        public DbSet<Hardware> Hardwares { get; set; }

        public DbSet<HwCustomItem> HardwareCustomItems { get; set; }

        public DbSet<CustomItem> CustomItems { get; set; }

        public DbSet<HardwareType> HardwareTypes { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public DbSet<OrderType> OrderTypes { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Numeration> Numeration { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Acl> ACLs { get; set; }

        public DbSet<AclVerb> AclVerbs { get; set; }

        public DbSet<PrintTemplate> PrintTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            base.OnModelCreating(modelBuilder);
        }        
    }
}
