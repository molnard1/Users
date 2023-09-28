using Microsoft.AspNetCore.Mvc;
using Users.Models;
using static Users.Dtos;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Connect _db = new("db_user", "127.0.0.1", "", "root");

        private bool UserExists(Guid id)
        {
            _db.Connection.Open();
            var command = _db.Connection.CreateCommand();
            command.CommandText = "SELECT null FROM users WHERE Id = @Id";
            command.Parameters.AddWithValue("Id", id);
            var reader = command.ExecuteReader();
            var ret = reader.HasRows;
            reader.Close();
            _db.Connection.Close();
            return ret;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] GetUserDto get)
        {
            try
            {
                _db.Connection.Open();
                var command = _db.Connection.CreateCommand();
                var sql = "SELECT * FROM users WHERE 1=1 ";
                if (get.Age != null)
                {
                    sql += "AND Age = @Age";
                    command.Parameters.AddWithValue("Age", get.Age);
                }

                if (get.Email != null)
                {
                    sql += "AND Email = @Email";
                    command.Parameters.AddWithValue("Email", get.Email);
                }

                if (get.Name != null)
                {
                    sql += "AND Name = @Name";
                    command.Parameters.AddWithValue("Name", get.Name);
                }

                if (get.Id != null)
                {
                    sql += "AND Id = @Id";
                    command.Parameters.AddWithValue("Id", get.Id);
                }

                command.CommandText = sql;

                List<User> users = new();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Age = reader.GetInt32("Age"),
                        Created = reader.GetDateTime("Created"),
                        Email = reader.GetString("Email"),
                        Name = reader.GetString("Name"),
                        Id = reader.GetGuid("Id")
                    });
                }

                _db.Connection.Close();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post(CreateUserDto createuser)
        {
            var user = createuser.ToUser();
            try
            {
                _db.Connection.Open();

                var command = _db.Connection.CreateCommand();
                command.CommandText =
                    "INSERT INTO users(Id, Name, Email, Age, Created) VALUES(@Id, @Name, @Email, @Age, @Created)";
                command.Parameters.AddWithValue("Id", user.Id);
                command.Parameters.AddWithValue("Name", user.Name);
                command.Parameters.AddWithValue("Email", user.Email);
                command.Parameters.AddWithValue("Age", user.Age);
                command.Parameters.AddWithValue("Created", user.Created);

                command.ExecuteNonQuery();

                _db.Connection.Close();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return StatusCode(201, user);
        }

        [HttpPut]
        public IActionResult Put(ReplaceUserDto replace)
        {
            try
            {
                if (!UserExists(replace.Id))
                {
                    return NotFound("Nem lézetik felhasználó ezzel az ID-vel!");
                }

                _db.Connection.Open();
                var command = _db.Connection.CreateCommand();
                command.CommandText = "UPDATE users SET Name = @Name, Email = @Email, Age = @Age WHERE Id = @Id";
                command.Parameters.AddWithValue("Name", replace.Name);
                command.Parameters.AddWithValue("Email", replace.Email);
                command.Parameters.AddWithValue("Age", replace.Age);
                command.Parameters.AddWithValue("Id", replace.Id);
                command.ExecuteNonQuery();
                _db.Connection.Close();

                return NoContent();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        public IActionResult Patch(UpdateUserDto update)
        {
            try
            {
                if (!UserExists(update.Id))
                {
                    return NotFound("Nem lézetik felhasználó ezzel az ID-vel!");
                }

                _db.Connection.Open();

                var command = _db.Connection.CreateCommand();
                var sql = "UPDATE users SET ";
                if (update.Age != null)
                {
                    sql += "Age = @Age,";
                    command.Parameters.AddWithValue("Age", update.Age);
                }

                if (update.Email != null)
                {
                    sql += "Email = @Email,";
                    command.Parameters.AddWithValue("Email", update.Email);
                }

                if (update.Name != null)
                {
                    sql += "Name = @Name,";
                    command.Parameters.AddWithValue("Name", update.Name);
                }

                sql = sql.TrimEnd(',');
                sql += " WHERE Id = @Id";
                command.Parameters.AddWithValue("Id", update.Id);

                command.CommandText = sql;
                if (command.Parameters.Count > 1)
                {
                    command.ExecuteNonQuery();
                }
                else
                {
                    return BadRequest("Legalább 1 változtatandó paraméter megadása szükséges!");
                }

                _db.Connection.Close();

                return NoContent();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IActionResult Delete(DeleteUserDto delete)
        {
            if (!UserExists(delete.Id))
            {
                return NotFound("Nem lézetik felhasználó ezzel az ID-vel!");
            }

            _db.Connection.Open();
            var command = _db.Connection.CreateCommand();
            command.CommandText = "DELETE FROM users WHERE Id=@Id";
            command.Parameters.AddWithValue("Id", delete.Id);
            command.ExecuteNonQuery();
            _db.Connection.Close();

            return NoContent();
        }

    }
}