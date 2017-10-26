using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SomeBasicFileStoreApp.Core
{
    public class CommandRepository
    {
        object key = true;
        readonly ConcurrentDictionary<Guid, object> handledCommands = new ConcurrentDictionary<Guid, object>();
        public void Handled(IEnumerable<Guid> commandIds)
        {
            foreach (var command in commandIds)
            {
                handledCommands.TryUpdate(command, key, key);
            }
        }
        public IEnumerable<Guid> UnHandled(IEnumerable<Guid> commandIds)
        {
            return commandIds
                .Where(IsUnHandled)
                .ToArray();
        }

        public bool IsUnHandled(Guid uniqueId)
        {
            object v;
            return !handledCommands.TryGetValue(uniqueId, out v);
        }
   }
}