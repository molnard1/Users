using System.ComponentModel.DataAnnotations;
using Users.Models;

namespace Users
{
    public class Dtos
    {
        public record UserDto(Guid Id, string Name, string Email, int Age, DateTimeOffset Created);
        public record GetUserDto(Guid? Id, string? Name, string? Email, int? Age);
        public record ReplaceUserDto([Required] Guid Id, [Required] string Name, [Required] string Email, [Required] int Age);
        public record UpdateUserDto([Required] Guid Id, string? Name, string? Email, int? Age);
        public record DeleteUserDto([Required] Guid Id);
        public record CreateUserDto([Required] string Name, [Required] string Email, [Required] int Age)
        {
            public User ToUser()
            {
                return new User
                {
                    Age = Age,
                    Name = Name,
                    Email = Email,
                    Created = DateTimeOffset.Now,
                    Id = Guid.NewGuid()
                };
            }
        }
    }
}
