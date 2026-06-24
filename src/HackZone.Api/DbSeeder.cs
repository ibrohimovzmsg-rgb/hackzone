using HackZone.Domain.Entities;
using HackZone.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace HackZone.Api;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<ApplicationDbContext>();

        // Roles
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(
                Role.Create(Role.Names.Student, "Talabalar"),
                Role.Create(Role.Names.Instructor, "O'\''qituvchilar"),
                Role.Create(Role.Names.Admin, "Administratorlar")
            );
            await db.SaveChangesAsync();
        }

        // Admin user
        if (!await db.Users.AnyAsync(u => u.Username == "admin"))
        {
            var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == Role.Names.Admin);
            var studentRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == Role.Names.Student);

            var admin = User.Create("admin", "admin@hackzone.uz", BC.HashPassword("Admin@123456"));
            admin.ConfirmEmail();
            admin.AddRole(adminRole!.Id);
            admin.AddRole(studentRole!.Id);
            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }

        // Forum categories
        if (!await db.ForumCategories.AnyAsync())
        {
            db.ForumCategories.AddRange(
                ForumCategory.Create("Umumiy muhokama", "Barcha mavzular", 1),
                ForumCategory.Create("CTF yechimlar", "CTF challenge yechimlarini ulashing", 2),
                ForumCategory.Create("Lab yordam", "Lab mashqlarda yordam so'\''rang", 3),
                ForumCategory.Create("Kurs savollari", "Kurs bo'\''yicha savollar", 4)
            );
            await db.SaveChangesAsync();
        }

        // Sample CTF challenges
        if (!await db.CtfChallenges.AnyAsync())
        {
            var challenges = new[]
            {
                CtfChallenge.Create("SQL Injection Asoslari", "Oddiy SQL injection zaifligini toping.",
                    Domain.Enums.ChallengeCategory.Web, Domain.Enums.Difficulty.Easy, 100,
                    BC.HashPassword("HZ{sql_1nj3ct10n_bas1cs}")),
                CtfChallenge.Create("XSS Topish", "Cross-site scripting zaifligini toping va exploit qiling.",
                    Domain.Enums.ChallengeCategory.Web, Domain.Enums.Difficulty.Easy, 150,
                    BC.HashPassword("HZ{xss_r3fl3ct3d}")),
                CtfChallenge.Create("Caesar Cipher", "Shifrlangan xabarni hal qiling.",
                    Domain.Enums.ChallengeCategory.Crypto, Domain.Enums.Difficulty.Easy, 100,
                    BC.HashPassword("HZ{c4es4r_c1ph3r_d3c0d3d}")),
                CtfChallenge.Create("Buffer Overflow", "Stack buffer overflow zaifligini toping.",
                    Domain.Enums.ChallengeCategory.Pwn, Domain.Enums.Difficulty.Medium, 300,
                    BC.HashPassword("HZ{buff3r_0v3rfl0w_pwn3d}"))
            };
            foreach (var c in challenges) c.Publish();
            db.CtfChallenges.AddRange(challenges);
            await db.SaveChangesAsync();
        }

        // Sample courses
        if (!await db.Courses.AnyAsync())
        {
            var course = Course.Create(
                "Veb Xavfsizligi Asoslari",
                "OWASP Top 10 va asosiy veb zaifliklarni o'\''rganing.",
                "Web Security",
                Domain.Enums.Difficulty.Easy);
            course.Publish();
            db.Courses.Add(course);
            await db.SaveChangesAsync();

            var lessons = new[]
            {
                Lesson.Create(course.Id, "SQL Injection nima?", "# SQL Injection\n\nSQL Injection...", 1, 20),
                Lesson.Create(course.Id, "XSS hujumlari", "# Cross-Site Scripting\n\nXSS...", 2, 25),
                Lesson.Create(course.Id, "CSRF himoyasi", "# CSRF\n\nCSRF token...", 3, 20)
            };
            foreach (var l in lessons) l.Publish();
            db.Lessons.AddRange(lessons);
            await db.SaveChangesAsync();
        }
    }
}
