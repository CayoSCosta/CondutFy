using System.Security.Claims;
using CondutFy.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CondutFy.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier) 
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub");
            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            // Extrai o TenantId de forma segura direto das claims do Token JWT do usuário
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenantId");
            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }
    }

    public string? Email => 
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => 
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}