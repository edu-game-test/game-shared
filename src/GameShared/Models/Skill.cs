using GameShared.Enums;

namespace GameShared.Models;

public class Skill
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SkillType Type { get; set; }
    public int Cooldown { get; set; }
    public float BaseDamageMultiplier { get; set; }
    public List<StatusEffect> Effects { get; set; } = [];
}

public class StatusEffect
{
    public string EffectId { get; set; } = string.Empty;
    public float Chance { get; set; }
    public int Duration { get; set; }
}
