using TestAPI.Domain.Models;

namespace TestAPI.Services.JWT;

public interface IGenerateJwt
{
    string GenerateJwtToken(Customer user,string role);
}
