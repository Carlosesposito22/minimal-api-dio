using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Domain.Entities
{
    [Table(name: "ADMINS")]
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Senha { get; set; }

        [StringLength(10)]
        public string Role { get; set; }
    }
}
