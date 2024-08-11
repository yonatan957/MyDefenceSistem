using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.Controllers
{
    public class DefinceWeaponsController : Controller
    {
        private readonly MyDefenceSistemContext _context;

        public DefinceWeaponsController(MyDefenceSistemContext context)
        {
            _context = context;
        }

        // GET: DefinceWeapons
        public async Task<IActionResult> Index()
        {
            return View(await _context.DefinceWeapon.ToListAsync());
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var item = _context.DefinceWeapons.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            item.quantity = quantity;
            _context.Update(item);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        
        private bool DefinceWeaponExists(int id)
        {
            return _context.DefinceWeapon.Any(e => e.Id == id);
        }
    }
}
