using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using NpgsqlTypes;
using TestAPI.Domain.DBContext;
using TestAPI.Domain.Models;
using TestAPI.Domain.Repo.Interfaces;
using TestAPI.Domain.ViewModels;

namespace TestAPI.Domain.Repo.Respositories;

public class UserRepository : IUserRepository
{
    private readonly TestDbContext _testDbContext;
    public UserRepository(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }
    public async Task<IActionResult> SaveUserAsync(User user)
    {
        try
        {
            // EntityEntry entry = _testDbContext.Entry(user);
            //     if (user.Id > 0)
            //     {
            //         _testDbContext.Entry(user).State = EntityState.Modified;
            //         await _testDbContext.SaveChangesAsync();
            //         return new JsonResult(new { success = true, message = "User Updated successfully!" });
            //     }
            //     else
            //     {
            //         await _testDbContext.Set<User>().AddAsync(user);
            //         await _testDbContext.SaveChangesAsync();
            //         return new JsonResult(new { success = true, message = "User Added successfully!" });
            //     }
            
                using (NpgsqlConnection? connection = _testDbContext.Database.GetDbConnection() as NpgsqlConnection)
                {
                    await connection.OpenAsync();

                    using (NpgsqlCommand command = new NpgsqlCommand("CALL sp_save_user(@p_id, @p_name, @p_code, @p_email, @p_phone, @p_address, @p_nickname, @p_success, @p_message)", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Input parameters
                        command.Parameters.AddWithValue("p_id", user.Id);
                        command.Parameters.AddWithValue("p_name", user.Name);
                        command.Parameters.AddWithValue("p_code", user.Code ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_email", user.Email);
                        command.Parameters.AddWithValue("p_phone", user.Phone);
                        command.Parameters.AddWithValue("p_address", user.Address ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("p_nickname", user.NickName ?? (object)DBNull.Value);

                        // Output parameters
                        NpgsqlParameter successParam = new NpgsqlParameter("p_success", NpgsqlDbType.Boolean) { Direction = ParameterDirection.InputOutput, Value = true };
                        NpgsqlParameter messageParam = new NpgsqlParameter("p_message", NpgsqlDbType.Varchar, 500) { Direction = ParameterDirection.InputOutput, Value = string.Empty };

                        command.Parameters.Add(successParam);
                        command.Parameters.Add(messageParam);

                        // Execute the stored procedure
                        await command.ExecuteNonQueryAsync();

                        // Retrieve output parameter values
                        bool success = (bool)successParam.Value;
                        string message = messageParam.Value.ToString();

                        return new JsonResult(new { success, message });
                    }
                }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while saving the user: {ex.Message}");
        }

    }
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        try
        {
            // User user = await (from u in _testDbContext.Users
            //     where u.Id == id
            //     select u).FirstOrDefaultAsync();

            // if (user != null)
            // {
            //     user.IsDeleted = true;
            //     _testDbContext.Entry(user).State = EntityState.Modified;
            //     await _testDbContext.SaveChangesAsync();
            //     return new JsonResult(new { success = true, message = "User deleted successfully!" });
            // }
            // else
            // {
            //     return new NotFoundObjectResult("User not found");
            // }

            NpgsqlParameter[] parameters = new[]
            {
                new NpgsqlParameter("p_id", NpgsqlDbType.Integer) { Value = id },
                new NpgsqlParameter("p_success", NpgsqlDbType.Boolean) { Direction = ParameterDirection.InputOutput ,Value = false },
                new NpgsqlParameter("p_message", NpgsqlDbType.Varchar) { Direction = ParameterDirection.InputOutput , Value= string.Empty }
            };

            await _testDbContext.Database
                .ExecuteSqlRawAsync("CALL sp_delete_user(@p_id, @p_success, @p_message)", parameters);

            bool success = (bool)parameters[1].Value;
            string message = parameters[2].Value.ToString();

            if (success)
            {
                return new JsonResult(new { success = true, message = message });
            }
            return new NotFoundObjectResult(message);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while deleting the user: {ex.Message}");
        }
    }
    public async Task<List<UserViewModel>> GetAllUsersAsync()
    {
        try
        {
            // return await (from user in _testDbContext.Users
            //  where user.IsDeleted == false
            //  select new UserViewModel
            //  {
            //      Id = user.Id,
            //      Name = user.Name,
            //      Code = user.Code,
            //      Email = user.Email,
            //      Phone = user.Phone,
            //      Address = user.Address,
            //      Nickname = user.NickName
            //  }).ToListAsync();

              List<UserViewModel> result = await _testDbContext.Database.SqlQueryRaw<UserViewModel>
                (
                    "SELECT id, name, code, email, phone, address, nick_name as nickname FROM fn_get_all_users()"
                ).ToListAsync();

                return result ?? new List<UserViewModel>();
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the users: {ex.Message}");
        }
    }
    public async Task<User> GetUserById(int id)
    {
        try
        {
            // return await (from u in _testDbContext.Users
            //  where u.Id == id
            //  select u).FirstOrDefaultAsync() ?? throw new Exception("User not found");
            string query = "SELECT \"Id\", \"Name\", \"Code\", \"Email\", \"Phone\", \"Address\", \"NickName\",\"IsDeleted\" FROM get_user_by_id(@p_id)";
            return await _testDbContext.Database
                .SqlQueryRaw<User>(query, new NpgsqlParameter("p_id", id))
                .FirstOrDefaultAsync() ?? new User();
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the user by ID: {ex.Message}");
        }
    }
    public async Task<UserViewModel> GetUserDetailsById(int id)
    {
        try
        {
            // UserViewModel customerEntity = await (from c in _testDbContext.Customers
            //     where c.Id == id && c.IsDeleted == false
            //     select new UserViewModel
            //     {
            //         Id = c.Id,
            //         Name = c.Name,
            //         Code = c.Code,
            //         Email = c.Email,
            //         Phone = c.Phone,
            //         Address = c.Address,
            //         Nickname = c.NickName
            //     }).FirstOrDefaultAsync() ?? new UserViewModel();

            //    return customerEntity; 

            string query = "SELECT \"Id\", \"Name\", \"Code\", \"Email\", \"Phone\", \"Address\", \"Nickname\" FROM fn_get_user_by_id(@p_id)";
            
            UserViewModel user = await _testDbContext.Database
                .SqlQueryRaw<UserViewModel>(query, new NpgsqlParameter("p_id", id))
                .FirstOrDefaultAsync() ?? new UserViewModel();

            return user;
                          
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the user details by ID: {ex.Message}");
        }
    }

}
