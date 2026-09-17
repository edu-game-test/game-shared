using System;
using System.Collections.Generic;
using MetaFramework.Common;

namespace GameShared.Tests.Fakes
{
    public sealed class ListLog : ILog
    {
        public List<string> Infos { get; } = new();
        public List<string> Warnings { get; } = new();
        public List<(string Message, Exception? Exception)> Errors { get; } = new();

        public void Info(string message) => Infos.Add(message);
        public void Warn(string message) => Warnings.Add(message);
        public void Error(string message, Exception? exception = null) => Errors.Add((message, exception));
    }
}
