using CondutFy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CondutFy.Application.Messages.Queries.GetConversationHistory;

public class MessageDto
{
    public Guid Id { get; set; }
    public string SenderPhone { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsFromBot { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetConversationHistoryQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetConversationHistoryQueryHandler(
        IApplicationDbContext context, 
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<MessageDto>> HandleAsync(GetConversationHistoryQuery query, CancellationToken cancellationToken)
    {
        // 🔒 Segurança LGPD: Pega o TenantId direto das claims do Token JWT do usuário logado
        var tenantId = _currentUserService.TenantId;

        if (tenantId == null)
        {
            throw new UnauthorizedAccessException("Usuário não autenticado ou Tenant não identificado.");
        }

        var dbQuery = _context.Messages
            .Where(m => m.TenantId == tenantId.Value);

        if (!string.IsNullOrEmpty(query.SenderPhone))
        {
            dbQuery = dbQuery.Where(m => m.SenderPhone == query.SenderPhone);
        }

        var messages = await dbQuery
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderPhone = m.SenderPhone,
                Content = m.Content,
                IsFromBot = m.IsFromBot,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return messages;
    }
}