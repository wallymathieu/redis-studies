using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeBasicFileStoreApp.Core.Commands
{
	public abstract class Command
	{
        public long SequenceNumber { get; set; }
        public abstract void Handle(IRepository repository);
	}
}