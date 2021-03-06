﻿using System;

namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddProductCommand : Command
    {
        public virtual int Id { get;  set; }
        public virtual int Version { get;  set; }
        public virtual float Cost { get;  set; }
        public virtual string Name { get;  set; }
        public AddProductCommand()
        {
        }
        public AddProductCommand(Guid id) : base(id)
        {
        }
        public override void Handle(Repository repository)
        {
            var command = this;
            Product p;
            if (!repository.TryGetProduct(Id, out p))
            {
                repository.Save(new Product(command.Id, command.Cost, command.Name, command.Version));
            }
        }
    }
}
