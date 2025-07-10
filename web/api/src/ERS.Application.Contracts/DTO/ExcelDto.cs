using System.Collections.Generic;
namespace ERS.DTO
{
    public class ExcelDto<T>
    {
        public string[] header { get; set; }
        public List<T> body { get; set; }
    }
}