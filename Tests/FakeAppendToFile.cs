using System.Collections.Generic;
using SomeBasicFileStoreApp.Core.Commands;
using System.Linq;
using SomeBasicFileStoreApp.Core.Infrastructure;
using System.Threading.Tasks;
using System;
using SomeBasicFileStoreApp.Core;

namespace SomeBasicFileStoreApp.Tests
{
    public class FakeAppendToFile : IAppendBatch, IReadBatch
    {
        private readonly IList<Command[]> batches = new List<Command[]>();
        private readonly IDictionary<Guid, Command> _commands = new Dictionary<Guid, Command>();

        public Guid[] Batch(IEnumerable<Command> commands)
        {
            var cs = commands.ToArray();
            batches.Add(cs);
            var ids = new List<Guid>(cs.Length);
            foreach (var command in commands)
            {
                var g = Guid.NewGuid();
                _commands.Add(g, command);
                ids.Add(g);
            }
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
            return ids.ToArray();
        }

        public IEnumerable<Command[]> Batches()
        {
            return batches.ToArray();
        }

        public IEnumerable<Command> Read(Guid[] keys)
        {
            return keys.Select(key=>_commands[key]);
        }

        public IEnumerable<Command> ReadAll()
        {
            return _commands.Values;
        }
    }
}