using System;
using System.Collections.Generic;
using SomeBasicFileStoreApp.Core;
using SomeBasicFileStoreApp.Core.Commands;

namespace SomeBasicFileStoreApp.Tests
{
    internal class ObjectContainer : IDisposable
    {
        private CommandHandler[] handlers;
        private PersistCommandsHandler _persistToFile;
        private readonly IRepository _repository = new Repository();
        private readonly FakeAppendToFile _fakeAppendToFile;
        private readonly CommandRepository _commandRepository;
        private readonly FakePubSub _pubSub;
        private readonly CommandsAddedEventHandler _commandAddedHandler;


        public ObjectContainer()
        {
            _fakeAppendToFile = new FakeAppendToFile();
            _commandRepository = new CommandRepository();
            _pubSub = new FakePubSub();
            _persistToFile = new PersistCommandsHandler(_fakeAppendToFile, _commandRepository, _pubSub);
            _commandAddedHandler = new CommandsAddedEventHandler(_commandRepository,
                                                                 _fakeAppendToFile,
                                                                 _repository);
            handlers = new CommandHandler[] {
                (command)=>command.Handle(_repository),
                _persistToFile.Handle
            };
        }

        public void Boot()
        {
            _pubSub.Start(ids => _commandAddedHandler.OnReceive(ids));
            _persistToFile.Start();
        }

        public IRepository GetRepository()
        {
            return _repository;
        }

        public void Dispose()
        {
            _persistToFile.Stop();
            _pubSub.Stop();
        }

        public IEnumerable<Command[]> BatchesPersisted()
        {
            return _fakeAppendToFile.Batches();
        }

        public void HandleAll(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                if (_commandRepository.IsUnHandled(command.UniqueId))
                {
                    foreach (var handler in handlers)
                    {
                        handler.Invoke(command);
                    }
                }
            }
        }
    }
}