using Microsoft.AspNetCore.Mvc;
using TestAPI.Domain.Models;
using TestAPI.Domain.ViewModels;

namespace TestAPI.Domain.Repo.Interfaces;

public interface ICustomerRepository
{
    public Task<IActionResult> SaveCustomerAsync(Customer customer);
    public Task<IActionResult> DeleteCustomerAsync(int id);
    public Task<List<UserViewModel>> GetAllCustomersAsync();
    public Task<UserViewModel> GetCustomerDetailsById(int id);
    public Customer GetCustomerById(int id);
    public Task<Customer> ValidateCredentials(LoginViewModel loginViewModel);
}
