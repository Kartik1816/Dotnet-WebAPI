using Microsoft.AspNetCore.Mvc;
using TestAPI.Domain.ViewModels;

namespace TestAPI.Services.Interfaces;

public interface IUserService
{
    public Task<IActionResult> SaveUserAsync(UserViewModel userViewModel);
    public Task<IActionResult> DeleteUserAsync(int id);
    public Task<List<UserViewModel>> GetAllUsersAsync();
    public Task<UserViewModel> GetUserById(int id);
}
