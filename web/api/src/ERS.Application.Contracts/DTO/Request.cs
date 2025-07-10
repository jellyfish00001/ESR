namespace ERS.DTO
{
    public class Request<T>
    {
        public int pageIndex { get; set; } = 1;// 当前页数
        public int pageSize { get; set; } = 10;// 每页记录数
        public T data { get; set; }
    }
}