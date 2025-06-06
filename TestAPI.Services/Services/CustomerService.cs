using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TestAPI.Domain.Models;
using TestAPI.Domain.Repo.Interfaces;
using TestAPI.Domain.ViewModels;
using TestAPI.Services.Interfaces;

namespace TestAPI.Services.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task<IActionResult> SaveCustomerAsync(UserViewModel userViewModel)
    {
        if (userViewModel == null)
        {
            return new BadRequestObjectResult("User data is null");
        }
        Customer customerToSave = new Customer();
        if (userViewModel.Id == 0)
        {
            Customer customer = new Customer
            {
                Name = userViewModel.Name,
                Code = userViewModel.Code,
                Email = userViewModel.Email,
                Phone = userViewModel.Phone,
                Address = userViewModel.Address,
                NickName = userViewModel.Nickname
            };
            customerToSave = customer;
        }
        else
        {
            Customer customer = _customerRepository.GetCustomerById(userViewModel.Id);
            if (customer == null)
            {
                return new NotFoundObjectResult("Customer not found");
            }
            customer.Name = userViewModel.Name;
            customer.Code = userViewModel.Code;
            customer.Email = userViewModel.Email;
            customer.Phone = userViewModel.Phone;
            customer.Address = userViewModel.Address;
            customer.NickName = userViewModel.Nickname;
            customerToSave = customer;
        }
        return await _customerRepository.SaveCustomerAsync(customerToSave);
    }
    public async Task<IActionResult> DeleteCustomerAsync(int id)
    {
        return await _customerRepository.DeleteCustomerAsync(id);
    }
    public async Task<List<UserViewModel>> GetAllCustomersAsync()
    {
        return await _customerRepository.GetAllCustomersAsync();
    }
    public async Task<UserViewModel> GetCustomerById(int id)
    {
        return await _customerRepository.GetCustomerDetailsById(id);
    }
    public async Task<Customer> ValidateCredentials(LoginViewModel loginViewModel)
    {
        return await _customerRepository.ValidateCredentials(loginViewModel);
    }
}
