using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class AppUser:IdentityUser
    {
        [PersonalData]
        [Column(TypeName ="nvarchar(150)")]
        public string FullName { get; set; }
    }
}
