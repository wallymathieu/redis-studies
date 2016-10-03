using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SomeBasicFileStoreApp.Core.Infrastructure;

namespace SomeBasicFileStoreApp.Tests
{
    class FakePubSub : IPubSub
    {
        private EventWaitHandle signal;
        private Thread thread;
        private bool stop = false;
        private readonly ConcurrentQueue<Guid[]> publishedIds = new ConcurrentQueue<Guid[]>();


        public FakePubSub()
        {
            signal = new EventWaitHandle(false, EventResetMode.AutoReset);

        }
        public void Publish(Guid[] ids)
        {
            publishedIds.Enqueue(ids);
        }

        public void Start(Action<Guid[]> onEvent)
        {
            if (thread != null)
            {
                throw new Exception();
            }
            thread = new Thread(() => { 
                while (!stop)
                {
                    signal.WaitOne();
                    AppendBatch(onEvent);
                }
                AppendBatch(onEvent);
            });
            thread.Start();
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

        void AppendBatch(Action<Guid[]> onEvent)
        {
            var lostOfCommandIds = new List<Guid[]>();

            Guid[] command;
            while (publishedIds.TryDequeue(out command))
            {
                lostOfCommandIds.Add(command);
            }

            if (lostOfCommandIds.Any())
            {
                foreach (var commandIds in lostOfCommandIds)
                {
                    onEvent(commandIds);
                }
            }        
        }
   }
}