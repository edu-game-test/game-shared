using GameShared.Enums;

namespace GameShared.Models;

public class Equipment
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentSlot Slot { get; set; }
    public RarityTier Rarity { get; set; }
    public int Level { get; set; }
    public Dictionary<string, float> StatBonuses { get; set; } = [];
}
