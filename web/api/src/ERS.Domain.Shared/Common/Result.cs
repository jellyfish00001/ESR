
namespace ERS.Common
{
    public class Result<T>
    {
        public int status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }
}
