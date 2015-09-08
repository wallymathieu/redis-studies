using NUnit.Framework;
using System.IO;
using System.Linq;
using With;
using System.Collections.Generic;
using StackExchange.Redis;
using SomeBasicFileStoreApp.Core.Infrastructure.Redis;

namespace SomeBasicFileStoreApp.Tests.Redis
{
	[TestFixture]
	public class PersistingEventsTests
	{
        private ConnectionMultiplexer redis;

        [SetUp]
        public void SetUp()
        {
            redis = ConnectionMultiplexer.Connect("localhost,resolvedns=1");
        }

		private List<int> dbs = new List<int>();
		[TearDown]
		public void TearDown()
		{
            if (redis != null)
            {
                foreach (var db in dbs)
                {
                    redis.GetServer("localhost,resolvedns=1").FlushDatabase(db);
                }
                redis.Close();
            }
		}

		[Test]
		public void Read_items_persisted_in_single_batch()
		{
			var commands = new GetCommands().Get().ToArray();
            var _persist = new AppendToRedis(redis.GetDatabase(
                12354.Tap(db => dbs.Add(db))));
            _persist.Batch(commands);
			Assert.That(_persist.ReadAll().Count(), Is.EqualTo(commands.Length));
		}
	}
}