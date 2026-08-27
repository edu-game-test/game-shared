namespace GameShared.DTOs;

public class BattleRequest
{
    public string PlayerId { get; set; } = string.Empty;
    public List<string> PartyHeroIds { get; set; } = [];
    public string StageId { get; set; } = string.Empty;
}

public class BattleTurnRequest
{
    public string BattleSessionId { get; set; } = string.Empty;
    public string SourceHeroId { get; set; } = string.Empty;
    public string TargetHeroId { get; set; } = string.Empty;
    public string SkillId { get; set; } = string.Empty;
}
