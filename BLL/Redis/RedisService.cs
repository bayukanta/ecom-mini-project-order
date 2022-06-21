using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Redis
{
    public class RedisService : IRedisService
    {

        private readonly ILogger _logger;

        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        public RedisService(IConfiguration configuration, ILogger<RedisService> logger)
        {
            string connectionString = configuration.GetValue<string>("RedisServer:ConnectionString");
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
            _logger = logger;
        }

        public ConnectionMultiplexer Connection => _lazyConnection.Value;

        private IDatabase RedisDb => Connection.GetDatabase();

        private List<IServer> GetRedisServers() => Connection.GetEndPoints().Select(endpoint => Connection.GetServer(endpoint)).ToList();


        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                bool stringValue = await RedisDb.KeyDeleteAsync(key);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                RedisValue stringValue = await RedisDb.StringGetAsync(key);
                if (string.IsNullOrEmpty(stringValue))
                {
                    return default;
                }
                else
                {
                    T objectValue = JsonConvert.DeserializeObject<T>(stringValue);
                    return objectValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default;
            }
        }

        public async Task SaveAsync(string key, object value)
        {
            try
            {
                string stringValue = JsonConvert.SerializeObject(value);
                await RedisDb.StringSetAsync(key, stringValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return;
            }

        }
    }
}
