using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.Data
{
    public class SeedService
    {
        public static void Initialize(MyDefenceSistemContext context)
        {
            context.Database.EnsureCreated();

            // בדוק אם קיימים נתונים בטבלאות
            if (context.Weapons.Any() || context.Origins.Any() || context.DefinceWeapons.Any())
            {
                return;   // DB כבר אותחל
            }

            var definceWeapons = new DefinceWeapon[]
            {
            new DefinceWeapon { Name = "כיפת ברזל", quantity = 10 }
            };

            context.DefinceWeapons.AddRange(definceWeapons);
            context.SaveChanges();

            var weapons = new Weapon[]
            {
            new Weapon { Type = "כטבם", Speed = 300, definceWeaponId = context.DefinceWeapons.First().Id },
            new Weapon { Type = "טיל", Speed = 880, definceWeaponId = context.DefinceWeapons.First().Id },
            new Weapon { Type = "טיל בליסטי", Speed = 18000, definceWeaponId = context.DefinceWeapons.First().Id }
            };

            context.Weapons.AddRange(weapons);
            context.SaveChanges();

            var origins = new Origin[]
            {
            new Origin { Name = "חאמס", Distance = 70 },
            new Origin { Name = "חיזבאללה", Distance = 100 },
            new Origin { Name = "חותים", Distance = 2377 },
            new Origin { Name = "איראן", Distance = 1600 }
            };

            context.Origins.AddRange(origins);
            context.SaveChanges();
        }
    }
}
