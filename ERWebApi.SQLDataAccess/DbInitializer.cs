﻿using ERService.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERWebApi.SQLDataAccess
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ERWebApiDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ERWebApiDbContext>())
                {
                    if (!context.Numeration.Any(n => n.Name == "default"))
                    {
                        context.Numeration.Add(new Numeration() { Name = "default", Pattern = "[MM][RRRR]" });
                    }

                    // ACLe
                    if (!context.AclVerbs.Any())
                    {
                        context.AclVerbs.AddRange(
                        new AclVerb() { Name = "Dostęp do konfiguracji aplikacji", DefaultValue = 0 },
                        new AclVerb() { Name = "Dostęp do konfiguracji wydruków", DefaultValue = 0 },
                        new AclVerb() { Name = "Dostęp do konfiguracji numeracji", DefaultValue = 0 },
                        new AclVerb() { Name = "Zarządzanie użytkownikami", DefaultValue = 0 },

                        new AclVerb() { Name = "Dodawanie nowych napraw", DefaultValue = 0 },
                        new AclVerb() { Name = "Usuwanie napraw", DefaultValue = 0 },
                        new AclVerb() { Name = "Edytowanie napraw", DefaultValue = 0 },

                        new AclVerb() { Name = "Dodawanie nowych klientów", DefaultValue = 0 },
                        new AclVerb() { Name = "Usuwanie klientów", DefaultValue = 0 },
                        new AclVerb() { Name = "Edytowanie klientów", DefaultValue = 0 }
                        );

                        context.SaveChanges();
                    }

                    // Przypisz Acle do roli admina
                    if (!context.Roles.Any(r => r.Name == "Domyślna"))
                    {
                        context.Roles.Add(
                        new Role()
                        {
                            Name = "Domyślna",
                            IsSystem = true
                        });

                        context.SaveChanges();
                    }

                    // Przypisz Verby do Acli
                    var role = context.Roles.FirstOrDefault(r => r.Name == "Domyślna");
                    var roleId = role.Id;
                    var acls = context.ACLs.Where(a => a.RoleId == roleId);
                    foreach (var acl in acls)
                    {
                        context.ACLs.Remove(acl);
                    }
                    context.SaveChanges();

                    foreach (var verb in context.AclVerbs)
                    {
                        context.ACLs.Add(
                            new Acl() { AclVerbId = verb.Id, Value = 1, RoleId = roleId });
                    }

                    context.SaveChanges();

                    // Dodaj admina i przypisz rolę
                    if (!context.Users.Any(u => u.Login == "administrator"))
                    {
                        context.Users.Add(
                        new User()
                        {
                            Login = "administrator",
                            IsActive = true,
                            IsAdmin = true,
                            IsSystem = true,
                            RoleId = context.Roles.Single(r => r.Name == "Domyślna").Id,
                            PasswordHash = "TxLVWrN0l5eCTgSgWzu+9DD0hjm9GHUQFke/ixgRhXG5fL6GqohNNRUozIuQnpMQ/AMUgo5O7Sm9XPExBvK5fyULJUVdIMOT/mupzdeDDP6L/5Zlc8IBBOIwXmRszQq7VjPxff6rvMMscS3KCvk7B3LYHZmdkpYWnndqsPwaCmlb8UdUvsZYbfT4ycUr4SqO2lrhVzy5decN8PtlCMKM9dAoYwqKppkN5Bw5Ge9Rt61dCNkefgmWkMMnXJI3mmpTTSOzTPjdIqaSmV4jFnpih6oSPwgTXWjsqGJprpL7y8fztD/hSCjluLGgfXBAsYiqcgDD2gKsmjGqbHVLT+6dcg==",
                            Salt = "h1WROAWPPhJjGE9FWtKit3rolD9Sobb5BDNaO9k+TvBJpEooFM8kIRRyizkrEZ8JGm/zfbncrcePUMKhFXN8tQ=="
                        }
                        );
                    }

                    context.SaveChanges();

                    // Ustawienia
                    if (!context.Settings.Any())
                    {
                        context.Settings.AddRange(
                        new Setting()
                        {
                            Key = "CompanyName",
                            Category = "CompanyInfo",
                            Value = "Test",
                            ValueType = typeof(string).FullName,
                            Description = "Nazwa firmy"
                        },
                        new Setting()
                        {
                            Key = "CompanyStreet",
                            Category = "CompanyInfo",
                            Value = "",
                            ValueType = typeof(string).FullName,
                            Description = "Ulica przy jakiej prowadzona jest działalność"
                        },
                        new Setting()
                        {
                            Key = "CompanyNumber",
                            Category = "CompanyInfo",
                            Value = "",
                            ValueType = typeof(string).FullName,
                            Description = "Numer budynku"
                        },
                        new Setting()
                        {
                            Key = "CompanyCity",
                            Category = "CompanyInfo",
                            Value = "",
                            ValueType = typeof(string).FullName,
                            Description = "Miasto prowadzenia działalności"
                        },
                        new Setting()
                        {
                            Key = "CompanyPostCode",
                            Category = "CompanyInfo",
                            Value = "",
                            ValueType = typeof(string).FullName,
                            Description = "Kod pocztowy"
                        },
                        new Setting()
                        {
                            Key = "CompanyNIP",
                            Category = "CompanyInfo",
                            Value = "",
                            ValueType = typeof(string).FullName,
                            Description = "NIP"
                        }
                        );
                    }

                    if (!context.HardwareTypes.Any())
                    {
                        context.HardwareTypes.AddRange(
                        new HardwareType() { Name = "Laptop" },
                        new HardwareType() { Name = "Komputer PC" },
                        new HardwareType() { Name = "Telefon" },
                        new HardwareType() { Name = "Drukarka" },
                        new HardwareType() { Name = "Konsola" },
                        new HardwareType() { Name = "Nawigacja" },
                        new HardwareType() { Name = "Aparat" },
                        new HardwareType() { Name = "Monitor" },
                        new HardwareType() { Name = "Telewizor" }
                        );

                        context.SaveChanges();

                        foreach (var hwType in context.HardwareTypes)
                        {
                            if (context.CustomItems.Any()) break;
                            switch (hwType.Name)
                            {
                                case "Komputer PC":
                                case "Laptop":
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Procesor" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "RAM" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "HDD" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Grafika" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Napęd" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Bateria" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Zasilacz" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Stan" });
                                    break;
                                case "Monitor":
                                case "Telewizor":
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Przekątna ekranu" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Akcesoria" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Stan" });
                                    break;
                                case "Telefon":
                                case "Drukarka":
                                case "Nawigacja":
                                case "Konsola":
                                case "Aparat":
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Stan" });
                                    context.CustomItems.Add(new CustomItem() { HardwareTypeId = hwType.Id, Key = "Akcesoria" });
                                    break;
                            }
                        }
                    }

                    if (!context.OrderStatuses.Any())
                    {
                        context.OrderStatuses.AddRange(
                        new OrderStatus() { Name = "Nowa naprawa" },
                        new OrderStatus() { Name = "W trakcie naprawy" },
                        new OrderStatus() { Name = "Oczekuje na części" },
                        new OrderStatus() { Name = "Naprawa zakończona" },
                        new OrderStatus() { Name = "Naprawa niewykonana" }
                        );
                    }

                    if (!context.OrderTypes.Any())
                    {
                        context.OrderTypes.AddRange(
                        new OrderType() { Name = "Gwarancyjna" },
                        new OrderType() { Name = "Niegwarancyjna" },
                        new OrderType() { Name = "Pogwarancyjna" }
                        );
                    }

                    if (!context.Customers.Any())
                    {
                        var cusId = Guid.NewGuid();
                        context.Customers.AddRange(new[]
                        {
                            new Customer {
                                Id = cusId,
                                FirstName = "Jan",
                                LastName = "Nowak",
                                CompanyName = "Nowak CO",
                                DateAdded = DateTime.Now,
                                Description = "Opis",
                                Email = "email1@op.pl",
                                Email2 = "email2@op.pl",
                                NIP = "649200200",
                                PhoneNumber = "122 333 222",
                                PhoneNumber2 = "223 444 222",
                                CustomerAddresses = new List<CustomerAddress>{ new CustomerAddress {
                                                                                        City = "Zawiercie",
                                                                                        Street = "Nowe Zawiercie" ,
                                                                                        Id = Guid.NewGuid(),
                                                                                        CustomerId = cusId
                                } }
                            }
                        });
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
