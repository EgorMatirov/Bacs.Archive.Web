using System.Collections.Generic;
using Bacs.Archive.Problem;

namespace Bacs.Archive.Web.Services.ArchiveClient
{
    public class ProblemFull
    {
        public class ReservedFlag
        {
            public ReservedFlag(Flag.Types.Reserved flag, bool enabled)
            {
                Flag = flag;
                Enabled = enabled;
            }

            public static ReservedFlag FromName(string name, bool enabled)
            {
                return new ReservedFlag(Utils.ParseEnum<Flag.Types.Reserved>(name), enabled);
            }

            public Flag.Types.Reserved Flag { get; }
            public bool Enabled { get; }
        }
        public class CustomFlag
        {
            public CustomFlag(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        public string Name { get; }
        public string Maintainers { get; }
        public string Id { get; }
        public string Revision { get; }
        public IEnumerable<ReservedFlag> ReservedFlags { get; set; }
        public IEnumerable<CustomFlag> CustomFlags { get; set; }
        public IEnumerable<TestGroup> TestGroups { get; set; }
        public int PretestsCount { get; set; }

        public ProblemFull(string id, string name, string maintainers, string revision, IEnumerable<ReservedFlag> reservedFlags, IEnumerable<CustomFlag> customFlags, IEnumerable<TestGroup> testGroups, int pretestsCount)
        {
            Id = id;
            Name = name;
            Maintainers = maintainers;
            Revision = revision;
            ReservedFlags = reservedFlags;
            CustomFlags = customFlags;
            TestGroups = testGroups;
            PretestsCount = pretestsCount;
        }
    }
}