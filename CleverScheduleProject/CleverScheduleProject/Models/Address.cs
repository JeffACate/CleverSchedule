using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleverScheduleProject.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
    }
}