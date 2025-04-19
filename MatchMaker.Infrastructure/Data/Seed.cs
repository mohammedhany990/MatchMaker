using MatchMaker.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using MatchMaker.Infrastructure.Data;

namespace MatchMaker.Infrastructure.Identity
{
    public static class Seed
    {
        public static async Task SeedUsers(
            AppDbContext dbContext,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager
            )
        {
            if (await userManager.Users.AnyAsync())
            {
                Log.Information("Users already exist in database. Skipping seeding.");
                return;
            }

            try
            {
              
                var usersData = await File.ReadAllTextAsync("../MatchMaker.Infrastructure/Data/SeedingFiles/Users.json");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.Never
                };
                var users = JsonSerializer.Deserialize<List<AppUser>>(usersData, options);

                if (users is null || !users.Any())
                {
                    Log.Warning("No users found in seed data");
                    return;
                }

                // Create roles first (outside the user loop)
                var roles = new List<AppRole>
                {
                new() { Name = "Member" },
                new() { Name = "Admin" },
                new() { Name = "Moderator" },
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name))
                    {
                        await roleManager.CreateAsync(role);
                    }
                }

                var createdUsers = 0;

                // Create regular users
                foreach (var user in users)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(user.Email))
                        {
                            Log.Warning("Skipping user with empty email");
                            continue;
                        }

                        var userName = user.Email.Split('@', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                        if (string.IsNullOrEmpty(userName))
                        {
                            Log.Warning("Invalid email format for user: {Email}", user.Email);
                            continue;
                        }

                        var newUser = new AppUser
                        {
                            UserName = userName.ToLower(),
                            Email = user.Email,
                            KnownAs = userName,
                            DateOfBirth = user.DateOfBirth,
                            City = user.City,
                            Country = user.Country,
                            Gender = user.Gender,
                            Interests = user.Interests,
                            Introduction = user.Introduction,
                            Created = DateTime.UtcNow,
                            LastActive = DateTime.UtcNow,
                            LookingFor = user.LookingFor
                        };

                        // Handle photos separately to avoid reference issues
                        if (user.Photos != null && user.Photos.Any())
                        {
                            newUser.Photos = user.Photos.Select(photo => new Photo
                            {
                                Url = photo.Url,
                                IsMain = photo.IsMain
                            }).ToList();
                        }

                        var result = await userManager.CreateAsync(newUser, "Abcd@1234"); // Stronger password

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(newUser, "Member");
                            createdUsers++;
                            Log.Information("Created user {UserName} ({Email})", userName, user.Email);
                        }
                        else
                        {
                            Log.Error("Failed to create user {Email}: {Errors}",
                                user.Email,
                                string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error creating user {Email}", user.Email);
                    }
                }

                // Create admin user (outside the regular user loop)
                try
                {
                    var admin = new AppUser
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        KnownAs = "admin",
                        City = "",
                        Country = "",
                        Gender = "male",
                        Interests = "",
                        Introduction = "",
                        Created = DateTime.UtcNow,
                        LastActive = DateTime.UtcNow
                    };

                    var adminResult = await userManager.CreateAsync(admin, "Abcd@1234");

                    if (adminResult.Succeeded)
                    {
                        await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
                        Log.Information("Created admin user {UserName} ({Email})", admin.UserName, admin.Email);
                        createdUsers++;
                    }
                    else
                    {
                        Log.Error("Failed to create admin user {Email}: {Errors}",
                            admin.Email,
                            string.Join(", ", adminResult.Errors.Select(e => e.Description)));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating admin user");
                }

                Log.Information("Successfully seeded {CreatedUsers} of {TotalUsers} users",
                    createdUsers, users.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while seeding users");
                throw;
            }
        }
    }
}