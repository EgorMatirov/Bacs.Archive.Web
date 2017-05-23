using Sakura.AspNetCore;

namespace Bacs.Archive.Web.Models.ProblemViewModels
{
    public class IndexViewModel
    {
        public class ProblemInfo
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string Revision { get; set; }
        }

        public string SearchQuery { get; set; }
        public IPagedList<ProblemInfo> Problems { get; set; }
    }
}