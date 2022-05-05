//-------------------------------------------------------------------------
// Copyright © 2020 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace Health.PharmaNet.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The LoggerMessage Extensions for .NET 6 optimized logging.
    /// </summary>
#pragma warning disable CA1810, CS8625
    public static class Logger
    {
        private static readonly Action<ILogger, string, Exception> InfoLogMessage;
        private static readonly Action<ILogger, string, Exception> DebugLogMessage;

        private static readonly Action<ILogger, string, Exception> ErrorMessage;

        /// <summary>
        /// Initializes static members of the <see cref="Logger"/> class.
        /// </summary>
        static Logger()
        {
            InfoLogMessage = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1, "Informational"),
                "Informational: {Param1}");

            DebugLogMessage = LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(2, "Debug"),
                "Debug: {Message}");

            ErrorMessage = LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(3, "Error"),
                "Error: '{Error}')");
        }

        /// <summary>
        /// Generic Logging Informational level.
        /// </summary>
        /// <param name="logger">An ILogger instance.</param>
        /// <param name="message">The informational message to log.</param>
        public static void LogInformation(this ILogger logger, string message)
        {
            InfoLogMessage(logger, message, null);
        }

        /// <summary>
        /// Generic Logging Debug level.
        /// </summary>
        /// <param name="logger">An ILogger instance.</param>
        /// <param name="message">The informational message to log.</param>
        public static void LogDebug(this ILogger logger, string message)
        {
            DebugLogMessage(logger, message, null);
        }

        /// <summary>
        /// Generic Logging Debug level.
        /// </summary>
        /// <param name="logger">An ILogger instance.</param>
        /// <param name="message">The error message to log.</param>
        public static void LogError(this ILogger logger, string message)
        {
            ErrorMessage(logger, message, null);
        }

        /// <summary>
        /// Generic Logging Debug level.
        /// </summary>
        /// <param name="logger">An ILogger instance.</param>
        /// <param name="message">The error message to log.</param>
        /// <param name="ex">Optional Exception object.</param>
        public static void LogException(this ILogger logger, string message, Exception ex)
        {
            ErrorMessage(logger, message, ex);
        }
    }
#pragma warning restore CA1810, CS8625
}