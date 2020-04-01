using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Redis
{
    public class RedisConfig
    {
        public string SubscribeChannel { get; set; }

        public string SendChannel { get; set; }

        public string Host { get; set; }
    }
}
