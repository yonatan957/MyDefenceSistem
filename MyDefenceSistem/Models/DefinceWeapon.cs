using System.ComponentModel.DataAnnotations;

namespace MyDefenceSistem.Models
{
    public class DefinceWeapon
    {
        public int Id { get; set; }
        [Display(Name = "סוג נשק הגנתי")]
        public string Name { get; set; }
        [Display(Name = "כמות")]

        public int quantity { get; set; }
    }
}