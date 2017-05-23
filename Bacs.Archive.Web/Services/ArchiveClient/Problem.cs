namespace Bacs.Archive.Web.Services.ArchiveClient
{
    public class Problem
    {
        public string Name { get; }
        public string Id { get; }
        public string Revision { get; }

        public Problem(string name, string id, string revision)
        {
            Name = name;
            Id = id;
            Revision = revision;
        }
    }
}