using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Redis.Store
{
    public class RedisStore<T> where T : IStoreEntry
    {
        private readonly RedisClient _redisClient;
        private readonly IDatabase _db;

        public RedisStore(RedisClient redisClient)
        {
            _redisClient = redisClient;
            _db = redisClient.GetDB();
        }

        public async Task<T> Get(string key)
        {
            var res = await _db.StringGetAsync(key);

            var value = JsonConvert.DeserializeObject<T>(res);

            return value;
        }

        public async Task Set(string key, T value)
        {
            try
            {
                var res = JsonConvert.SerializeObject(value);

                await _db.StringSetAsync(key, res);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
