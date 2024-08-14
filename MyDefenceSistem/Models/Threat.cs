using System.ComponentModel.DataAnnotations;
using static MyDefenceSistem.Models.Enums;

namespace MyDefenceSistem.Models
{
    public class Threat
    {
        [Key]
        public int ThreatId { get; set; }

        [Required]
        [Display(Name = "מקום שיגור")]
        public int OriginId { get; set; }

        public Origin? Origin { get; set; }


        [Display(Name = "זמן שיגור")]
        public DateTime? LaunchTime { get; set; }


        [Display(Name = "סוג נשק")]
        public int WeaponId { get; set; }

        public Weapon? Weapon { get; set; }


        [Display(Name = "כמות טילים")]
        public int MissleQuantity { get; set; }


        [Display(Name = "כמות טילים שיורטו")]
        public int? hitted { get; set; }

        public int ? TimeToHit { get; set; }

        [Display(Name = "מצב האיום")]
        public ThreatStatus Status { get; set; }
    }
}
