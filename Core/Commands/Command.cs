namespace SomeBasicFileStoreApp.Core.Commands
{
    public abstract class Command
    {
        public long SequenceNumber { get; set; }
        public abstract void Handle(IRepository repository);
    }
}