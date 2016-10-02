using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SomeBasicFileStoreApp.Core.Commands;

namespace SomeBasicFileStoreApp.Core
{
    public interface IReadBatch
    {
        Task<IEnumerable<Command>> ReadAll();
        Task<IEnumerable<Command>> Read(Guid[] keys);
    }
}
