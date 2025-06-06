using Microsoft.AspNetCore.Mvc;
using TestAPI.Domain.Models;
using TestAPI.Domain.Repo.Interfaces;
using TestAPI.Domain.ViewModels;
using TestAPI.Services.Interfaces;

namespace TestAPI.Services.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<IActionResult> SaveUserAsync(UserViewModel userViewModel)
    {
        if (userViewModel == null)
        {
            return new BadRequestObjectResult("User data is null");
        }
        User userToSave = new User();
        if (userViewModel.Id == 0)
        {
            User customer = new User
            {
                Name = userViewModel.Name,
                Code = userViewModel.Code,
                Email = userViewModel.Email,
                Phone = userViewModel.Phone,
                Address = userViewModel.Address,
                NickName = userViewModel.Nickname
            };
            userToSave = customer;
        }
        else
        {
            User user = await _userRepository.GetUserById(userViewModel.Id);
            if (user == null)
            {
                return new NotFoundObjectResult("Customer not found");
            }
            user.Name = userViewModel.Name;
            user.Code = userViewModel.Code;
            user.Email = userViewModel.Email;
            user.Phone = userViewModel.Phone;
            user.Address = userViewModel.Address;
            user.NickName = userViewModel.Nickname;
            userToSave = user;
            userToSave.Id = userViewModel.Id; // Ensure the ID is set for updates
        }
        return await _userRepository.SaveUserAsync(userToSave);
    }
    public Task<IActionResult> DeleteUserAsync(int id)
    {
        return _userRepository.DeleteUserAsync(id);
    }
    public async Task<List<UserViewModel>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }
    public async Task<UserViewModel> GetUserById(int id)
    {
        return await _userRepository.GetUserDetailsById(id);
    }

}
