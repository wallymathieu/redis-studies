using System;
using System.Linq;

namespace SomeBasicFileStoreApp.Core
{
    public class CommandsAddedEventHandler
    {
        readonly CommandRepository _commandRepository;
        readonly IReadBatch _readBatch;
        readonly IRepository _repository;

        public CommandsAddedEventHandler(CommandRepository commandRepository,
                                         IReadBatch readBatch,
                                         IRepository repository)
        {
            _commandRepository = commandRepository;
            _readBatch = readBatch;
            _repository = repository;
        }

        public void OnReceive(Guid[] keys)
        {
            foreach (var command in _readBatch.Read(_commandRepository.UnHandled(keys).ToArray()))
            {
                command.Handle(_repository);
            }
        }
    }
}
