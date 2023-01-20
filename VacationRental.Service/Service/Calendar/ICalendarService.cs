using VacationRental.Service.Contract.Request;
using VacationRental.Service.Contract.Response;

namespace VacationRental.Service.Service.Calendar
{
    public interface ICalendarService
    {
        CalendarResponse GetCalendar(CalendarRequest request);

    }
}
