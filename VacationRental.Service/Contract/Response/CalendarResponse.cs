using System.Collections.Generic;

namespace VacationRental.Service.Contract.Response
{
    public class CalendarResponse
    {
        public int RentalId { get; set; }

        public List<CalendarDateResponse> Dates { get; set; }
    }
}
