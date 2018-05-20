using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

// https://metanit.com/sharp/aspnet5/16.13.php
namespace MWN.ViewModels
{
    //Эта модель позволит управлять всеми ролями для одного пользователя 
    //(в ASP.NET Core Identity один пользователь може иметь множество ролей).
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
