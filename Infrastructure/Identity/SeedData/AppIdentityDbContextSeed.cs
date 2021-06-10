using Core.Entities.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.SeedData
{
    public class AppIdentityDbContextSeed
    {
       
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Member" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            if (!userManager.Users.Any())
            {              
                var user = new AppUser
                {
                    DisplayName = "Isaac",
                    Email = "isaac@webshop.nl",
                    UserName = "isaac@webshop.nl",
                    AddressUser = new AddressUser
                    {
                        FirstName = "Gaorieh",
                        LastName = "Isaac",
                        Street = "Deimtstraat 26",
                        City = "Purmerend",
                        State = "Noord Holland",
                        Zipcode = "1445GN"
                    }
                };
                var user1 = new AppUser
                {
                    DisplayName = "Ahmet",
                    Email = "ahmet@webshop.nl",
                    UserName = "ahmet@webshop.nl",
                    AddressUser = new AddressUser
                    {
                        FirstName = "Ahmet",
                        LastName = "Bibi",
                        Street = "EuropaLaan 112",
                        City = "Eindhoven",
                        State = "Noord Brabant",
                        Zipcode = "1440NN"
                    }
                };

                await userManager.CreateAsync(user, "P@ssw0rd");
                await userManager.AddToRoleAsync(user, "Admin");
                await userManager.CreateAsync(user1, "P@ssw0rd");
                await userManager.AddToRoleAsync(user1, "Admin");
            }
        }
    }
}
