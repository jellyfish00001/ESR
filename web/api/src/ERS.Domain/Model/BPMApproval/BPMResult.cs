namespace ERS.Model
{
    public class BPMResult<T>
    {
        public string msg { get; set; }
        public string status { get; set; }
        public string info { get; set; }
        public int? total { get; set; }
        public T data { get; set; }
    }
}
