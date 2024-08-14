using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;
using MyDefenceSistem.BL;
using static MyDefenceSistem.Models.Enums;

namespace MyDefenceSistem.Controllers
{
    public class ThreatsController : Controller
    {
        // TO DO!!!!!!!!: private readonly IThreatsService _treatService;  replace after refactor 
        private readonly IThreatsService _treatService;
        private readonly IWeaponsService _weaponsService;
        private readonly IoriginService _ioriginService;
        public ThreatsController( IThreatsService threatsService, IoriginService ioriginService, IWeaponsService weaponsService)
        {         
            _treatService = threatsService;
            _ioriginService = ioriginService;
            _weaponsService = weaponsService;
        }

        // GET: Threats
        public async Task<IActionResult> Index()
        {
            var myDefenceSistemContext = await _treatService.GetNOnActiveThreats();
            return View(myDefenceSistemContext);
        }

        // GET: Threats/Create
        public async Task<IActionResult> Create()
        {
            ViewData["OriginId"] = new SelectList(await _ioriginService.GetOriginListAsync(), "Id", "Name");
            ViewData["WeaponId"] = new SelectList(await _weaponsService.GetWeaponsListAsync(), "WeaponId", "Type");
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
                await _treatService.CreateThreat(threat);
                return RedirectToAction(nameof(Index));
            }
            return View(threat);
        }

        // GET: Threats/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            await _treatService.DeleteThreat(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Launch(int id)
        {
            if (id == 0 || id == null) 
                return NotFound();
            var Sucsses = await _treatService.Launch(id);
            if (!Sucsses)
            {
                return RedirectToAction(nameof(Index), new { Error = "Attack not found" });
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Defence()
        {
            return View();
        }
        private async Task<bool> ThreatExists(int id)
        {
            return await _treatService.ThreatExist(id);
        }
    }
}