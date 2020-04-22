using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CleverScheduleProject.Data;
using CleverScheduleProject.Models;
using System.Security.Claims;
using CleverScheduleProject.Library;

namespace CleverScheduleProject.Controllers
{

    public class ContractorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeocodingService _geocodingService;

        public ContractorsController(ApplicationDbContext context, GeocodingService geocodingService)
        {
            _context = context;
            _geocodingService = geocodingService;
        }

        // GET: Contractors
        public IActionResult Index()
        {
            var applicationDbContext = _context.Contractors.Include(c => c.IdentityUser);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var contractor = _context.Contractors.Where(c => c.IdentityUserId == userId)
                .Include(c => c.Address)
                .Include(c => c.IdentityUser)
                .SingleOrDefault();
            
            if (contractor == null)
            {
                return RedirectToAction(nameof(Create));
            }
            return RedirectToAction(nameof(Schedule));
        }

        // GET: Contractors/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else 
            {
                var contractor = _context.Contractors.Where(c => c.ContractorId == id)
                .Include(c => c.Address)
                .Include(c => c.IdentityUser)
                .SingleOrDefault();

                if (contractor == null)
                {
                    return NotFound();
                }
                return View(contractor);
            }
        }

        // GET: Contractors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contractors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractorId,Name,AddressId,IdentityUserId")] Contractor contractor, [Bind("Street,City,State,Zip")] Address address)
        {

            if (ModelState.IsValid)
            {
                var coords = await _geocodingService.GetCoords(address); 

                address.Lat = coords[0];
                address.Lon = coords[1];

                _context.Addresses.Add(address);
                _context.SaveChanges();

                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                contractor.IdentityUserId = userId;
                contractor.Address = address;
                contractor.AddressId = address.AddressId;

                _context.Contractors.Add(contractor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contractor);
        }

        public async Task<IActionResult> Schedule()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Contractor)
                .Where(a => a.Contractor.IdentityUserId == userId);
            return View(await appointments.ToListAsync());
        }
        // GET: Contractors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractor = await _context.Contractors.FindAsync(id);
            if (contractor == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", contractor.AddressId);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", contractor.IdentityUserId);
            return View(contractor);
        }

        // POST: Contractors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractorId,Name,AddressId,IdentityUserId")] Contractor contractor)
        {
            if (id != contractor.ContractorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contractor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractorExists(contractor.ContractorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", contractor.AddressId);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", contractor.IdentityUserId);
            return View(contractor);
        }

        // GET: Contractors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractor = await _context.Contractors
                .Include(c => c.Address)
                .Include(c => c.IdentityUser)
                .FirstOrDefaultAsync(m => m.ContractorId == id);
            if (contractor == null)
            {
                return NotFound();
            }

            return View(contractor);
        }

        // POST: Contractors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contractor = await _context.Contractors.FindAsync(id);
            _context.Contractors.Remove(contractor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractorExists(int id)
        {
            return _context.Contractors.Any(e => e.ContractorId == id);
        }
    }
}
