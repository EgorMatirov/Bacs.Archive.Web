using System.Collections.Generic;

namespace Bacs.Archive.Web.Models.ProblemViewModels
{
    public class EditFlagsViewModel
    {
        public class ReservedFlag
        {
            public string Name { get; set; }
            public bool Enabled { get; set; }
        }
        public string Id { get; set; }
        public List<ReservedFlag> ReservedFlags { get; set; }
        public IEnumerable<string> CustomFlags { get; set; }
    }
}
