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

    public GetConversationHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageDto>> HandleAsync(GetConversationHistoryQuery query, CancellationToken cancellationToken)
    {
        var dbQuery = _context.Messages
            .Where(m => m.TenantId == query.TenantId);

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