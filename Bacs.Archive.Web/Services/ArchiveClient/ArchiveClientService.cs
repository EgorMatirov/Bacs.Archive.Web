using System;
using System.Collections.Generic;
using System.Linq;
using Bacs.Archive.Client.CSharp;
using Bacs.Archive.Problem;
using Bacs.Problem.Single;

namespace Bacs.Archive.Web.Services.ArchiveClient
{
    public class TestGroup
    {
        public string Id { get; set; }
        public long Score { get; set; }
        public IEnumerable<string> Tests { get; set; }
        public bool WhileOk { get; set; }
    }
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ArchiveClientService : IArchiveClientService
    {
        private readonly string _bacsStatementsKey;
        private readonly IArchiveClient _archiveClient;

        public ArchiveClientService(string host, int port, string clientCertificatePath, string clientKeyPath, string caCertificatePath, string bacsStatementsKey)
        {
            _bacsStatementsKey = bacsStatementsKey;
            _archiveClient = ArchiveClientFactory.CreateFromFiles(host, port, clientCertificatePath, clientKeyPath, caCertificatePath);
        }

        public IEnumerable<Problem> GetAll()
        {
            var statusAll = _archiveClient.StatusAll().ToArray();
            return statusAll.Select(x => new Problem(string.Empty, x.Key, x.Value.Status.Revision.Value.ToBase64()));
        }

        public ProblemFull GetFull(string id)
        {
            var importResult = _archiveClient.ImportResult(id)[id];
            var statusResult = _archiveClient.Status(id)[id];
            if (statusResult.Error?.Code == Error.Types.Code.NotFound) return null;
            var flags = statusResult.Status.Flags.Flag.ToList();

            var containsFlag = new Func<Flag.Types.Reserved, bool>(x => flags.Any(y => y.Reserved == x));

            var reservedFlags = Enum.GetValues(typeof(Flag.Types.Reserved))
                .Cast<Flag.Types.Reserved>()
                .Where(x => x != Flag.Types.Reserved.None)
                .Select(x => new ProblemFull.ReservedFlag(x, containsFlag(x)))
                .ToList();

            var customFlags = flags
                .Where(x => x.FlagCase == Flag.FlagOneofCase.Custom)
                .Select(x => new ProblemFull.CustomFlag(x.Custom))
                .ToList();

            var name = importResult.Problem.Info.Name.First().Value;
            var maintainers = string.Join(", ", importResult.Problem.Info.Maintainer);
            var revision = importResult.Problem.System.Revision.Value.ToBase64();

            var extensionValue = importResult.Problem.Profile.First().Extension.Value;
            var profileExtension = ProfileExtension.Parser.ParseFrom(extensionValue);
            var testGroups = profileExtension.TestGroup.Select(x => new TestGroup
            {
                Id = x.Id,
                Score = x.Score,
                Tests = x.Tests.Query.Select(y => y.Id),
                WhileOk = x.Tests.ContinueCondition == TestSequence.Types.ContinueCondition.WhileOk,
            });

            const string pretestsPrefix = "pre";
            var hasPretests = profileExtension.TestGroup.Any(x => x.Id == pretestsPrefix);
            var pretestsCount = !hasPretests ? 0 : profileExtension
                .TestGroup
                .FirstOrDefault(x => x.Id != pretestsPrefix)
                .Tests.Query.Count;

            return new ProblemFull(
                id,
                name,
                maintainers,
                revision,
                reservedFlags,
                customFlags,
                testGroups,
                pretestsCount,
                $"http://bacs.cs.istu.ru/problem/{id}?key={_bacsStatementsKey}");
        }

        public string Create(IEnumerable<byte> bytes)
        {
            return _archiveClient.Upload(SevenZipArchive.ZipFormat, bytes).First().Key;
        }

        public IEnumerable<byte> GetArchive(string id)
        {
            return _archiveClient.Download(SevenZipArchive.ZipFormat, id);
        }

        public void UpdateFlags(string id, IEnumerable<ProblemFull.CustomFlag> customFlags, IEnumerable<ProblemFull.ReservedFlag> reservedFlags)
        {
            _archiveClient.ClearFlags(id);
            var enabledReservedFlags = reservedFlags.Where(x => x.Enabled).Select(x => x.Flag).ToArray();
            var customFlagNames = customFlags.Select(x => x.Name).ToArray();
            _archiveClient.SetFlags(enabledReservedFlags, customFlagNames, id);
        }
    }
}