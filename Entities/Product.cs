using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }    
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } 
    }
}
