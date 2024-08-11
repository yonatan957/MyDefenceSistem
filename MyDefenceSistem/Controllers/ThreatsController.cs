using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;
using MyDefenceSistem.Services;
using static MyDefenceSistem.Models.Enums;

namespace MyDefenceSistem.Controllers
{
    public class ThreatsController : Controller
    {
        private readonly MyDefenceSistemContext _context;
        private readonly ThreatsService _treatService;

        public ThreatsController(MyDefenceSistemContext context, ThreatsService threatsService)
        {
            _context = context;
            _treatService = threatsService;
        }

        // GET: Threats
        public async Task<IActionResult> Index()
        {
            var myDefenceSistemContext = _context.Threat.Include(t => t.Origin).Include(t => t.Weapon).Where(t => t.Status == ThreatStatus.NonActive);
            return View(await myDefenceSistemContext.ToListAsync());
        }

        // GET: Threats/Create
        public IActionResult Create()
        {
            ViewData["OriginId"] = new SelectList(_context.Origins, "Id", "Name");
            ViewData["WeaponId"] = new SelectList(_context.Weapons, "WeaponId", "Type");
            return View();
        }

        // POST: Threats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OriginId,WeaponId,MissleQuantity,Status")] Threat threat)
        {

            if (ModelState.IsValid)
            {
                _context.Add(threat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OriginId"] = new SelectList(_context.Origins, "Id", "Id", threat.OriginId);
            ViewData["WeaponId"] = new SelectList(_context.Weapons, "WeaponId", "Type", threat.WeaponId);
            return View(threat);
        }

        // GET: Threats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var threat = await _context.Threat.FindAsync(id);
            if (threat != null)
            {
                _context.Threat.Remove(threat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Launch(int id)
        {
            if (id == 0 || id == null) 
                return NotFound();
            var threat = _context.Threat.Include(t => t.Weapon).Include(t => t.Origin).FirstOrDefault(t => t.ThreatId == id);
            if (threat == null || threat.Status == ThreatStatus.Done || threat.Status == ThreatStatus.Active)
            {
                return RedirectToAction(nameof(Index), new { Error = "Attack not found" });
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Defence()
        {
            return View();
        }
        private bool ThreatExists(int id)
        {
            return _context.Threat.Any(e => e.ThreatId == id);
        }
    }
}