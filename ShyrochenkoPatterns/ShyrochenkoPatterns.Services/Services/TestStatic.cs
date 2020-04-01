using ShyrochenkoPatterns.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ShyrochenkoPatterns.Services.Services
{
    public static class TestStatic
    {
        public static ConcurrentQueue<Likes> LikesQueue { get; set; } = new ConcurrentQueue<Likes>();

        public static List<Likes> LikesList { get; set; } = new List<Likes>();

        public static ConcurrentDictionary<int, Likes> LikesDictionary { get; set; } = new ConcurrentDictionary<int, Likes>();

        public static object Locker { get; set; } = new object();
    }
}
