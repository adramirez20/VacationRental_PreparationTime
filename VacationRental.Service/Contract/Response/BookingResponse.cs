using System;

namespace VacationRental.Service.Contract.Response
{
    public class BookingResponse
    {
        public int Id { get; set; }

        public int RentalId { get; set; }

        public DateTime Start { get; set; }

        public int Unit { get; set; }

        public int Nights { get; set; }
    }
}
