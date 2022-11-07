using ZeeslagFrontEnd.Shared.Enums;

namespace ZeeslagFrontEnd.Shared.Records;

public record Game(Guid Id, string Player, Difficulty Difficulty, Guid BoardId);
