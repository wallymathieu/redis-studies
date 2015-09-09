using System;
using StackExchange.Redis;


namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddCustomerCommand : Command
    {
        public virtual int Id { get;  set; }

        public virtual int Version { get;  set; }

        public virtual string Firstname { get;  set; }

        public virtual string Lastname { get;  set; }

        public override void Handle(IRepository _repository)
        {
            var command = this;
            _repository.Save(new Customer(command.Id, command.Firstname, command.Lastname, command.Version));
        }

    }
}
