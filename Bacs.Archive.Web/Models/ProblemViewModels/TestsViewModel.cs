using System.Collections.Generic;
using System.Linq;

namespace Bacs.Archive.Web.Models.ProblemViewModels
{
    public class Test
    {
        public string Id { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
    }

    public class TestGroup
    {
        public string Id { get; set; }
        public IEnumerable<Test> Tests { get; set; }
        public string Continue { get; set; }
        public long Score { get; set; }
    }

    public class TestsViewModel
    {
        public TestsViewModel(string id, IEnumerable<Services.ArchiveClient.TestGroup> testGroups, IEnumerable<Services.TestsFetcher.Test> tests)
        {
            Id = id;
            var testsDictionary = tests.ToDictionary(x => x.Id, x => x);
            TestGroups = testGroups.Select(x => new TestGroup()
            {
                Id = string.IsNullOrEmpty(x.Id) ? "None" : x.Id,
                Tests = x.Tests
                    .OrderBy(s => s) //TODO: enable natural sorting
                    .Select(s => testsDictionary[s])
                    .Select(ConvertTest),
                Continue = x.WhileOk ? "While ok" : "Always",
                Score = x.Score
            });
        }

        private static Test ConvertTest(Services.TestsFetcher.Test test)
        {
            return new Test {Id = test.Id, Input = test.Input, Output = test.Output};
        }

        public string Id { get; }
        public IEnumerable<TestGroup> TestGroups { get; }
    }
}
