using System;
using System.Collections.Generic;
using SomeBasicFileStoreApp.Core.Commands;

namespace SomeBasicFileStoreApp.Core
{
    public interface IReadBatch
    {
        IEnumerable<Command> ReadAll();
        IEnumerable<Command> Read(Guid[] keys);
    }
}
