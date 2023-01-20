using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VacationRental.Service.Contract.Request;
using VacationRental.Service.Contract.Response;
using VacationRental.Service.Service.Booking;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Service.Contract.Exception;

namespace VacationRental.Service.Test.Booking
{
    [TestFixture]
    internal class BookingTest
    {
        [Test]
        [TestCase("bookingData")]
        public void ShouldThrowExceptionWithNullDependencyBookingData(string ExpectedParam)
        {
            Action nullPolicy = () =>
            {
                _ = new BookingService(null, null);
            };

            nullPolicy.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(ExpectedParam);
        }

        [Test]
        [TestCase("rentalData")]
        public void ShouldThrowExceptionWithNullDependencyRentalData(string ExpectedParam)
        {
            Action nullPolicy = () =>
            {
                _ = new BookingService(new ConcurrentDictionary<int, BookingResponse>(), null);
            };

            nullPolicy.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(ExpectedParam);
        }

        [Test]
        [TestCaseSource(nameof(ShouldServerExceptionTestCases))]
        public void ShouldServerException((BookingServiceDouble Policy, BookingRequest Request, int ExpectedResult) testCase)
        {
            testCase.Policy.Add(testCase.Request);
            
            Action IsOccupied = () =>
            {
                BookingService services = new BookingService(testCase.Policy._bookingData, testCase.Policy._rentalData);
                _ = services.Add(testCase.Request);
            };


            IsOccupied.Should().Throw<ServerException>();

        }

        private static IEnumerable<TestCaseData> ShouldServerExceptionTestCases()
        {
            yield return new TestCaseData((
                Policy: new BookingServiceDouble(AvailableBookingResponse()),
                Request: AvailableBookingRequest(),
                ExpectedResult: 1
            )).SetName("SetBookingButIsOccupied");
        }

        [Test]
        [TestCaseSource(nameof(ShouldAddNewBookingTestCases))]
        public void ShouldAddNewBooking((BookingServiceDouble Policy, BookingRequest Request, int ExpectedResult) testCase)
        {
            testCase.Policy.Add(testCase.Request);
            BookingService services = new BookingService(testCase.Policy._bookingData, testCase.Policy._rentalData);
            var response = services.Add(testCase.Request);

            response.Should().Be(testCase.ExpectedResult);

        }

        private static IEnumerable<TestCaseData> ShouldAddNewBookingTestCases()
        {
            yield return new TestCaseData((
                Policy: new BookingServiceDouble(AvailableBookingResponse()),
                Request: AvailableBookingRequest(r => r.Start = new DateTime(2002, 01, 10)),
                ExpectedResult: 2
            )).SetName("SetBookingSameDay");
        }


        [Test]
        [TestCaseSource(nameof(ShouldGoodResponseDataTestCases))]
        public void ShouldGoodResponseData((BookingServiceDouble Policy, BookingResponse ExpectedResult) testCase)
        {

            var response = testCase.Policy.GetByID(1);

            response.Should().BeEquivalentTo(testCase.ExpectedResult);

        }

        private static IEnumerable<TestCaseData> ShouldGoodResponseDataTestCases()
        {
            yield return new TestCaseData((
                Policy: new BookingServiceDouble(AvailableBookingResponse()),
                Response: AvailableBookingResponse()
            )).SetName("GetBookingResponse");
        }

        private static BookingRequest AvailableBookingRequest(Action<BookingRequest> modify = null)
        {
            var result = new BookingRequest
            {
                RentalId = 1,
                Nights = 2,
                Start = new DateTime(2002, 01, 01)
            };

            modify?.Invoke(result);
            return result;
        }

        private static BookingResponse AvailableBookingResponse(Action<BookingResponse> modify = null)
        {
            var result = new BookingResponse
            {
                Id = 1,
                Nights = 2,
                RentalId = 1,
                Start = new DateTime(2002, 01, 01),
                Unit = 1
            };

            modify?.Invoke(result);
            return result;
        }

        public class BookingServiceDouble : IBookingService
        {
            private readonly BookingResponse _bookingResponse;
            public readonly IDictionary<int, BookingResponse> _bookingData;
            public readonly IDictionary<int, RentalResponse> _rentalData;


            internal BookingServiceDouble(BookingResponse bookingResponse)
            {
                _bookingResponse = bookingResponse;
                _bookingData = new Dictionary<int, BookingResponse>();
                _rentalData = new Dictionary<int, RentalResponse>();
            }

            public int Add(BookingRequest bookingModel)
            {
                _bookingData.Add(1, _bookingResponse);
                _rentalData.Add(1, new RentalResponse
                {
                    Id = 1,
                    PreparationDays = 1,
                    Units = 1
                });
                return 1;
            }

            public IDictionary<int, BookingResponse> GetAll()
            {
                IDictionary<int, BookingResponse> books = new Dictionary<int, BookingResponse>();
                books.Add(1, _bookingResponse);
                return books;
            }

            public BookingResponse GetByID(int bookingId)
            {
                return _bookingResponse;
            }
        }
    }
}
