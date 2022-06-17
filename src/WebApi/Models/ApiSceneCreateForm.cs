using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class ApiSceneCreateForm
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
