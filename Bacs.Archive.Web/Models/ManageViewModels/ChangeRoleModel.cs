using System.ComponentModel.DataAnnotations;

namespace Bacs.Archive.Web.Models.ManageViewModels
{
    public class ChangeRoleModel
    {
        public string Id { get; set; }
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        [Display(Name = "Подтверждён")]
        public bool IsVerified { get; set; }
    }
}
