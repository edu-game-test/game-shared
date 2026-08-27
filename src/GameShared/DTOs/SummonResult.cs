using GameShared.Enums;

namespace GameShared.DTOs;

public class SummonResult
{
    public List<SummonedHero> Heroes { get; set; } = [];
    public int GemsSpent { get; set; }
}

public class SummonedHero
{
    public string HeroTemplateId { get; set; } = string.Empty;
    public string HeroName { get; set; } = string.Empty;
    public RarityTier Rarity { get; set; }
    public bool IsNew { get; set; }
}
