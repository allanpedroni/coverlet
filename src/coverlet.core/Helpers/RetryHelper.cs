﻿// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Coverlet.Core.Abstractions;

namespace Coverlet.Core.Helpers
{
  // A slightly amended version of the code found here: https://stackoverflow.com/a/1563234/186184
  // This code allows for varying backoff strategies through the use of Func<TimeSpan>.
  internal class RetryHelper : IRetryHelper
  {
    /// <summary>
    /// Retry a void method.
    /// </summary>
    /// <param name="action">The action to perform</param>
    /// <param name="backoffStrategy">A function returning a Timespan defining the backoff strategy to use.</param>
    /// <param name="maxAttemptCount">The maximum number of retries before bailing out. Defaults to 3.</param>
    public void Retry(
        Action action,
        Func<TimeSpan> backoffStrategy,
        int maxAttemptCount = 3)
    {
      Do<object>(() =>
      {
        action();
        return null;
      }, backoffStrategy, maxAttemptCount);
    }

    /// <summary>
    /// Retry a method returning type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to return</typeparam>
    /// <param name="action">The action to perform</param>
    /// <param name="backoffStrategy">A function returning a Timespan defining the backoff strategy to use.</param>
    /// <param name="maxAttemptCount">The maximum number of retries before bailing out. Defaults to 3.</param>
    public T Do<T>(
        Func<T> action,
        Func<TimeSpan> backoffStrategy,
        int maxAttemptCount = 3)
    {
      var exceptions = new List<Exception>();

      for (int attempted = 0; attempted < maxAttemptCount; attempted++)
      {
        try
        {
          if (attempted > 0)
          {
            Thread.Sleep(backoffStrategy());
          }
          return action();
        }
        catch (DirectoryNotFoundException)
        {
          // do nothing
          return default;
        }
        catch (IOException ex)
        {
          exceptions.Add(ex);
        }
      }

      throw new AggregateException(exceptions);
    }

  }
}
