using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Model
{
    public partial class User : IdentityUser
    {

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime Birthday { get; set; }
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();

        public virtual ICollection<UserLibrary> UserLibraries { get; set; } = new List<UserLibrary>();

    }
}