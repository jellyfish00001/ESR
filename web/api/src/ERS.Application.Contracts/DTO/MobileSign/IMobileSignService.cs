using System.Threading.Tasks;
namespace ERS.DTO.MobileSign
{
    public interface IMobileSignService
    {
        Task<string> test(string rno);
        Task<Result<string>> recalllForm(string rno);
        Task<Result<string>> SendMobileSignXMLData(string rno);
        Task<Result<string>> recalllFormAll();
    }
}