using Users.Models;

namespace Users
{
    public static class Extensions
    {
        public static Dtos.UserDto AsDto(this User user)
        {
            return new Dtos.UserDto(user.Id, user.Name, user.Email, user.Age, user.Created);
        }
    }
}
