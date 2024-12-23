using CraftOfWord.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CraftOfWord.Endpoints
{
    public static class UserEndpoints
    {
        public static void RegisterUserEndpoints(this WebApplication app)
        {
            // Register a new user
            app.MapPost("/users", async (RegisterUserDto userDto, CraftOfWordContext db) =>
            {
                if (await db.User.AnyAsync(u => u.Email == userDto.Email))
                {
                    return Results.Conflict("User with this email already exists.");
                }

                var user = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Password = HashPassword(userDto.Password),
                    Points = userDto.Points,
                    Role = "User" // Set default role as "User"
                };

                db.User.Add(user);
                await db.SaveChangesAsync();

                return Results.Created($"/users/{user.Id}", user);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "RegisterUser";
                operation.Description = "Creates a user with provided information.";
                return operation;
            })
            .Produces<User>(StatusCodes.Status201Created, "application/json")
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);


            // User login
            app.MapPost("/login", async (LoginDto loginDto, CraftOfWordContext db, IConfiguration configuration) =>
            {
                var user = await db.User.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || user.Password != HashPassword(loginDto.Password))
                {
                    return Results.Unauthorized();
                }

                var token = GenerateJwtToken(user, configuration);

                return Results.Ok(new { token });
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "UserLogin";
                operation.Description = "Authenticates a user and returns a JWT token.";
                return operation;
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            // Get user information
            app.MapGet("/users/{id}", async (int id, CraftOfWordContext db) =>
            {
                var user = await db.User.FindAsync(id);

                if (user == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(user);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "GetUserInformation";
                operation.Description = "Retrieves information about a specific user.";
                return operation;
            })
            .Produces<User>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

            // Update user information
            app.MapPut("/users/{id}", async (int id, UpdateUserDto updateUserDto, CraftOfWordContext db, HttpContext httpContext) =>
            {
                var user = await db.User.FindAsync(id);

                if (user == null)
                {
                    return Results.NotFound();
                }

                // Get the user ID and role from the JWT token
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);

                if (userIdClaim == null || roleClaim == null)
                {
                    return Results.Unauthorized();
                }

                var tokenUserId = int.Parse(userIdClaim.Value);
                var tokenUserRole = roleClaim.Value;

                // Check if the token user is the same as the user being updated, or if the token user is an admin
                if (tokenUserId != id && tokenUserRole != "Admin")
                {
                    return Results.Forbid();
                }

                user.Name = updateUserDto.Name ?? user.Name;
                user.Points = updateUserDto.Points != 0 ? updateUserDto.Points : user.Points;

                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "UpdateUserInformation";
                operation.Description = "Updates information of a specific user.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

            // Delete user information
            app.MapDelete("/users/{id}", async (int id, CraftOfWordContext db, HttpContext httpContext) =>
            {
                var user = await db.User.FindAsync(id);

                if (user == null)
                {
                    return Results.NotFound();
                }

                // Get the user ID and role from the JWT token
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);

                if (userIdClaim == null || roleClaim == null)
                {
                    return Results.Unauthorized();
                }

                var tokenUserId = int.Parse(userIdClaim.Value);
                var tokenUserRole = roleClaim.Value;

                // Check if the token user is the same as the user being deleted, or if the token user is an admin
                if (tokenUserId != id && tokenUserRole != "Admin")
                {
                    return Results.Forbid();
                }

                db.User.Remove(user);
                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "DeleteUser";
                operation.Description = "Deletes a user by ID.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

            // Get all users (Admin only)
            app.MapGet("/users", [Authorize(Roles = "Admin")] async (CraftOfWordContext db) =>
            {
                var users = await db.User.Select(user => new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Points = user.Points,
                    Role = user.Role
                }).ToListAsync();

                return Results.Ok(users);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "GetAllUsers";
                operation.Description = "Retrieves a list of all users.";
                return operation;
            })
            .Produces<List<UserResponseDto>>(StatusCodes.Status200OK, "application/json");

            // Helper method for password hashing
            static string HashPassword(string password)
            {
                using var sha256 = SHA256.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }

            // Helper method for generating JWT tokens
            static string GenerateJwtToken(User user, IConfiguration configuration)
            {
                var jwtSettings = configuration.GetSection("Jwt");
                var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }
}
