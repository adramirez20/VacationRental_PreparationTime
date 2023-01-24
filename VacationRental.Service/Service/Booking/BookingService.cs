using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using VacationRental.Service.Contract.Exception;
using VacationRental.Service.Contract.Request;
using VacationRental.Service.Contract.Response;
using VacationRental.Service.Extensions;

namespace VacationRental.Service.Service.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IDictionary<int, BookingResponse> _bookingData;
        private readonly IDictionary<int, RentalResponse> _rentalData;

        public BookingService(IDictionary<int, BookingResponse> bookingData, IDictionary<int, RentalResponse> rentalData)
        {
            _bookingData = bookingData ?? throw new ArgumentNullException(nameof(bookingData));
            _rentalData = rentalData ?? throw new ArgumentNullException(nameof(rentalData));
        }

        public BookingResponse GetByID(int id)
        {
            return _bookingData.First(x => x.Key == id).Value ?? throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.NotFound, message: "Booking not found");
        }

        public IDictionary<int, BookingResponse> GetAll()
        {
            return _bookingData;
        }

        public int Add(BookingRequest request)
        {
            ValidateRequest(request);

            GetAvailableRental(request, out var unitResult);

            if (unitResult == 0)
            {
                throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.Conflict, message: "is Occupied");
            }

            var key = _bookingData.Keys.Count == 0 ? 1: _bookingData.Keys.Last().GetNewInt();

            _bookingData.Add(key, new BookingResponse
            {
                Id = key,
                Nights = request.Nights,
                RentalId = request.RentalId,
                Unit = unitResult,
                Start = request.Start.Date
            });

            return key;
        }

        private void ValidateRequest(BookingRequest booking)
        {
            if (booking.Nights <= 0)
            {
                throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.BadRequest, message: "Nigts must be positive");
            }

            if (_rentalData.Select(r => r.Key == booking.RentalId) == null)
            {
                throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.NotFound, message: "Rental not found");
            }
        }

        private void GetAvailableRental(BookingRequest newBooking, out int avalibleRentals)
        {
            avalibleRentals = 0;

            if (_bookingData.Count == 0)
            {
                avalibleRentals = 1;
            }
           
            
            RentalResponse rental = _rentalData.First(r => r.Key == newBooking.RentalId).Value;
            var blockingDays = rental.PreparationDays;

            var bookingsForRental = _bookingData.Values.Where(x => x.RentalId == newBooking.RentalId);

            var occupiedRentalUnits = AvailableRentalExt.GetAvailableRental(newBooking.Start, newBooking.Nights, bookingsForRental, blockingDays);

            if (occupiedRentalUnits.Count < rental.Units)
            {
                int nextAvailableUnit = AvailableRentalExt.GetAvailableUnit(occupiedRentalUnits, rental.Units);

                avalibleRentals = nextAvailableUnit;
            }
        }

    }
}
