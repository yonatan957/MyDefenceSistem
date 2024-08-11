using MyDefenceSistem.Models;
using System.Collections.Concurrent;

namespace MyDefenceSistem.Services
{
    public static class Information
    {
        public static ConcurrentDictionary<int, CancellationTokenSource> _attacks = new ConcurrentDictionary<int, CancellationTokenSource>();
        public static ConcurrentQueue<Threat> _threatQueue = new ConcurrentQueue<Threat>();
    }
}
