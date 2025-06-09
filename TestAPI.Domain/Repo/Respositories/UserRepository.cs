using System.Data;
using System.Reflection;
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

            Dictionary<string, object?> inputParameters = GenerateStoredProcedureParametersFromObject(user);

            NpgsqlParameter[]? outputParameters = GetSuccessMessageParameters();

            (bool success, string message) = await ExecuteStoredProcedureAsync(DatabaseObject.SpSaveUser, inputParameters, outputParameters);
            return new JsonResult(new { success, message });
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
            // using (NpgsqlConnection? connection = _testDbContext.Database.GetDbConnection() as NpgsqlConnection)
            // {
            //     await connection.OpenAsync();
            //     using (NpgsqlCommand command = new NpgsqlCommand("CALL sp_delete_user(@p_id, @p_success, @p_message)", connection))
            //     {
            //         command.CommandType = CommandType.Text;

            //         // Add input parameter
            //         Dictionary<string, object>? deleteParameters = GenerateStoredProcedureParameterForSingleInput(id);
            //         foreach (var param in deleteParameters)
            //         {
            //             command.Parameters.AddWithValue(param.Key, param.Value);
            //         }
            //         // Output parameters
            //         NpgsqlParameter[] successMessageParams = GetSuccessMessageParameters();
            //         command.Parameters.Add(successMessageParams[0]);
            //         command.Parameters.Add(successMessageParams[1]);

            //         // Execute the stored procedure
            //         await command.ExecuteNonQueryAsync();

            //         // Retrieve output parameter values
            //         bool success = (bool)successMessageParams[0].Value;
            //         string message = successMessageParams[1].Value.ToString();

            //         return new JsonResult(new { success, message });
            //     }
            // }

            var inputParameters = GenerateStoredProcedureParameterForSingleInput(id);

            var outputParameters = GetSuccessMessageParameters();

            var (success, message) = await ExecuteStoredProcedureAsync(DatabaseObject.SpDeleteUser, inputParameters, outputParameters);

            return new JsonResult(new { success, message });
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

            // List<UserViewModel> result = await _testDbContext.Database.SqlQueryRaw<UserViewModel>
            //   (
            //       "SELECT id, name, code, email, phone, address, nick_name as nickname FROM fn_get_all_users()"
            //   ).ToListAsync();

            // return result ?? new List<UserViewModel>();
            // passs id, name, code, email, phone, address, nick_name as nickname in ExecuteFunctionAsync method
            var outputParameters = GetOutputParameterForFunctions(new UserViewModel());
            return await ExecuteFunctionAsync<List<UserViewModel>>(DatabaseObject.FnGetAllUsers, new Dictionary<string, object?>(),outputParameters)
                   ?? new List<UserViewModel>();
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
            // string query = "SELECT \"Id\", \"Name\", \"Code\", \"Email\", \"Phone\", \"Address\", \"NickName\",\"IsDeleted\" FROM get_user_by_id(@p_id)";
            // return await _testDbContext.Database
            //     .SqlQueryRaw<User>(query, new NpgsqlParameter("p_id", id))
            //     .FirstOrDefaultAsync() ?? new User();

            var parameters = new Dictionary<string, object?>
            {
                { "p_id", id }
            };

            return await ExecuteFunctionAsync<User>(DatabaseObject.FnGetUserById, parameters,null) ?? throw new Exception("User not found");
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

            var parameters = new Dictionary<string, object?>
            {
                { "p_id", id }
            };

            return await ExecuteFunctionAsync<UserViewModel>(DatabaseObject.FnGetUserById, parameters,null) ?? new UserViewModel();

        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the user details by ID: {ex.Message}");
        }
    }

    private Dictionary<string, object?> GenerateStoredProcedureParametersFromObject(object obj)
    {
        Dictionary<string, object?> parameters = new Dictionary<string, object?>();

        foreach (PropertyInfo property in obj.GetType().GetProperties())
        {
            string propertyName = $"p_{property.Name}";
            object? propertyValue = property.GetValue(obj) ?? DBNull.Value;
            parameters.Add(propertyName, propertyValue);
        }

        return parameters;
    }
    private Dictionary<string, object?> GenerateStoredProcedureParameterForSingleInput(int id)
    {
        return new Dictionary<string, object?>
        {
            { "p_id", id }
        };
    }
    private NpgsqlParameter[] GetSuccessMessageParameters()
    {
        return new NpgsqlParameter[]
        {
            new NpgsqlParameter("p_success", NpgsqlDbType.Boolean) { Direction = ParameterDirection.InputOutput, Value = true },
            new NpgsqlParameter("p_message", NpgsqlDbType.Varchar, 500) { Direction = ParameterDirection.InputOutput, Value = string.Empty }
        };
    }

    private async Task<(bool success, string message)> ExecuteStoredProcedureAsync(string storedProcedureName, Dictionary<string, object?> inputParameters, NpgsqlParameter[] outputParameters)
    {
        using (NpgsqlConnection? connection = _testDbContext.Database.GetDbConnection() as NpgsqlConnection)
        {
            if (connection == null) throw new Exception("Database connection is not available.");

            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand($"CALL {storedProcedureName}({string.Join(", ", inputParameters.Keys.Select(key => $"@{key}"))}, {string.Join(", ", outputParameters.Select(param => $"@{param.ParameterName}"))})", connection))
            {
                command.CommandType = CommandType.Text;

                // Add input parameters dynamically
                foreach (var param in inputParameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                // Add output parameters
                foreach (var outputParam in outputParameters)
                {
                    command.Parameters.Add(outputParam);
                }


                await command.ExecuteNonQueryAsync();

                // Retrieve output parameter values
                bool success = outputParameters[0].Value == null;
                string message = outputParameters[1].Value?.ToString() ?? string.Empty;

                return (success, message);
            }
        }
    }
    private async Task<T?> ExecuteFunctionAsync<T>(string functionName, Dictionary<string, object?> parameters, Dictionary<string,object?>? outputprameter) where T : class
    {
        try
        {
            // Build the SQL query dynamically
            string parameterPlaceholders = string.Join(", ", parameters.Keys.Select(key => $"@{key}"));
            string query = parameters.Count > 0
            ? $"SELECT {string.Join(", ", outputprameter.Keys)} FROM {functionName}({parameterPlaceholders})"
            : $"SELECT {string.Join(", ", outputprameter.Keys)} FROM {functionName}()";

            // Create NpgsqlParameter array from the dictionary
            NpgsqlParameter[]? npgsqlParameters = parameters.Select(param => new NpgsqlParameter(param.Key, param.Value ?? DBNull.Value)).ToArray();

            // Execute the query and map the result
            if (npgsqlParameters.Count() > 0)
            {
                return await _testDbContext.Database
                .SqlQueryRaw<T>(query, npgsqlParameters)
                .FirstOrDefaultAsync();
            }
            else
            {
                return await _testDbContext.Database
                .SqlQueryRaw<T>(query)
                .FirstOrDefaultAsync();
            }

        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while executing the function '{functionName}': {ex.Message}");
        }
    }

    private Dictionary<string, object?> GetOutputParameterForFunctions(object obj)
    {
        Dictionary<string, object?> parameters = new Dictionary<string, object?>();

        foreach (PropertyInfo property in obj.GetType().GetProperties())
        {
            string propertyName = $"{char.ToLower(property.Name[0])}{property.Name.Substring(1)}";
            object? propertyValue = property.GetValue(obj) ?? DBNull.Value;
            parameters.Add(propertyName, propertyValue);
        }

        return parameters;
    }

}
