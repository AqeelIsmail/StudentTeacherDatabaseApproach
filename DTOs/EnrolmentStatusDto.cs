using System.ComponentModel.DataAnnotations;

namespace StudentTeacherDatabaseApproach.DTOs
{
    public class EnrolmentStatusDto
    {
        [Required]
        public string Status { get; set; } = null!;
    }
}
