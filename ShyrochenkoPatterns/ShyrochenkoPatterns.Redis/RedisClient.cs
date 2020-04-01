using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Redis
{
    public class RedisClient
    {
        private static ConnectionMultiplexer connection;
        private ISubscriber _pubsub;
        private readonly RedisConfig _config;

        private Action<string, string> OnNewMessage;

        public RedisClient(IOptions<RedisConfig> options)
        {
            _config = options.Value;
        }

        public RedisClient(RedisConfig config)
        {
            _config = config;
        }

        public void Connect()
        {
            if (connection == null || !IsConnected)
            {
                connection = ConnectionMultiplexer.Connect(_config.Host);

                // Create pub/sub
                _pubsub = connection.GetSubscriber();
            }
        }

        public bool IsConnected => connection?.IsConnected ?? false;

        public IDatabase GetDB()
        {
            return connection?.GetDatabase();
        }

        public void Subscribe(string channelName, Action<RedisChannel, RedisValue> action)
        {
            if (!string.IsNullOrEmpty(channelName) && action != null)
            {
                _pubsub?.Subscribe(channelName, (channel, message) => action?.Invoke(channel, message));             
            }
        }

        public async Task SendBack(string message)
        {
            await _pubsub?.PublishAsync(_config.SubscribeChannel, message);
        }

        public async Task Send(string channelId, string message)
        {
            await _pubsub?.PublishAsync(channelId, message);
        }
    }
}
