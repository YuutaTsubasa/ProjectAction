using System;
using System.Collections;
using System.Collections.Generic;

namespace ProjectAction.AutoAttributes
{
    public static class EnumerableExtensions
    {
        public static void ForEach(this IEnumerable source, Action<object> action)
        {
            if (source == null || action == null)
            {
                return;
            }

            var enumerator = source.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    action(enumerator.Current);
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null)
            {
                return;
            }

            var enumerator = source.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    action(enumerator.Current);
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null || action == null)
            {
                return;
            }

            var index = 0;
            source.ForEach(value =>
            {
                action(value, index);
                index++;
            });
        }
    }
}
