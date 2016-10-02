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

        public async Task<Guid[]> Batch(IEnumerable<Command> commands)
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
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            return ids.ToArray();
        }

        public IEnumerable<Command[]> Batches()
        {
            return batches.ToArray();
        }

        public Task<IEnumerable<Command>> Read(Guid[] keys)
        {
            return Task.FromResult( keys.Select(key=>_commands[key]));
        }

        public Task<IEnumerable<Command>> ReadAll()
        {
            return Task.FromResult((IEnumerable<Command>) _commands.Values);
        }
    }
}