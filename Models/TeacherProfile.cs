using System;
using System.Collections.Generic;

namespace StudentTeacherDatabaseApproach.Models;

public partial class TeacherProfile
{
    public int TeacherProfileId { get; set; }

    public int UserId { get; set; }

    public string EmployeeNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual User User { get; set; } = null!;
}
