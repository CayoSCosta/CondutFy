namespace CondutFy.Application.Webhooks.Commands.ReceiveMessage;

public record ReceiveMessageCommand(
    string Identifier,       // Ex: O número da instância ou página que recebeu a msg
    string SenderPhone,      // Quem mandou a mensagem (o cliente final)
    string MessageContent,   // O texto da mensagem
    string ExternalMessageId // ID único da mensagem no provedor (para evitar duplicidade)
);