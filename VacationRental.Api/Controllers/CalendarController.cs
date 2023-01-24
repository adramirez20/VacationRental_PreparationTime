using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Service.Contract.Request;
using VacationRental.Service.Contract.Response;
using VacationRental.Service.Service.Calendar;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(
            ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            var CResult = _calendarService.GetCalendar(new CalendarRequest
            {
                RentalId = rentalId,
                Start = start,
                Nights = nights,
            });

            var result = new CalendarViewModel
            {
                RentalId = CResult.RentalId
            };
            
            result.Dates = new List<CalendarDateResponse>();

            result.Dates.AddRange(CResult.Dates.Select(c => new CalendarDateViewModel
            {
                Date = c.Date,
                Bookings = c.Bookings
            }).ToList());

            return result;
        }
    }
}
