using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CleverScheduleProject.Models
{
    public class Appointment
    {
        public string Status { get; set; }
        public DateTime DateTime { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [ForeignKey("Contractor")]
        public int ContractorId { get; set; }
        public Contractor Contractor { get; set; }

        [NotMapped]
        public static List<SelectListItem> Contractors { get; set; }
        [NotMapped]
        public double DriveTimeToNewAppointment { get; set; }
        [NotMapped]
        public DateTime EndTime { get; set; }
    }
}
