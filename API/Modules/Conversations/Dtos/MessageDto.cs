﻿namespace API.Modules.Conversations.Dtos;

public record MessageDto(Guid Id, Guid ConversationId, Guid SenderId, string Content, DateTime CreatedOn);