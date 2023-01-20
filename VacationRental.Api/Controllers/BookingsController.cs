using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Service.Contract.Request;
using VacationRental.Service.Service.Booking;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            var result = _bookingService.GetByID(bookingId);
            return new BookingViewModel
            {
                Id = result.Id,
                RentalId = result.RentalId,
                Start = result.Start,
                Nights = result.Nights,
                Unit = result.Unit
            };
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            try
            {
                return new ResourceIdViewModel
                {
                    Id = _bookingService.Add(new BookingRequest
                    {
                        Nights = model.Nights,
                        RentalId = model.RentalId,
                        Start = model.Start
                    })
                };

            }
            catch(Exception e)
            {
                throw new ApplicationException(e.Message);
            }

        }
    }
}
