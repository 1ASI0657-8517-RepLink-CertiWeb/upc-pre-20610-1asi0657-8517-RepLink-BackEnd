using CertiWeb.API.Users.Domain.Model.Aggregates;

namespace CertiWeb.API.Users.Application.Internal.OutboundServices;

public interface ITokenService{
    string GenerateToken(User user);

    Task<int?> ValidateToken(string token);

    /// <summary>
    /// Extracts the role/plan claim embedded in a previously issued token.
    /// </summary>
    /// <param name="token">The token to inspect.</param>
    /// <returns>The role claim value if the token is valid and carries one, null otherwise.</returns>
    Task<string?> GetRoleFromToken(string token);
}