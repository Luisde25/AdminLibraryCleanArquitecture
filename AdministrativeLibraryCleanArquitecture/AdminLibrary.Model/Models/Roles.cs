using AdminLibrary.Models;
using AdminLibrary.Models.Entities;
using AdminLibrary.Shared;
using Microsoft.EntityFrameworkCore;

namespace AdminLibrary.Model.Models
{
    public class Roles : EntityBase<int>
    {
        public Roles()
        {

        }
        public Roles(string name, string desciption, bool status)
        {
            Name = name;
            Description = desciption;
            Status = status;
        }

        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }

        /// <summary>
        /// Relacion de muchos a Uno.
        /// </summary>
       public virtual ICollection<UsersRoles> UsersRolesVirtual { get; set; } = null!;
       
    }
}
