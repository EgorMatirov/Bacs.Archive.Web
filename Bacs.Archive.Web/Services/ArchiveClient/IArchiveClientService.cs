using System.Collections.Generic;

namespace Bacs.Archive.Web.Services.ArchiveClient
{
    public interface IArchiveClientService
    {
        IEnumerable<Problem> GetAll();
        ProblemFull GetFull(string id);
        string Create(IEnumerable<byte> bytes);
        IEnumerable<byte> GetArchive(string id);
        void UpdateFlags(string id, IEnumerable<ProblemFull.CustomFlag> customFlags, IEnumerable<ProblemFull.ReservedFlag> reservedFlags);
    }
}