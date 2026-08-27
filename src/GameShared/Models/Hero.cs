using GameShared.Enums;

namespace GameShared.Models;

public class Hero
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public HeroClass Class { get; set; }
    public ElementType Element { get; set; }
    public RarityTier Rarity { get; set; }
    public int Level { get; set; }
    public int Stars { get; set; }
    public HeroStats Stats { get; set; } = new();
    public List<string> SkillIds { get; set; } = [];
    public string? EquipmentSetId { get; set; }
}

public class HeroStats
{
    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public float CriticalRate { get; set; }
    public float CriticalDamage { get; set; }
    public float Resistance { get; set; }
    public float Accuracy { get; set; }
}
