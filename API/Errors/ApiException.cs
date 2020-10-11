namespace API.Errors
{
    public class ApiException
    {
        public ApiException(int statusCode, string message = null, string datails = null)
        {
            StatusCode = statusCode;
            Message = message;
            Datails = datails;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Datails { get; set; }
    }
}