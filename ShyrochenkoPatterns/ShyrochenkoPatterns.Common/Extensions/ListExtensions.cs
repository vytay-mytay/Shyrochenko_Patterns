using System.Collections.Generic;

namespace ShyrochenkoPatterns.Common.Extensions
{
    public static class ListExtensions
    {
        public static List<TResult> Empty<TResult>(this List<TResult> list)
        {
            return new List<TResult>();
        }

        public static List<TResult> Replace<TResult>(this List<TResult> list, TResult oldItem, TResult newItem)
        {
            int index = list.IndexOf(oldItem);
            if (index != -1)
                list[index] = newItem;

            return list;
        }
    }
}
