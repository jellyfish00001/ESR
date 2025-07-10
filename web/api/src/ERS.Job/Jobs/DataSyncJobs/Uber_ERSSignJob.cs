using ERS.IDomainServices;
using ERS.Job.Util;
using ERS.UberToERS;

namespace ERS.Job.Jobs.DataSyncJobs
{
    public class Uber_ERSSignJob : IJobBase
    {
        //private IUberToERSRepository _uberToERSRepository;

        //private IUberDomainService _uberDomainService;

        //public Uber_ERSSignJob(IUberToERSRepository uberToERSRepository,IUberDomainService uberDomainService)
        //{
        //    _uberToERSRepository = uberToERSRepository;
        //    _uberDomainService = uberDomainService;
        //}
        //public async Task Run()
        //{
        //    //_uberToERSRepository.TransactionToSign();
        //    await _uberDomainService.TransactionToSign();
        //    //await Task.CompletedTask;
        //}
        private readonly IServiceProvider _serviceProvider;

        public Uber_ERSSignJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Run()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var uberDomainService = scope.ServiceProvider.GetRequiredService<IUberDomainService>();
                await uberDomainService.TransactionToSign();
            }
        }
    }
}
