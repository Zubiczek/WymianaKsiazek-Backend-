using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WymianaKsiazek.Api.Database
{
    public class UserEntity : IdentityUser
    {
        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public virtual ICollection<BookEntity> Books { get; set; }

        public UserEntity()
        {
            CreatedOn = DateTime.UtcNow;
            UpdatedOn = DateTime.UtcNow;
            Books = new List<BookEntity>();
        }
    }
}
