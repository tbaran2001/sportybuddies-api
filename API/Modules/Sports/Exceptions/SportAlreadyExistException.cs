using API.Common.Exceptions;

namespace API.Modules.Sports.Exceptions;

public class SportAlreadyExistException(string name) : ConflictException($"Sport with name \"{name}\" already exist!");