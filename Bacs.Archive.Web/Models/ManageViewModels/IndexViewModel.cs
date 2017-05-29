using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bacs.Archive.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public class UserModel
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            [Display(Name = "Подтверждён")]
            public bool IsVerified { get; set; }
            [Display(Name = "Может загружать задачи")]
            public bool CanUpload { get; set; }
        }

        public IList<UserModel> Users;
    }
}
