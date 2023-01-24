using System;
using System.Collections.Generic;
using System.Text;
using VacationRental.Service.Contract.Request;
using VacationRental.Service.Contract.Response;

namespace VacationRental.Service.Service.Booking
{
    public interface IBookingService
    {
        BookingResponse GetByID(int bookingId);

        int Add(BookingRequest bookingModel);

        IDictionary<int, BookingResponse> GetAll();

        
    }
}
