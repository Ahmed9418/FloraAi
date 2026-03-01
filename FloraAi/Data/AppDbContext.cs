using FloraAI.Models;
using Microsoft.EntityFrameworkCore;

namespace FloraAI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<PlantLibrary> PlantLibrary => Set<PlantLibrary>();
    public DbSet<UserPlant> UserPlants => Set<UserPlant>();
    public DbSet<DiagnosisRecord> DiagnosisRecords => Set<DiagnosisRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            e.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<PlantLibrary>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Name).IsUnique();
            e.Property(p => p.Name).HasMaxLength(150).IsRequired();
            e.Property(p => p.ArabicName).HasMaxLength(150).IsRequired();
            e.Property(p => p.WateringInstructions).HasMaxLength(300).IsRequired();
            e.Property(p => p.SunlightRequirement).HasMaxLength(150).IsRequired();
            e.Property(p => p.FertilizingInstructions).HasMaxLength(300).IsRequired();
            e.Property(p => p.CareTips).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<UserPlant>(e =>
        {
            e.HasKey(up => up.Id);
            e.Property(up => up.NickName).HasMaxLength(100);
            e.HasOne(up => up.User).WithMany(u => u.UserPlants)
             .HasForeignKey(up => up.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(up => up.PlantLibrary).WithMany(p => p.UserPlants)
             .HasForeignKey(up => up.PlantLibraryId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DiagnosisRecord>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.PlantNameInput).HasMaxLength(150).IsRequired();
            e.Property(d => d.DiseaseLabel).HasMaxLength(50).IsRequired();
            e.Property(d => d.ArabicDiseaseLabel).HasMaxLength(100).IsRequired();
            e.Property(d => d.TreatmentText).HasColumnType("nvarchar(max)").IsRequired();
            e.HasOne(d => d.User).WithMany(u => u.DiagnosisRecords)
             .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(d => d.UserPlant).WithMany(up => up.DiagnosisRecords)
             .HasForeignKey(d => d.UserPlantId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
        });

        SeedPlantLibrary(modelBuilder);
    }

    private static void SeedPlantLibrary(ModelBuilder modelBuilder)
    {
        var seed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<PlantLibrary>().HasData(
            new PlantLibrary { Id = 1, Name = "pothos", ArabicName = "نبتة البوثوس", BaseWateringDays = 7, WateringInstructions = "كل 7 أيام — اترك التربة تجف جزئياً بين الريّات.", SunlightRequirement = "ضوء خافت إلى متوسط.", FertilizingInstructions = "سماد سائل متوازن مرة كل شهر خلال الربيع والصيف.", CareTips = "نبتة مثالية للمبتدئين. تتحمّل الإهمال وتُنقّي الهواء.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 2, Name = "monstera", ArabicName = "نبتة المونستيرا", BaseWateringDays = 7, WateringInstructions = "كل 7-5 أيام عندما تجف التربة العلوية.", SunlightRequirement = "ضوء غير مباشر ساطع.", FertilizingInstructions = "سماد سائل غني بالنيتروجين كل 3 أسابيع في موسم النمو.", CareTips = "امسحي الأوراق بقطعة قماش رطبة شهرياً. تحتاج عمود داعم كلما كبرت.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 3, Name = "snake plant", ArabicName = "نبتة الثعبان", BaseWateringDays = 14, WateringInstructions = "كل 14 يوماً صيفاً وكل 3-4 أسابيع شتاءً.", SunlightRequirement = "تتأقلم مع أي إضاءة.", FertilizingInstructions = "سماد خفيف مرة في الربيع ومرة في الصيف فقط.", CareTips = "من أقوى النباتات الداخلية. لا تتركيها في ماء راكد.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 4, Name = "peace lily", ArabicName = "زنبق السلام", BaseWateringDays = 7, WateringInstructions = "كل 7 أيام أو عند بدء ترهّل الأوراق قليلاً.", SunlightRequirement = "ضوء خافت إلى متوسط.", FertilizingInstructions = "سماد سائل شامل كل 6 أسابيع خلال موسم النمو.", CareTips = "تترهّل أوراقها عند العطش وترتفع بعد الري مباشرة.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 5, Name = "fiddle leaf fig", ArabicName = "نبتة التين كمنجة", BaseWateringDays = 7, WateringInstructions = "كل 7 أيام — تأكّد من جفاف الطبقة العلوية قبل الري.", SunlightRequirement = "ضوء غير مباشر ساطع لساعات طويلة.", FertilizingInstructions = "سماد سائل للنباتات الورقية كل 4 أسابيع.", CareTips = "حساسة للتحريك والتيارات الهوائية. اختاري مكاناً ثابتاً.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 6, Name = "spider plant", ArabicName = "نبتة العنكبوت", BaseWateringDays = 7, WateringInstructions = "كل 7 أيام صيفاً وكل 10-14 يوماً شتاءً.", SunlightRequirement = "ضوء متوسط إلى ساطع غير مباشر.", FertilizingInstructions = "سماد متوازن كل شهرين في موسم النمو.", CareTips = "حساسة لمياه الفلور — استخدمي ماء مفلتراً.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 7, Name = "rubber plant", ArabicName = "نبتة المطاط", BaseWateringDays = 10, WateringInstructions = "كل 10 أيام صيفاً وكل أسبوعين شتاءً.", SunlightRequirement = "ضوء غير مباشر ساطع.", FertilizingInstructions = "سماد سائل مخفّف كل 4 أسابيع في موسم النمو.", CareTips = "امسحي الأوراق اللامعة بقطعة رطبة. قلّلي الري في الشتاء.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 8, Name = "aloe vera", ArabicName = "صبار الألوفيرا", BaseWateringDays = 14, WateringInstructions = "كل 14 يوماً — جفاف كامل للتربة قبل الري.", SunlightRequirement = "ضوء مباشر أو غير مباشر ساطع.", FertilizingInstructions = "سماد خاص بالصبار مرة في الربيع فقط.", CareTips = "تربة مفككة سريعة التصريف. تعفّن الجذور هو أكبر خطر عليها.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 9, Name = "zz plant", ArabicName = "نبتة الزد زد", BaseWateringDays = 14, WateringInstructions = "كل 14 يوماً — تخزّن الماء في جذورها.", SunlightRequirement = "تتأقلم مع الإضاءة الخافتة.", FertilizingInstructions = "سماد خفيف مرة كل موسم نمو.", CareTips = "بطيئة النمو لكنها قوية جداً. تنمو في أي بيئة تقريباً.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 10, Name = "calathea", ArabicName = "نبتة الكاليثيا", BaseWateringDays = 5, WateringInstructions = "كل 5 أيام — رطوبة منتظمة بدون تشبّع.", SunlightRequirement = "ضوء خافت إلى متوسط.", FertilizingInstructions = "سماد سائل خفيف الجرعة كل 4 أسابيع في موسم النمو.", CareTips = "استخدمي ماء مفلتراً دائماً. رشّي الأوراق لرفع الرطوبة.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 11, Name = "ficus", ArabicName = "نبتة الفيكس", BaseWateringDays = 7, WateringInstructions = "كل 7 أيام — اتركي التربة تجف قليلاً بين الريّات.", SunlightRequirement = "ضوء غير مباشر ساطع.", FertilizingInstructions = "سماد سائل شامل كل 4 أسابيع في موسم النمو.", CareTips = "تتساقط أوراقها عند تحريكها. اختاري مكاناً ثابتاً.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 12, Name = "philodendron", ArabicName = "نبتة الفيلودندرون", BaseWateringDays = 7, WateringInstructions = "كل 7 أيام — 3-4 سم من التربة العلوية تجف بين الريّات.", SunlightRequirement = "ضوء متوسط غير مباشر.", FertilizingInstructions = "سماد غني بالنيتروجين كل 3-4 أسابيع موسم النمو.", CareTips = "سريعة النمو. تتدلّى بشكل جميل من على الرفوف.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 13, Name = "orchid", ArabicName = "زهرة الأوركيد", BaseWateringDays = 7, WateringInstructions = "كل 7 أيام بغمر الإناء في ماء الغرفة 15 دقيقة ثم تصريفه.", SunlightRequirement = "ضوء غير مباشر ساطع.", FertilizingInstructions = "سماد خاص بالأوركيد بنصف الجرعة كل أسبوعين.", CareTips = "لا تتركي الماء على الأوراق. تحتاج فرق حرارة ليلي/نهاري للإزهار.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 14, Name = "succulents", ArabicName = "النباتات العصارية", BaseWateringDays = 14, WateringInstructions = "كل 14 يوماً صيفاً وكل 4-6 أسابيع شتاءً.", SunlightRequirement = "ضوء مباشر لعدة ساعات يومياً.", FertilizingInstructions = "سماد خاص بالصبار مرة في الربيع ومرة في الصيف.", CareTips = "التربة المفككة شرط أساسي. الإفراط في الري هو أول قاتل لها.", IsAiGenerated = false, CreatedAt = seed },
            new PlantLibrary { Id = 15, Name = "boston fern", ArabicName = "سرخس بوسطن", BaseWateringDays = 3, WateringInstructions = "كل 3 أيام — لا تتركيها تجف أبداً.", SunlightRequirement = "ضوء متوسط غير مباشر.", FertilizingInstructions = "سماد سائل متوازن مخفّف كل 4 أسابيع في موسم النمو.", CareTips = "رشّي الأوراق يومياً أو ضعيها بجانب صينية ماء لرفع الرطوبة.", IsAiGenerated = false, CreatedAt = seed }
        );
    }
}