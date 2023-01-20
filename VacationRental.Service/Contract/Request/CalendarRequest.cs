using System;

namespace VacationRental.Service.Contract.Request
{
    public class CalendarRequest
    {
        public int RentalId { get; set; }

        public DateTime Start{ get; set; }

        public int Nights{ get; set; }
    }
}
