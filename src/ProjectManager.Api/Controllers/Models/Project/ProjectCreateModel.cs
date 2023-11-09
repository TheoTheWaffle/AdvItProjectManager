using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Controllers.Models.Project
{
    public class ProjectCreateModel
    {
        [Required(ErrorMessage = "{0} field is required", AllowEmptyStrings = false)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
