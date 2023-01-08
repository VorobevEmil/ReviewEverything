using System.Net;

namespace ReviewEverything.Server.Common.Exceptions
{
    public class HttpStatusRequestException : Exception
    {
        public HttpStatusRequestException(HttpStatusCode statusCode) : base()
        {
            StatusCode = statusCode;
        }

        public HttpStatusRequestException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}