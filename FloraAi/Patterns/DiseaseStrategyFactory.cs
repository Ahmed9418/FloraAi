namespace FloraAI.Patterns;

public abstract class DiseaseStrategy
{
    public abstract string ArabicLabel { get; }
    public abstract int WateringMultiplierPercent { get; }
    public int GetRecoveryWateringDays(int baseDays) =>
        Math.Max(1, (int)Math.Round(baseDays * WateringMultiplierPercent / 100.0));
}

public class HealthyStrategy : DiseaseStrategy { public override string ArabicLabel => "سليم"; public override int WateringMultiplierPercent => 100; }
public class FungiStrategy : DiseaseStrategy { public override string ArabicLabel => "فطريات"; public override int WateringMultiplierPercent => 175; }
public class BacteriaStrategy : DiseaseStrategy { public override string ArabicLabel => "بكتيريا"; public override int WateringMultiplierPercent => 150; }
public class VirusStrategy : DiseaseStrategy { public override string ArabicLabel => "فيروس"; public override int WateringMultiplierPercent => 100; }
public class PestsStrategy : DiseaseStrategy { public override string ArabicLabel => "آفات"; public override int WateringMultiplierPercent => 90; }

public static class DiseaseStrategyFactory
{
    private static readonly Dictionary<string, DiseaseStrategy> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["healthy"] = new HealthyStrategy(),
        ["fungi"] = new FungiStrategy(),
        ["bacteria"] = new BacteriaStrategy(),
        ["virus"] = new VirusStrategy(),
        ["pests"] = new PestsStrategy(),
    };

    public static DiseaseStrategy Create(string label) =>
        _map.TryGetValue(label.Trim(), out var s) ? s
        : throw new ArgumentException($"تسمية المرض '{label}' غير معروفة. القيم المقبولة: healthy, fungi, bacteria, virus, pests.");
}