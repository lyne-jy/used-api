using UsedAPI.Models;

namespace UsedAPI.Interfaces;

public interface ITokenService
{
    string CreateToken(Seller seller);
}