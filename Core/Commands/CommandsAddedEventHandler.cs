using System;
using System.Linq;
using System.Threading.Tasks;

namespace SomeBasicFileStoreApp.Core
{
    public class CommandsAddedEventHandler
    {
        readonly CommandRepository _commandRepository;
        readonly IReadBatch _readBatch;
        readonly Repository _repository;

        public CommandsAddedEventHandler(CommandRepository commandRepository,
                                         IReadBatch readBatch,
                                         Repository repository)
        {
            _commandRepository = commandRepository;
            _readBatch = readBatch;
            _repository = repository;
        }

        public async Task OnReceive(Guid[] keys)
        {
            foreach (var command in await _readBatch.Read(_commandRepository.UnHandled(keys).ToArray()))
            {
                command.Handle(_repository);
            }
        }
    }
}
