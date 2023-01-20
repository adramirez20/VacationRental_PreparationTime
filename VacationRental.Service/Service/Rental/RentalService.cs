using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using VacationRental.Service.Contract.Exception;
using VacationRental.Service.Contract.Request;
using VacationRental.Service.Contract.Response;
using VacationRental.Service.Extensions;

namespace VacationRental.Service.Service.Rental
{
    public class RentalService : IRentalService
    {
        private readonly IDictionary<int, BookingResponse> _bookingData;
        private readonly IDictionary<int, RentalResponse> _rentalData;

        public RentalService(IDictionary<int, BookingResponse> bookingData, IDictionary<int, RentalResponse> rentalData)
        {
            _bookingData = bookingData ?? throw new ArgumentNullException(nameof(bookingData));
            _rentalData = rentalData ?? throw new ArgumentNullException(nameof(rentalData));
        }

        public RentalResponse GetById(int id)
        {
            return _rentalData.First(x => x.Key == id).Value ?? throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.NotFound, message: "Rental not found");
        }

        public int Add(RentalRequest request)
        {
            ValidateRequest(request);

            var key = _rentalData.Keys.Count == 0 ? 1 : _rentalData.Keys.Last().GetNewInt();

            _rentalData.Add(key, new RentalResponse
            {
                Id = key,
                Units = request.Units,
                PreparationDays = request.PreparationDays
            });

            return key;
        }

        public RentalResponse Update(RentalRequest request)
        {
            int idRental = request.Id ?? throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.BadRequest, message: "Rental id is mandatory");

            ValidateRequest(request);
            ValidateRentalExist(idRental);

            var newObj = new RentalRequest()
            {
                Id = request.Id,
                Units = request.Units,
                PreparationDays = request.PreparationDays
            };

            CanUpdateRental(newObj);

            var rental = _rentalData[idRental];
            rental.Units = request.Units;
            rental.PreparationDays = request.PreparationDays;

            return rental;
        }

        private void ValidateRequest(RentalRequest rental)
        {
            if (rental.Units <= 0)
            {
                throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.BadRequest, message: "Unit number must be positive");
            }
        }

        private void ValidateRentalExist(int rentalId)
        {
            if (!_rentalData.ContainsKey(rentalId))
            {
                throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.NotFound, message: "Rental not found");
            }

        }

        private void CanUpdateRental(RentalRequest rental)
        {
            var rentalBookings = _bookingData.Values.Where(x => x.RentalId == rental.Id).OrderBy(x => x.Start);
            var newBookings = new Dictionary<int, BookingResponse>();

            foreach (var booking in rentalBookings)
            {
                GetAvailableRental(booking, newBookings, rental, out var avalibleRentals);

                if (avalibleRentals == 0)
                {
                    throw new Contract.Exception.ServerException(httpStatusCode: HttpStatusCode.NotModified, message: string.Format("Unable to update rental {0} ", rental.Id));
                }

                newBookings.Add(booking.Id, booking);
            }

        }

        private void GetAvailableRental(BookingResponse newBooking, IDictionary<int, BookingResponse> tempBookingsData, RentalRequest tempRental, out int avalibleRentalsCount)
        {
            avalibleRentalsCount = 0;

            if (tempBookingsData.Values.Count == 0)
            {
                avalibleRentalsCount = 1;
            }

            var ocupiedUnits = new List<int>();
            int blockingDays = tempRental.PreparationDays;

            var bookingsForRental = tempBookingsData.Values.Where(x => x.RentalId == newBooking.RentalId);

            foreach (var booking in bookingsForRental)
            {
                bool isOcupied =
                    (booking.Start <= newBooking.Start.Date && booking.Start.AddDays(booking.Nights + blockingDays) >
                    newBooking.Start.Date) || (booking.Start < newBooking.Start.AddDays(newBooking.Nights + blockingDays) &&
                    booking.Start.AddDays(booking.Nights) >= newBooking.Start.AddDays(newBooking.Nights + blockingDays)) ||
                    (booking.Start > newBooking.Start && booking.Start.AddDays(booking.Nights + blockingDays) <
                    newBooking.Start.AddDays(newBooking.Nights + blockingDays));

                if (isOcupied)
                {
                    ocupiedUnits.Add(booking.Unit);
                }
            }

            bool isAvailable = ocupiedUnits.Count < tempRental.Units;

            if (isAvailable)
            {
                int nextAvailableUnit = this.GetAvailableUnit(ocupiedUnits, tempRental.Units);

                avalibleRentalsCount = nextAvailableUnit;
            }

        }

        private int GetAvailableUnit(List<int> occupiedRentalUnits, int rentalUnits)
        {
            for (int i = 1; i <= rentalUnits; i++)
            {
                if (occupiedRentalUnits.Where(x => x == i).FirstOrDefault() == 0)
                {
                    return i;
                }
            }

            return 0;
        }

    }
}
