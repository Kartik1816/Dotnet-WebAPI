using Microsoft.AspNetCore.Mvc;
using TestAPI.Domain.Models;
using TestAPI.Domain.ViewModels;

namespace TestAPI.Services.Interfaces;

public interface ICustomerService
{
    public Task<IActionResult> SaveCustomerAsync(UserViewModel userViewModel);
    public Task<IActionResult> DeleteCustomerAsync(int id);
    public Task<List<UserViewModel>> GetAllCustomersAsync();
    public Task<UserViewModel> GetCustomerById(int id);
    public Task<Customer> ValidateCredentials(LoginViewModel loginViewModel);

}
