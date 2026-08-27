using GameShared.Enums;

namespace GameShared.DTOs;

public class BattleResult
{
    public string BattleSessionId { get; set; } = string.Empty;
    public BattleStatus Status { get; set; }
    public List<BattleTurnEvent> Events { get; set; } = [];
    public LootDrop? Rewards { get; set; }
}

public class BattleTurnEvent
{
    public string SourceHeroId { get; set; } = string.Empty;
    public string TargetHeroId { get; set; } = string.Empty;
    public string SkillId { get; set; } = string.Empty;
    public int DamageDealt { get; set; }
    public bool IsCritical { get; set; }
    public List<string> AppliedEffects { get; set; } = [];
    public int TargetRemainingHp { get; set; }
}
