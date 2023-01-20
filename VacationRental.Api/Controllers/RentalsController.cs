using Microsoft.AspNetCore.Mvc;
using System;
using VacationRental.Api.Models;
using VacationRental.Service.Contract.Request;
using VacationRental.Service.Service.Rental;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService; 

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            var result = _rentalService.GetById(rentalId);
            return new RentalViewModel
            {
                Id = result.Id,
                PreparationDays = result.PreparationDays,
                Units = result.Units
            };
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            try
            {
                return new ResourceIdViewModel
                {
                    Id = _rentalService.Add(new RentalRequest
                    {
                        Units = model.Units,
                        PreparationDays = model.PreparationDays
                    })
                };

            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
           
        }
    }
}
