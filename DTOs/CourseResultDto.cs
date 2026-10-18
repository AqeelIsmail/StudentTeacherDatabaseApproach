using System.ComponentModel.DataAnnotations;

namespace StudentTeacherDatabaseApproach.DTOs
{
    public class CourseResultDto
    {
        [Range(typeof(decimal), "0", "100")]
        public decimal FinalMark { get; set; }
    }
}
