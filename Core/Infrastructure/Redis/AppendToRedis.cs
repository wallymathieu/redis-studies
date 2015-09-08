using System.Linq;
using With;
using System.Collections.Generic;
using StackExchange.Redis;
using SomeBasicFileStoreApp.Core.Commands;
using System;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
	public class AppendToRedis
	{
        private readonly IDatabase db;
        public AppendToRedis(IDatabase db)
        {
            this.db = db;
        }
        public void Batch(IEnumerable<Command> commands){
            var batch = this.db.CreateBatch();
            foreach (var command in commands)
            {
                command.Persist(batch);
            }
            batch.Execute();
        }
        public IEnumerable<Command> ReadAll(){
            throw new NotImplementedException();
        }
	}

}