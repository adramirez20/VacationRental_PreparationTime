using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Service.Contract.Response;

namespace VacationRental.Service.Extensions
{
    public class AvailableRentalExt
    {
        public static List<int> GetAvailableRental(DateTime start, int nights , IEnumerable<BookingResponse> bookingsForRental, int blockingDays)
        {
            var occupiedRentalUnits = new List<int>();
            
            foreach (var booking in bookingsForRental)
            {

                var isOccupied = (booking.Start <= start.Date && booking.Start.AddDays(booking.Nights + blockingDays) > start.Date) 
                            || (booking.Start < start.AddDays(nights + blockingDays) && booking.Start.AddDays(booking.Nights) >= start.AddDays(nights + blockingDays)) 
                            || (booking.Start > start && booking.Start.AddDays(booking.Nights + blockingDays) < start.AddDays(nights + blockingDays));

                if (isOccupied)
                {
                    occupiedRentalUnits.Add(booking.Unit);
                }
            }

            return occupiedRentalUnits;
        }


        public static int GetAvailableUnit(List<int> occupiedRentalUnits, int rentalUnits)
        {
            for (int i = 1; i <= rentalUnits; i++)
            {
                if (occupiedRentalUnits?.Where(x => x == i).FirstOrDefault() == 0)
                {
                    return i;
                }
            }

            return 0;
        }
        
    }
}
