using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.Data
{
    public class MyDefenceSistemContext : DbContext
    {
        public MyDefenceSistemContext (DbContextOptions<MyDefenceSistemContext> options)
            : base(options)
        {
            SeedService.Initialize(this);
        }

        public DbSet<MyDefenceSistem.Models.Threat> Threat { get; set; } = default!;
        public DbSet<MyDefenceSistem.Models.DefinceWeapon> DefinceWeapon { get; set; } = default!;
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Origin> Origins { get; set; }
        public DbSet<DefinceWeapon> DefinceWeapons { get; set; }
    }
}
