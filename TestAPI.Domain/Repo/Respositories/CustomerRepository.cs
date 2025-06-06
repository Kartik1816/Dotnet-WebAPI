using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Domain.DBContext;
using TestAPI.Domain.Models;
using TestAPI.Domain.Repo.Interfaces;
using TestAPI.Domain.ViewModels;

namespace TestAPI.Domain.Repo.Respositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly TestDbContext _testDbContext;
    public CustomerRepository(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }
    public async Task<IActionResult> SaveCustomerAsync(Customer customer)
    {
        try
        {
            if (customer.Id != 0)
            {
                _testDbContext.Customers.Update(customer);
                await _testDbContext.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "User Updated successfully!" });
            }
            else
            {
                await _testDbContext.Customers.AddAsync(customer);
                await _testDbContext.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "User Added successfully!" });
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while updating the user: {ex.Message}");
        }
    }

    public Customer GetCustomerById(int id)
    {
        try
        {
            Customer customer = _testDbContext.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }
            return customer;
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the customer: {ex.Message}");
        }
    }
    public async Task<IActionResult> DeleteCustomerAsync(int id)
    {
        try
        {
            Customer customer = GetCustomerById(id);
            if (customer == null)
            {
                return new JsonResult(new { success = false, message = "Customer not found" });
            }

            customer.IsDeleted = true;
            _testDbContext.Customers.Update(customer);
            await _testDbContext.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Customer Deleted successfully!" });
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while deleting the Customer: {ex.Message}");
        }
    }
    public async Task<List<UserViewModel>> GetAllCustomersAsync()
    {
        try
        {
            List<UserViewModel> customers = await _testDbContext.Customers
                .Where(c => c.IsDeleted == false)
                .Select(c => new UserViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Email = c.Email,
                    Phone = c.Phone,
                    Address = c.Address,
                    Nickname = c.NickName
                })
                .ToListAsync();

            return customers;
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the customers: {ex.Message}");
        }
    }

    public async Task<UserViewModel> GetCustomerDetailsById(int id)
    {
        try
        {
            UserViewModel customer = await _testDbContext.Customers
                .Where(c => c.Id == id && c.IsDeleted == false)
                .Select(c => new UserViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Email = c.Email,
                    Phone = c.Phone,
                    Address = c.Address,
                    Nickname = c.NickName
                })
                .FirstOrDefaultAsync() ?? new UserViewModel();

            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            return customer;
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the customer: {ex.Message}");
        }
    }
    public async Task<Customer> ValidateCredentials(LoginViewModel loginViewModel)
    {
        try
        {
            Customer customer = await _testDbContext.Customers
                .FirstOrDefaultAsync(c => c.Email == loginViewModel.Email && c.Code == loginViewModel.Code);

            if (customer == null)
            {
                throw new Exception("Invalid credentials");
            }
            else
            {
                return customer;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while validating credentials: {ex.Message}");
        }
    }
}
