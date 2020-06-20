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
        public async Task<IActionResult> CreateAppointment([Bind("DateTime,ContractorId")] Appointment newAppointment)
        {
            if (ModelState.IsValid)
            {
                //Appointment newAppointment = incomingAppointment;
                var applicationDbContext = _context.Clients.Include(c => c.IdentityUser);
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Client client = _context.Clients.Where(c => c.IdentityUserId == userId)
                    .Include(c => c.Address)
                    .Include(c => c.IdentityUser)
                    .SingleOrDefault();
                newAppointment.ClientId = client.ClientId;
                newAppointment.Status = Constants.Appointment_Variables.Pending;
                newAppointment.EndTime = newAppointment.DateTime; // + 1 hour

                // if: the appointment is in the db, do not add and redirect.
                List<Appointment> allAppointments = _context.Appointments.Where(a => a.DateTime.DayOfYear == newAppointment.DateTime.DayOfYear).ToList(); ;

                if (newAppointment.DateTime.Ticks < DateTime.Now.Ticks)
                {
                    return RedirectToAction(nameof(InvalidAppointment), newAppointment);
                }
                foreach (var appointment in allAppointments)
                {
                    if (appointment.DateTime.Ticks == newAppointment.DateTime.Ticks)
                    {
                        return RedirectToAction(nameof(InvalidAppointment), newAppointment);
                    }
                }
                // else: add appointment to db with Pending status
                _context.Appointments.Add(newAppointment);

                // Comment code: 140803 check appointment availability
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CheckAvailability), newAppointment);
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "ClientId", newAppointment.ClientId);
            ViewData["ContractorId"] = new SelectList(_context.Contractors, "Name", "ContractorId", newAppointment.ContractorId);
            return View(newAppointment);
        }

        public async Task<IActionResult> CheckAvailability(Appointment appointmentToConfirm)
        {
            // Comment code: 140804
            bool appointmentAvailable = true; //appointment unavailable at first

            Contractor contractor = _context.Contractors.Where(c => c.ContractorId == appointmentToConfirm.ContractorId) // Contractor Address
                    .Include(c => c.Address)
                    .SingleOrDefault();

            Address startAddress = contractor.Address; // Starting Address = Contractor Address at first

            List<Appointment> appointmentsToday = _context.Appointments.Where(a => a.DateTime.Date == DateTime.Today && a.Status == Constants.Appointment_Variables.Approved) 
                .Include(a => a.Client)
                .Include(a => a.Client.Address)
                .Include(a => a.Contractor)
                .Include(a => a.Contractor.Address)
                .ToList();

            appointmentToConfirm.Client = _context.Clients.Where(c => c.ClientId == appointmentToConfirm.ClientId).SingleOrDefault();

            foreach (Appointment appointment in appointmentsToday)
            {
                //appointment.EndTime = appointment.DateTime + 1 hour
                appointment.DriveTimeToNewAppointment = await _travelTimeService.GetTravelTime(appointment.Client.Address, appointmentToConfirm.Client.Address);
            }

            throw new NotImplementedException();
            appointmentAvailable = CheckAllAppointments(appointmentToConfirm);

            if(appointmentAvailable) 
            {
                return RedirectToAction(nameof(AppointmentConfirmed), appointmentToConfirm); 
            }
            else 
            {
                appointmentToConfirm.Status = Constants.Appointment_Variables.Denied;
                return RedirectToAction(nameof(SuggestAlternate), new Appointment() ); 
            }
        }
        private bool CheckAllAppointments(Appointment appointmentToConfirm)
        {
            bool appointmentAvailable = false;

            /* 
                for loop
                    if (
                        (Appointment[i].EndTime + Appointment[i].DriveTimeToNewAppointment) < appointmentToConfirm &&
                        (Appointment[i+1].EndTime + Appointment[i+1].DriveTimeToAppointment) < appointmentToConfirm &&
                        (Appointment.DateTime > Appointment[i].EndTime
                       )
                        {
                            appointmentAvailable == true;
                            break;
                        }
            */

            return appointmentAvailable;
        }
        public IActionResult InvalidAppointment(Appointment invalidAppointment)
        {
            invalidAppointment.Status = Constants.Appointment_Variables.Denied;
            Contractor contractor =  _context.Contractors.Where(c => c.ContractorId == invalidAppointment.ContractorId).SingleOrDefault();
            ViewData["Contractor"] = contractor.Name;
            Client client = _context.Clients.Where(cl => cl.ClientId == invalidAppointment.ClientId).SingleOrDefault();
            ViewData["Client"] = client.Name;
            return View(invalidAppointment);
        }
        public IActionResult SuggestAlternate(Appointment appointmentToSuggest)
        {
            // Get list of available appointments list of appointments where distanceToNext < 
            // Send list in ViewData
            Contractor contractor = _context.Contractors.Where(c => c.ContractorId == appointmentToSuggest.ContractorId).SingleOrDefault();
            ViewData["Contractor"] = contractor.Name;
            Client client = _context.Clients.Where(cl => cl.ClientId == appointmentToSuggest.ClientId).SingleOrDefault();
            ViewData["Client"] = client.Name;
            return View(appointmentToSuggest);
        }
        public async Task<IActionResult> AppointmentConfirmed(Appointment approvedAppointment)
        {
            approvedAppointment = _context.Appointments.Where(a => a.DateTime == approvedAppointment.DateTime).SingleOrDefault();
            approvedAppointment.Status = Constants.Appointment_Variables.Approved;
            await _context.SaveChangesAsync();
            Contractor contractor = _context.Contractors.Where(c => c.ContractorId == approvedAppointment.ContractorId).SingleOrDefault();
            ViewData["Contractor"] = contractor.Name;
            Client client = _context.Clients.Where(cl => cl.ClientId == approvedAppointment.ClientId).SingleOrDefault();
            ViewData["Client"] = client.Name;
            return View(approvedAppointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(DateTime? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Appointment appointment = await _context.Appointments.FindAsync(id);
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
