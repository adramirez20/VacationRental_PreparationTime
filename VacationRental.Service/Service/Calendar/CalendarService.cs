using System.Collections.Generic;
using System.Linq;
using System.Net;
using VacationRental.Service.Contract.Exception;
using VacationRental.Service.Contract.Request;
using VacationRental.Service.Contract.Response;
using VacationRental.Service.Service.Booking;
using VacationRental.Service.Service.Rental;

namespace VacationRental.Service.Service.Calendar
{
    public class CalendarService : ICalendarService
    {
        private readonly IRentalService _rentalService;
        private readonly IBookingService _bookingService;

        public CalendarService(IRentalService rentalService, IBookingService bookingService)
        {
            _rentalService = rentalService;
            _bookingService = bookingService;
        }

        public CalendarResponse GetCalendar(CalendarRequest request)
        {
            ValidateRequest(request, out var rental);

            var calendar = new CalendarResponse
            {
                RentalId = rental.Id,
                Dates = new List<CalendarDateResponse>(),
            };

            for (var i = 0; i < request.Nights; i++)
            {
                var dateCalendar = new CalendarDateResponse
                {
                    Date = request.Start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingResponse>()
                };

                GetCalendarDateBookings(calendar, dateCalendar, rental);
            }

            return calendar;
        }

        private void ValidateRequest(CalendarRequest request, out RentalResponse rental)
        {
            if (request.Nights < 0)
            {
                throw new ServerException(httpStatusCode: HttpStatusCode.BadRequest, message: "Nights must be greater than zero");
            }

            rental = _rentalService.GetById(request.RentalId);

            if (rental == null)
            {
                throw new ServerException(httpStatusCode: HttpStatusCode.NotFound, message: "Rental not found");
            }

        }

        private void GetCalendarDateBookings(CalendarResponse calendar, CalendarDateResponse dateCalendar, RentalResponse rental)
        {
            IEnumerable<BookingResponse> rentalBookings = _bookingService.GetAll().Values.Where(x => x.RentalId == calendar.RentalId).ToList();
            int blockingDays = rental.PreparationDays;

            foreach (var booking in rentalBookings)
            {
                if (booking.Start <= dateCalendar.Date && booking.Start.AddDays(booking.Nights + blockingDays) > dateCalendar.Date)
                {
                    dateCalendar.Bookings.Add(new CalendarBookingResponse()
                    {
                        Id = booking.Id,
                        Unit = booking.Unit
                    });
                }
            }

            dateCalendar.PreparationTimes = this.GetAvailableUnits(
                totalUnits: rental.Units,
                dateCalendar: dateCalendar);

            calendar.Dates.Add(dateCalendar);
        }

        private List<int> GetAvailableUnits(int totalUnits, CalendarDateResponse dateCalendar)
        {
            var result = new List<int>();

            for (var i = 1; i <= totalUnits; i++)
            {
                if (dateCalendar.Bookings.Where(x => x.Unit == i) == null)
                {
                    result.Add(i);
                }
            }

            return result;
        }

    }
}
