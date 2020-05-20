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
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TravelTimeService _travelTimeService;

        public AppointmentsController(ApplicationDbContext context, TravelTimeService travelTimeService)
        {
            _context = context;
            _travelTimeService = travelTimeService;
            _travelTimeService = travelTimeService;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.Client).Include(a => a.Contractor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(DateTime? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Contractor)
                .FirstOrDefaultAsync(m => m.DateTime == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult CreateAppointment()
        {
            
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId");
            ViewData["Contractors"]= new SelectList(_context.Contractors, "ContractorsId", "ContractorId");
            return View();
        }

        //CREATE APT


        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment([Bind("DateTime,ContractorId")] Appointment appointment)
        {

            if (ModelState.IsValid)
            {
                var applicationDbContext = _context.Clients.Include(c => c.IdentityUser);
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var client = _context.Clients.Where(c => c.IdentityUserId == userId)
                    .Include(c => c.Address)
                    .Include(c => c.IdentityUser)
                    .SingleOrDefault();
                appointment.ClientId = client.ClientId;
                appointment.Status = Constants.Appointment_Variables.Pending;
                try
                {
                    _context.Appointments.Add(appointment);
                }
                catch(Exception e)
                {
                    RedirectToAction(nameof(InvalidAppointment), appointment);
                }
                // Comment code: 140803 check appointment availability

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CheckAvailability),appointment);
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId", appointment.ClientId);
            ViewData["ContractorId"] = new SelectList(_context.Contractors, "Name", "ContractorId", appointment.ContractorId);
            return View(appointment);
        }

        public async Task<IActionResult> CheckAvailability(Appointment appointmentToConfirm)
        {
            return RedirectToAction(nameof(AppointmentConfirmed), appointmentToConfirm);
            // Comment code: 140804
            bool appointmentAvailable = false; //appointment unavailable at first

            var contractor = _context.Contractors.Where(c => c.ContractorId == appointmentToConfirm.ContractorId) // Contractor Address
                    .Include(c => c.Address)
                    .SingleOrDefault();

            var startAddress = contractor.Address; // Starting Address = Contractor Address at first

            List<Appointment> appointmentsToday = _context.Appointments.Where(a => a.DateTime.Date == DateTime.Today) 
                .Include(a => a.Client)
                .Include(a => a.Client.Address)
                .ToList();

            List<Appointment> availableAppointments = new List<Appointment>();
            
            foreach (var appointment in appointmentsToday)
            {
                
            }
            startAddress = contractor.Address;
            foreach(var appointment in appointmentsToday)
            {

            }
            appointmentAvailable = true;
            if(appointmentAvailable)
            {
                return RedirectToAction(nameof(AppointmentConfirmed), appointmentToConfirm);
            }
            else
            {
                return RedirectToAction(nameof(SuggestAlternate), appointmentToConfirm);
            }
        }

        public IActionResult SuggestAlternate(Appointment appointmentToSuggest)
        {
            // Get list of available appointments list of appointments where distanceToNext < 
            // Send list in ViewData
            return View(appointmentToSuggest);
        }
        public async Task<IActionResult> AppointmentConfirmed(Appointment appointment)
        {
            var confirmedAppointment = _context.Appointments.Where(a => a.ContractorId == appointment.ContractorId && a.DateTime == appointment.DateTime).SingleOrDefault();
            confirmedAppointment.Status = Constants.Appointment_Variables.Approved;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),"Clients");
            //return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(DateTime? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId", appointment.ClientId);
            ViewData["ContractorId"] = new SelectList(_context.Contractors, "ContractorId", "ContractorId", appointment.ContractorId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DateTime id, [Bind("Status,DateTime,ClientId,ContractorId")] Appointment appointment)
        {
            if (id != appointment.DateTime)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.DateTime))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId", appointment.ClientId);
            ViewData["ContractorId"] = new SelectList(_context.Contractors, "Name", "Name", appointment.Contractor.Name);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(DateTime? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Contractor)
                .FirstOrDefaultAsync(m => m.DateTime == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DateTime id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(DateTime id)
        {
            return _context.Appointments.Any(e => e.DateTime == id);
        }
    }
}
