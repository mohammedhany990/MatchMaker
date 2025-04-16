using MatchMaker.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MatchMaker.Infrastructure.Identity
{
    public static class Seed
    {
        public static async Task SeedUsers(
            AppIdentityDbContext dbContext,
            UserManager<AppUser> userManager,
            string seedDataPath = null)
        {
            if (await userManager.Users.AnyAsync())
            {
                Log.Information("Users already exist in database. Skipping seeding.");
                return;
            }

            try
            {

                var usersData = await File.ReadAllTextAsync("../MatchMaker.Infrastructure/Identity/SeedingFiles/Users.json");
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

                var createdUsers = 0;
                foreach (var user in users)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(user.Email))
                        {
                            Log.Warning("Skipping user with empty email");
                            continue;
                        }


                        var userName = user.Email?.Split('@', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
                                       ?? throw new InvalidOperationException("User email cannot be null or empty");

                        var newUser = new AppUser
                        {
                            UserName = userName,
                            Email = user.Email,
                            KnownAs = userName,
                            //EmailConfirmed = true, 
                            DateOfBirth = user.DateOfBirth,
                            City = user.City,
                            Country = user.Country,
                            Gender = user.Gender,
                            Interests = user.Interests,
                            Introduction = user.Introduction,
                            Created = DateTime.UtcNow,
                            LastActive = DateTime.UtcNow,
                            LookingFor = user.LookingFor,
                            Photos = user.Photos?.Select(photo => new Photo
                            {
                                Url = photo.Url,
                                IsMain = photo.IsMain
                            }).ToList() ?? []
                        };


                        var result = await userManager.CreateAsync(newUser, "Abcd@1234");

                        if (result.Succeeded)
                        {
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