using VacationRental.Service.Contract.Request;
using VacationRental.Service.Contract.Response;

namespace VacationRental.Service.Service.Rental
{
    public interface IRentalService
    {
        int Add(RentalRequest request);

        RentalResponse GetById(int rentalId);
    }
}
