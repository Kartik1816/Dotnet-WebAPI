using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Domain.ViewModels;
using TestAPI.Services.Interfaces;

namespace TestAPI.Web.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    //post method to save user
    [HttpPost("SaveUser")]
    public async Task<IActionResult> SaveUser([FromBody] UserViewModel userViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Invalid user data" });
        }
        return await _userService.SaveUserAsync(userViewModel);
    }
    //soft delete method to delete user
    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (id <= 0)
        {
            return new JsonResult(new { success = false, message = "Invalid user ID" });
        }
        return await _userService.DeleteUserAsync(id);
    }
    //get method to get all users
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        List<UserViewModel> users = await _userService.GetAllUsersAsync();
        if (users == null || !users.Any())
        {
            return new JsonResult(new List<UserViewModel>());
        }
        return new JsonResult(users);
    }
    //Get User by Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        if (id <= 0)
        {
            return new JsonResult(new UserViewModel());
        }
        UserViewModel user = await _userService.GetUserById(id);
        if (user == null)
        {
            return new JsonResult(new UserViewModel());
        }
        return new JsonResult(user);
    }
    
}
