using System;
using StackExchange.Redis;


namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddCustomerCommand : Command
    {
        public virtual int Id { get; private set; }

        public virtual int Version { get; private set; }

        public virtual string Firstname { get; private set; }

        public virtual string Lastname { get; private set; }

        public override void Handle(IRepository _repository)
        {
            var command = this;
            _repository.Save(new Customer(command.Id, command.Firstname, command.Lastname, command.Version));
        }
        public override Guid Persist(IBatch batch)
        {
            var id = this.HashCreate(batch);
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", Id),
                    new HashEntry("Version", Version),
                    new HashEntry("Firstname", Firstname),
                    new HashEntry("Lastname", Lastname),
                });
            return id;
        }
    }
}
