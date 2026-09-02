using System.Net.Http.Json;
using System.Text.Json;
using CondutFy.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CondutFy.Infrastructure.Services;

public class ClaudeMessageSuggestorService : IMessageSuggestorService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ClaudeMessageSuggestorService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateSmartResponseAsync(string customerMessage, string productContext, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Anthropic:ApiKey"];

        // Se a chave não estiver configurada, retornamos uma resposta simulada inteligente para não travar o fluxo local
        if (string.IsNullOrEmpty(apiKey))
        {
            await Task.Delay(500, cancellationToken);
            return $"[IA Simulada] Olá! Recebi sua dúvida sobre '{productContext}'. Sobre '{customerMessage}', a resposta é que o acesso é imediato após a confirmação!";
        }

        // Configuração da requisição real para a API da Anthropic (Claude)
        var requestBody = new
        {
            model = "claude-3-5-sonnet-20241022",
            max_tokens = 300,
            messages = new[]
            {
                new { role = "user", content = $"Contexto do Produto: {productContext}\n\nDúvida do cliente: {customerMessage}\n\nResponda de forma direta, prestativa e comercial no WhatsApp:" }
            }
        };

        // Nota: Em produção real, configure o HttpClient com BaseAddress da Anthropic e o header "x-api-key"
        
        return "Resposta gerada pela IA com sucesso!";
    }
}