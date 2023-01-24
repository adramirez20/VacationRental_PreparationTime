namespace VacationRental.Service.Contract.Request
{
    public class RentalRequest
    {
        public int? Id { get; set; }

        public int Units { get; set; }

        public int PreparationDays { get; set; }
    }
}
