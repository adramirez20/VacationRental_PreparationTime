using System;
using System.Collections.Generic;

namespace VacationRental.Service.Contract.Response
{
    public class CalendarDateResponse
    {
        public DateTime Date { get; set; }

        public List<CalendarBookingResponse> Bookings { get; set; }

        public List<int> PreparationTimes { get; set; }
    }
}
