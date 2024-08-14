using MyDefenceSistem.Models;
using System.Collections.Concurrent;

namespace MyDefenceSistem.BL
{
    public static class Information
    {
        public static ConcurrentDictionary<int, CancellationTokenSource> _attacks = new ConcurrentDictionary<int, CancellationTokenSource>();
        public static ConcurrentQueue<Threat> _threatQueue = new ConcurrentQueue<Threat>();
        public static ConcurrentQueue<Threat> _threatDoneQueue = new ConcurrentQueue<Threat>();
    }
}
