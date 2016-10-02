using NUnit.Framework;
using System.Linq;
using StackExchange.Redis;
using SomeBasicFileStoreApp.Core.Infrastructure.Redis;
using System.Net;

namespace SomeBasicFileStoreApp.Tests.Redis
{
    [TestFixture]
    public class PersistingEventsTests
    {
        private ConnectionMultiplexer redis;
        private EndPoint endpoint;
        [SetUp]
        public void SetUp()
        {
            redis = ConnectionMultiplexer.Connect("localhost,resolvedns=1,allowadmin=1,connectTimeout=10000");
            endpoint = redis.GetEndPoints().First();
        }

        [TearDown]
        public void TearDown()
        {
            if (redis != null)
            {
                redis.GetServer(endpoint).FlushDatabase();
                redis.Close();
            }
        }

        [Test]
        public void Read_items_persisted_in_single_batch()
        {
            var commands = new GetCommands().Get().ToArray();
            var _persist = new AppendToRedis(redis.GetDatabase());
            _persist.Batch(commands);
            var read = _persist.ReadAll();
            Assert.That(read.Count(), Is.EqualTo(commands.Length));
        }
    }
}