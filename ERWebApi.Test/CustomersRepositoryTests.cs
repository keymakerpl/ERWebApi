using ERService.Business;
using ERWebApi.SQLDataAccess;
using ERWebApi.SQLDataAccess.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ERWebApi.Test
{
    public class CustomersRepositoryTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public CustomersRepositoryTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task GetCustomers_PageSizeThree_ShouldReturnThreeAutors()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ERWebApiDbContext>()
                .UseInMemoryDatabase($"CourseLibaryForTesting{Guid.NewGuid()}") // izolacja testu, ma mieæ swoj¹ bazê                
                .Options;

            using (var context = new ERWebApiDbContext(options))
            {
                context.Customers.AddRange(new[] {
                    new Customer
                    {
                            Id = Guid.NewGuid(),
                            FirstName = "Jan",
                            LastName = "Popek",
                            PhoneNumber = "123 321 323",
                            PhoneNumber2 = "213 321 222",
                            Email = "weer@op.pl",
                            Email2 = "weer2@op.pl",
                            CompanyName = "Company",
                            NIP = "649 000 333",
                            Description = "Description"                            
                    },
                    new Customer
                    {
                            Id = Guid.NewGuid(),
                            FirstName = "Bob",
                            LastName = "Popek",
                            PhoneNumber = "133 321 323",
                            PhoneNumber2 = "213 321 222",
                            Email = "wr@op.pl",
                            Email2 = "weer2@op.pl",
                            CompanyName = "Company",
                            NIP = "669 500 333",
                            Description = "Description"
                    },
                    new Customer
                    {
                            Id = Guid.NewGuid(),
                            FirstName = "Man",
                            LastName = "Kot",
                            PhoneNumber = "153 321 323",
                            PhoneNumber2 = "263 321 222",
                            Email = "weer23@op.pl",
                            Email2 = "weer244@op.pl",
                            CompanyName = "Company 4",
                            NIP = "649 032 333",
                            Description = "Description 3"
                    },
                    new Customer
                    {
                            Id = Guid.NewGuid(),
                            FirstName = "Janusz",
                            LastName = "Popek",
                            PhoneNumber = "123 421 323",
                            PhoneNumber2 = "213 321 222",
                            Email = "weer43@op.pl",
                            Email2 = "weer232@op.pl",
                            CompanyName = "Company 4",
                            NIP = "649 000 373",
                            Description = "Description 4"
                    },
                    new Customer
                    {
                            Id = Guid.NewGuid(),
                            FirstName = "Tom",
                            LastName = "Kat",
                            PhoneNumber = "139 321 323",
                            PhoneNumber2 = "243 321 222",
                            Email = "weer2@op.pl",
                            Email2 = "weer233@op.pl",
                            CompanyName = "Company",
                            NIP = "649 050 333",
                            Description = "Description 6"
                    }
                });

                context.SaveChanges();
            }

            using (var context = new ERWebApiDbContext(options))
            {
                var repository = new CustomersRepository(context);

                // Act
                var authors = await repository.GetEntitiesAsync(1, 3);

                // Assert
                Assert.Equal(3, authors.Count());
            }
        }

        [Fact]
        public void GetCustomer_EmptyGuid_ShouldThrowArgumentException()
        {
            // Arrange

            // InMemory nie udaje w pe³ni silnika sql, np referencje, je¿eli trzeba to lepiej u¿yæ sqlite
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());

            var options = new DbContextOptionsBuilder<ERWebApiDbContext>()
                //.UseInMemoryDatabase($"CourseLibaryForTesting{Guid.NewGuid()}") // izolacja testu, ma mieæ swoj¹ bazê
                .UseSqlite(connection)
                .Options;

            using (var context = new ERWebApiDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                var repository = new CustomersRepository(context);

                // Assert
                Assert.ThrowsAsync<ArgumentException>(
                    // Act
                    async () => await repository.GetByIdAsync(Guid.Empty)
                    );
            }
        }

        [Fact]
        public void CreateCustomer_Null_ShouldThrowArgumentNullException()
        {
            // Arrange

            // InMemory nie udaje w pe³ni silnika sql, np referencje, je¿eli trzeba to lepiej u¿yæ sqlite
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());

            var options = new DbContextOptionsBuilder<ERWebApiDbContext>()
                //.UseInMemoryDatabase($"CourseLibaryForTesting{Guid.NewGuid()}") // izolacja testu, ma mieæ swoj¹ bazê
                .UseSqlite(connection)
                .Options;

            using (var context = new ERWebApiDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                var repository = new CustomersRepository(context);

                // Assert
                Assert.Throws<ArgumentNullException>(
                    // Act
                    () => repository.Add(null));
            }
        }
    }
}
