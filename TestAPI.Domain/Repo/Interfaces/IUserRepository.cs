using Microsoft.AspNetCore.Mvc;
using TestAPI.Domain.Models;
using TestAPI.Domain.ViewModels;

namespace TestAPI.Domain.Repo.Interfaces;

public interface IUserRepository
{
    public Task<IActionResult> SaveUserAsync(User user);
    public Task<IActionResult> DeleteUserAsync(int id);
    public Task<List<UserViewModel>> GetAllUsersAsync();
    public Task<User> GetUserById(int id);
    public Task<UserViewModel> GetUserDetailsById(int id);
}
