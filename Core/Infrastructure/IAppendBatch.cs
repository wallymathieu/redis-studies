using SomeBasicFileStoreApp.Core.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SomeBasicFileStoreApp.Core.Infrastructure
{
    public interface IAppendBatch
    {
        Task<Guid[]> Batch(IEnumerable<Command> commands);
    }
}
