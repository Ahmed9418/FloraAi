using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FloraAI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BaseWateringDays = table.Column<int>(type: "int", nullable: false),
                    WateringInstructions = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SunlightRequirement = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FertilizingInstructions = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CareTips = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsAiGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlantLibraryId = table.Column<int>(type: "int", nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextWateringDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlants_PlantLibrary_PlantLibraryId",
                        column: x => x.PlantLibraryId,
                        principalTable: "PlantLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPlants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosisRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserPlantId = table.Column<int>(type: "int", nullable: true),
                    PlantNameInput = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DiseaseLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArabicDiseaseLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TreatmentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiagnosedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagnosisRecords_UserPlants_UserPlantId",
                        column: x => x.UserPlantId,
                        principalTable: "UserPlants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiagnosisRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PlantLibrary",
                columns: new[] { "Id", "ArabicName", "BaseWateringDays", "CareTips", "CreatedAt", "FertilizingInstructions", "IsAiGenerated", "Name", "SunlightRequirement", "WateringInstructions" },
                values: new object[,]
                {
                    { 1, "نبتة البوثوس", 7, "نبتة مثالية للمبتدئين. تتحمّل الإهمال وتُنقّي الهواء.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد سائل متوازن مرة كل شهر خلال الربيع والصيف.", false, "pothos", "ضوء خافت إلى متوسط.", "كل 7 أيام — اترك التربة تجف جزئياً بين الريّات." },
                    { 2, "نبتة المونستيرا", 7, "امسحي الأوراق بقطعة قماش رطبة شهرياً. تحتاج عمود داعم كلما كبرت.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد سائل غني بالنيتروجين كل 3 أسابيع في موسم النمو.", false, "monstera", "ضوء غير مباشر ساطع.", "كل 7-5 أيام عندما تجف التربة العلوية." },
                    { 3, "نبتة الثعبان", 14, "من أقوى النباتات الداخلية. لا تتركيها في ماء راكد.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد خفيف مرة في الربيع ومرة في الصيف فقط.", false, "snake plant", "تتأقلم مع أي إضاءة.", "كل 14 يوماً صيفاً وكل 3-4 أسابيع شتاءً." },
                    { 4, "زنبق السلام", 7, "تترهّل أوراقها عند العطش وترتفع بعد الري مباشرة.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد سائل شامل كل 6 أسابيع خلال موسم النمو.", false, "peace lily", "ضوء خافت إلى متوسط.", "كل 7 أيام أو عند بدء ترهّل الأوراق قليلاً." },
                    { 5, "نبتة التين كمنجة", 7, "حساسة للتحريك والتيارات الهوائية. اختاري مكاناً ثابتاً.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد سائل للنباتات الورقية كل 4 أسابيع.", false, "fiddle leaf fig", "ضوء غير مباشر ساطع لساعات طويلة.", "كل 7 أيام — تأكّد من جفاف الطبقة العلوية قبل الري." },
                    { 6, "نبتة العنكبوت", 7, "حساسة لمياه الفلور — استخدمي ماء مفلتراً.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد متوازن كل شهرين في موسم النمو.", false, "spider plant", "ضوء متوسط إلى ساطع غير مباشر.", "كل 7 أيام صيفاً وكل 10-14 يوماً شتاءً." },
                    { 7, "نبتة المطاط", 10, "امسحي الأوراق اللامعة بقطعة رطبة. قلّلي الري في الشتاء.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد سائل مخفّف كل 4 أسابيع في موسم النمو.", false, "rubber plant", "ضوء غير مباشر ساطع.", "كل 10 أيام صيفاً وكل أسبوعين شتاءً." },
                    { 8, "صبار الألوفيرا", 14, "تربة مفككة سريعة التصريف. تعفّن الجذور هو أكبر خطر عليها.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد خاص بالصبار مرة في الربيع فقط.", false, "aloe vera", "ضوء مباشر أو غير مباشر ساطع.", "كل 14 يوماً — جفاف كامل للتربة قبل الري." },
                    { 9, "نبتة الزد زد", 14, "بطيئة النمو لكنها قوية جداً. تنمو في أي بيئة تقريباً.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد خفيف مرة كل موسم نمو.", false, "zz plant", "تتأقلم مع الإضاءة الخافتة.", "كل 14 يوماً — تخزّن الماء في جذورها." },
                    { 10, "نبتة الكاليثيا", 5, "استخدمي ماء مفلتراً دائماً. رشّي الأوراق لرفع الرطوبة.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد سائل خفيف الجرعة كل 4 أسابيع في موسم النمو.", false, "calathea", "ضوء خافت إلى متوسط.", "كل 5 أيام — رطوبة منتظمة بدون تشبّع." },
                    { 11, "نبتة الفيكس", 7, "تتساقط أوراقها عند تحريكها. اختاري مكاناً ثابتاً.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد سائل شامل كل 4 أسابيع في موسم النمو.", false, "ficus", "ضوء غير مباشر ساطع.", "كل 7 أيام — اتركي التربة تجف قليلاً بين الريّات." },
                    { 12, "نبتة الفيلودندرون", 7, "سريعة النمو. تتدلّى بشكل جميل من على الرفوف.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد غني بالنيتروجين كل 3-4 أسابيع موسم النمو.", false, "philodendron", "ضوء متوسط غير مباشر.", "كل 7 أيام — 3-4 سم من التربة العلوية تجف بين الريّات." },
                    { 13, "زهرة الأوركيد", 7, "لا تتركي الماء على الأوراق. تحتاج فرق حرارة ليلي/نهاري للإزهار.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد خاص بالأوركيد بنصف الجرعة كل أسبوعين.", false, "orchid", "ضوء غير مباشر ساطع.", "كل 7 أيام بغمر الإناء في ماء الغرفة 15 دقيقة ثم تصريفه." },
                    { 14, "النباتات العصارية", 14, "التربة المفككة شرط أساسي. الإفراط في الري هو أول قاتل لها.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد خاص بالصبار مرة في الربيع ومرة في الصيف.", false, "succulents", "ضوء مباشر لعدة ساعات يومياً.", "كل 14 يوماً صيفاً وكل 4-6 أسابيع شتاءً." },
                    { 15, "سرخس بوسطن", 3, "رشّي الأوراق يومياً أو ضعيها بجانب صينية ماء لرفع الرطوبة.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "سماد سائل متوازن مخفّف كل 4 أسابيع في موسم النمو.", false, "boston fern", "ضوء متوسط غير مباشر.", "كل 3 أيام — لا تتركيها تجف أبداً." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisRecords_UserId",
                table: "DiagnosisRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisRecords_UserPlantId",
                table: "DiagnosisRecords",
                column: "UserPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantLibrary_Name",
                table: "PlantLibrary",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_PlantLibraryId",
                table: "UserPlants",
                column: "PlantLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_UserId",
                table: "UserPlants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiagnosisRecords");

            migrationBuilder.DropTable(
                name: "UserPlants");

            migrationBuilder.DropTable(
                name: "PlantLibrary");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
