namespace FaRequestLoggingApi.Models
{
    public class RequestLog
    {
        public string TransactionId { get; set; }
        public DateTime LogTime { get; set; }
        public int TemplateId { get; set; }
        public string Environment { get; set; }
        public string ServiceName { get; set; }
        public string MachineName { get; set; }
        public string LogType { get; set; }
        public string ServiceRequestStatus { get; set; }
        public string RequestContent { get; set; }
        public string ResponseContent { get; set; }
        public string ExceptionContent { get; set; }
    }
}
