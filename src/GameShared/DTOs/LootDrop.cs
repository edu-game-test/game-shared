using GameShared.Enums;

namespace GameShared.DTOs;

public class LootDrop
{
    public int Gold { get; set; }
    public int ExperiencePoints { get; set; }
    public List<HeroFragment> HeroFragments { get; set; } = [];
    public List<EquipmentDrop> Equipment { get; set; } = [];
}

public class HeroFragment
{
    public string HeroTemplateId { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class EquipmentDrop
{
    public string EquipmentTemplateId { get; set; } = string.Empty;
    public RarityTier Rarity { get; set; }
}
