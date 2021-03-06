﻿using System;
namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddCustomerCommand : Command
    {
        public virtual int Id { get; set; }
        public virtual int Version { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Lastname { get; set; }
        public AddCustomerCommand()
        {
        }
        public AddCustomerCommand(Guid id) : base(id)
        {
        }
        public override void Handle(Repository repository)
        {
            var command = this;
            Customer c;
            if (!repository.TryGetCustomer(command.Id, out c))
            {
                repository.Save(new Customer(command.Id, command.Firstname, command.Lastname, command.Version));
            }
        }
    }
}
