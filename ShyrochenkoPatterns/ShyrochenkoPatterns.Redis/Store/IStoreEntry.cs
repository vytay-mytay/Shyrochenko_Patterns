using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Redis.Store
{
    public interface IStoreEntry
    {
        HashEntry[] GetEntry();
    }
}
