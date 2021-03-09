using System;

namespace WymianaKsiazek.Api.Database
{
    public class BookEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UserId { get; set; }
        public virtual UserEntity User { get; set; }

        public BookEntity()
        {
            CreatedOn = DateTime.UtcNow;
            UpdatedOn = DateTime.UtcNow;
        }
    }
}
