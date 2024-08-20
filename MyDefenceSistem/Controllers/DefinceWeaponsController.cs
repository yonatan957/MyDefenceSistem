using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.BL;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.Controllers
{
    public class DefinceWeaponsController : Controller
    {
        private readonly IDefenceWeaponService _defenceWeaponService;

        public DefinceWeaponsController(IDefenceWeaponService defenceWeaponService)
        {
            _defenceWeaponService = defenceWeaponService;
        }

        // GET: DefinceWeapons
        public async Task<IActionResult> Index()
        {
            return View(await _defenceWeaponService.GetTable());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var sucsses = await _defenceWeaponService.UpdateQuantity(id, quantity);
            if (!sucsses) return BadRequest();
            return RedirectToAction(nameof(Index));
        }
        

    }
}
