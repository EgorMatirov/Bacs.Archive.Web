using System.Collections.Generic;

namespace Bacs.Archive.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public class UserModel
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public bool IsVerified { get; set; }
        }

        public IList<UserModel> Users;
    }
}
