using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeBasicFileStoreApp.Core.Commands
{
    public abstract class Command
    {
        public Command():this(Guid.NewGuid())
        {
        }
        public Command(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }
        /// <summary>
        /// Used for debugging purposes
        /// </summary>
        /// <value>The sequence number.</value>
        public long SequenceNumber { get; set; }
        public Guid UniqueId { get; set; }

        public abstract void Handle(IRepository repository);
        public static string ToString(IEnumerable<Guid> uniqueIds)
        {
            return string.Join(",", uniqueIds.Select(id => id.ToString("N")));
        }
        public static Guid[] ParseUniqueIds(string keys)
        {
            return keys.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(id => Guid.Parse(id))
                       .ToArray();
        }

    }
}