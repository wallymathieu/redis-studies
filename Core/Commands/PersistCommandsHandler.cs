using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SomeBasicFileStoreApp.Core.Infrastructure;

namespace SomeBasicFileStoreApp.Core.Commands
{
    public class PersistCommandsHandler
    {
        Thread thread;
        bool stop = false;
        readonly ConcurrentQueue<Command> Commands = new ConcurrentQueue<Command>();
        readonly IAppendBatch _appendBatch;
        EventWaitHandle signal;
        readonly CommandRepository _repo;
        readonly IPubSub _pubSub;

        public PersistCommandsHandler(IAppendBatch appendBatch, CommandRepository repo, IPubSub pubSub)
        {
            _appendBatch = appendBatch;
            _repo = repo;
            signal = new EventWaitHandle(false, EventResetMode.AutoReset);
            _pubSub = pubSub;
        }

        public void Start()
        {
            if (thread != null)
            {
                throw new Exception();
            }
            thread = new Thread(ThreadStart);
            thread.Start();
        }

        private void ThreadStart()
        {
            while (!stop)
            {
                signal.WaitOne();
                AppendBatch();
            }
            // While the batch has been running, more commands might have been added
            // and stop might have been called
            AppendBatch();
        }

        private void AppendBatch()
        {
            var commands = new List<Command>();

            Command command;
            while (Commands.TryDequeue(out command))
            {
                commands.Add(command);
            }

            if (commands.Any())
            {
                _repo.Handled(commands.Select(c=>c.UniqueId));
                _pubSub.Publish(_appendBatch.Batch(commands));
            }
        }

        public void Stop()
        {
            stop = true;
            signal.Set();

            if (thread != null)
            {
                thread.Join();
            }
        }

        public void Handle(Command command)
        {
            // send the command to separate thread and persist it
            Commands.Enqueue(command);
            signal.Set();
        }
    }
}

