using System.Net;

namespace VacationRental.Service.Contract.Exception
{
    public class ServerException : System.Exception
    {
        private readonly int _httpStatusCode;

        public ServerException(
            HttpStatusCode httpStatusCode,
            string message) : base(message)
        {
            _httpStatusCode = (int)httpStatusCode;
        }

    }
}