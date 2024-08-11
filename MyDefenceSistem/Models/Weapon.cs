using static MyDefenceSistem.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyDefenceSistem.Models
{
    public class Weapon
    {
        [Key]
        public int WeaponId { get; set; }
        [Required]
        public string Type { get; set; }
        public int Speed { get; set; }
        public int definceWeaponId { get; set; }
        public DefinceWeapon definceWeapon { get; set; }
    }
}
