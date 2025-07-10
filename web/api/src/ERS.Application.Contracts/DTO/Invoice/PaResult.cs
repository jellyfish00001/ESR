namespace ERS.Application.Contracts.DTO.Invoice
{
    public class PaResult<T>
    {
        public string status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public int total { get; set; }
    }
}