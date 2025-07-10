using ERS.IRepositories;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace ERS.Repositories
{
    public class RedisRepository : IRedisRepository, ISingletonDependency
    {
        private IDistributedCache<AutoNoCacheItem> _cache;
        private IConfiguration _configuration;

        public RedisRepository(IDistributedCache<AutoNoCacheItem> cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<bool> AddLockAutoNoAsync(string key)
        {
            //AutoNoCacheItem key = new();
            //key.formcode = form_code;
            bool result = await Setnx(key, key, 30);
            return result;
            //AutoNoCacheItem result = await _cache.GetAsync(key.ToString());
            //if (result == null)
            //    await _cache.SetAsync(key.ToString(), key, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30) });
            //else
            //    return false;
            ////AutoNoCacheItem result = await _cache.GetOrAddAsync(key.ToString(), async ()=> await getDetail(key), () => new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30) });
            //return true;
        }

        async Task<bool> Setnx(string key, string value, int seconds)
        {
            IDatabase db = GetDB();
            bool flag = await db.StringSetAsync(key, value, TimeSpan.FromSeconds(seconds), When.NotExists, CommandFlags.None);
            //if (!flag)
            //{
            //    string _value = db.StringGet(key);
            //    if (string.IsNullOrEmpty(_value))
            //        flag = db.StringSet(key, value, TimeSpan.FromSeconds(seconds), When.NotExists, CommandFlags.None);
            //    else
            //        if (_value == value) flag = true;
            //}
            return flag;
        }
        public async Task DelLockAsync(string key)
        {
            IDatabase db = GetDB();
            await db.KeyDeleteAsync(key);
        }

        IDatabase GetDB()
        {
            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(_configuration.GetSection("Redis:Configuration").Value);
            IDatabase db = conn.GetDatabase(Convert.ToInt32(_configuration.GetSection("Redis:Database").Value));
            return db;
        }

        async Task<AutoNoCacheItem> getDetail(AutoNoCacheItem key) => key;
    }

    public class AutoNoCacheItem
    {
        public string formcode { get; set; }
        public string date { get; } = DateTime.Now.ToString("yyyyMMdd");

        public override string ToString()
        {
            return $"{formcode}_{date}";
        }
    }
}
