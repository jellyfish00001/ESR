namespace ERS.DTO
{
    public class Result<T>
    {
        public int status { get; set; } = 1;
        public string message { get; set; }
        public T data { get; set; }
        public int total { get; set; }
    }
}
