using System;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace StudentStatusTrackerServer.Objects
{
    public class StudentObject
    {
        [Required]
        public string StudentID { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }


        public List<CourseObject> StudentCoursesList { get; set; } = new List<CourseObject>();


        public StudentObject() { }

        public StudentObject(string studentId, string studentFirstName, string studentLastName)
        {
            StudentID = studentId;
            StudentFirstName = studentFirstName;
            StudentLastName = studentLastName;
        }

        public void SetCoursesScores()
        {
            foreach (var course in StudentCoursesList)
                course.CalculateScore();
        }
    }
}