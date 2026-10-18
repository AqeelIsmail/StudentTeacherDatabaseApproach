using System.ComponentModel.DataAnnotations;

namespace StudentTeacherDatabaseApproach.DTOs
{
    //DTO used for creating and updating course categories
    public class CourseCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
