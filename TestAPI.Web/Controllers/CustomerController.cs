using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Domain.Models;
using TestAPI.Domain.ViewModels;
using TestAPI.Services.Interfaces;
using TestAPI.Services.JWT;

namespace TestAPI.Web.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IGenerateJwt _generateJwt;
    public CustomerController(ICustomerService customerService, IGenerateJwt generateJwt)
    {
        _customerService = customerService;
        _generateJwt = generateJwt;
    }
    //post method to add customer
    [HttpPost("SaveCustomer")]
    public async Task<IActionResult> SaveCustomer([FromBody] UserViewModel userViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Invalid user data" });
        }
        return await _customerService.SaveCustomerAsync(userViewModel);
    }

    //soft delete method to delete customer
    [HttpDelete("DeleteCustomer/{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        if (id <= 0)
        {
            return new JsonResult(new { success = false, message = "Invalid customer ID" });
        }

        // Assuming a method exists in the service to handle soft deletion
        return await _customerService.DeleteCustomerAsync(id);
    }

    //get method to get all customers
    [HttpGet("GetAllCustomers")]
    public async Task<IActionResult> GetAllCustomers()
    {
        List<UserViewModel> customers = await _customerService.GetAllCustomersAsync();
        if (customers == null || !customers.Any())
        {
            return new JsonResult(new List<UserViewModel>());
        }
        return new JsonResult(customers);
    }

    //Get Customer by Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        if (id <= 0)
        {
            return new JsonResult(new UserViewModel());
        }

        var customer = await _customerService.GetCustomerById(id);
        if (customer == null)
        {
            return new JsonResult(new UserViewModel());
        }

        return new JsonResult(customer);
    }

    //Login method to authenticate customer
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Invalid login data" });
        }

        Customer result = await _customerService.ValidateCredentials(loginViewModel);
        if(result == null)
        {
            return Unauthorized(new { success = false, message = "Invalid credentials" });
        }
        // Generate JWT token
        string token = _generateJwt.GenerateJwtToken(result,"Customer");
        //now set token in cookies
        Response.Cookies.Append("token", token, new CookieOptions
        {
            Path="/",
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30) // Set expiration time as needed
        });

        return Ok(result);
    }

}
