using API.Common.Exceptions;

namespace API.Modules.Conversations.Exceptions;

public class ConversationNotFoundException(Guid id) : NotFoundException("Conversation", id);