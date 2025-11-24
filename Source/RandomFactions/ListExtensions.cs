using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomFactions;

// Extension: shuffle list in a reproducible order
public static class ListExtensions
{
    public static IEnumerable<T> InRandomOrder<T>(this IList<T> list, Random prng)
    {
        var copy = list.ToList();
        for(int i = 0; i < copy.Count; i++)
        {
            int swapIdx = prng.Next(i, copy.Count);
            (copy[i], copy[swapIdx]) = (copy[swapIdx], copy[i]);
        }
        return copy;
    }
}