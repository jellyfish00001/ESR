using System.Threading.Tasks;
namespace ERS.Application.Contracts.DTO.Eautono
{
    public interface IEautonoService
    {
        Task<string> add(string fromcode, string company, string user);
    }
}