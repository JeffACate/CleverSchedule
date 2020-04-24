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
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeocodingService _geocodingService;
        private readonly TravelTimeService _travelTimeService;

        public ClientsController(ApplicationDbContext context, GeocodingService geocodingService, TravelTimeService travelTimeService)
        {
            _context = context;
            _geocodingService = geocodingService;
            _travelTimeService = travelTimeService;
        }

        // GET: Clients
        public IActionResult Index()
        {
            var applicationDbContext = _context.Clients.Include(d => d.IdentityUser);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = _context.Clients.Where(c => c.IdentityUserId == userId)
                .Include(c => c.Address)
                .Include(c => c.IdentityUser)
                .SingleOrDefault();

            if (client == null)
            {
                return RedirectToAction(nameof(Create));
            }
            return RedirectToAction(nameof(Profile));
        }

        // GET: Clients/Details/5
        public IActionResult Profile()
        {
            var applicationDbContext = _context.Clients.Include(d => d.IdentityUser);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = _context.Clients.Where(c => c.IdentityUserId == userId)
                .Include(c => c.Address)
                .Include(c => c.IdentityUser)
                .SingleOrDefault();

            if (client == null)
            {
                return NotFound();
            }

            var appointments = _context.Appointments.Where(a => a.ClientId == client.ClientId)
                .Where(a => a.Status.Equals(Constants.Appointment_Variables.Approved))
                .Where(a => a.DateTime.DayOfYear == DateTime.Today.DayOfYear)
                .Include(a => a.Client)
                .Include(a => a.Contractor)
                .ToList();

            ViewData["Appointments"] = appointments;

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId");
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractorId,Name,AddressId,IdentityUserId")] Client client, [Bind("Street,City,State,Zip")] Address address)
        {

            if (ModelState.IsValid)
            {
                var coords = await _geocodingService.GetCoords(address);

                address.Lat = coords[0];
                address.Lon = coords[1];

                _context.Addresses.Add(address);
                _context.SaveChanges();

                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                client.IdentityUserId = userId;
                client.Address = address;
                client.AddressId = address.AddressId;

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", client.AddressId);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", client.IdentityUserId);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Name,AddressId,IdentityUserId")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "AddressId", client.AddressId);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", client.IdentityUserId);
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Address)
                .Include(c => c.IdentityUser)
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }

        //Get travelTime
        //public async Task<double> CallApi()
        //{
        //    double[] from = new double[] { 43.00708, -88.0041 };
        //    double[] to = new double[] { 42.93541183, -87.97806167 };
        //    double travelTime = await _travelTimeService.GetTravelTime(from,to);
        //    return travelTime;
        //}
    }
}
