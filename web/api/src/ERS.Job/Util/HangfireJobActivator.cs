using Hangfire;

namespace ERS.Job.Util
{
    public class HangfireJobActivator : JobActivator
    {
        readonly IServiceScopeFactory _serviceScopeFactory;
        public HangfireJobActivator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            return new HangfireJobActivatorScope(_serviceScopeFactory.CreateScope());
        }
    }

    public class HangfireJobActivatorScope : JobActivatorScope
    {
        readonly IServiceScope _serviceScope;
        public HangfireJobActivatorScope(IServiceScope serviceScope)
        {
            if (serviceScope == null) throw new ArgumentNullException(nameof(serviceScope));
            this._serviceScope = serviceScope;
        }

        public override object Resolve(Type type)
        {
            return this._serviceScope.ServiceProvider.GetService(type);
        }

        public override void DisposeScope()
        {
            this._serviceScope.Dispose();
        }
    }
}