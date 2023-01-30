using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentStatusTrackerServer.Objects
{
    public class CourseObject
    {
        [Required]
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public double CourseScore { get; set; } = 0;

        public List<SubmitObject> Exercises { get; set; } = new List<SubmitObject>();

        public List<SubmitObject> Laboratories { get; set; } = new List<SubmitObject>();

        public CourseObject() { }

        public CourseObject(int courseId, string courseName)
        {
            CourseID = courseId;
            CourseName = courseName;
        }

        public void CalculateScore()
        {
            int sum = 0;
            int SubmitSum = 0;
            int LaboratoriesSum = 0;
            for (int i = 0; i < Laboratories.Count; i++)
            {
                LaboratoriesSum += Laboratories[i].SubmitScore;
                SubmitSum++;
            }
            int ExerciseSum = 0;
            for (int i = 0; i < Exercises.Count; i++)
            {
                ExerciseSum += Exercises[i].SubmitScore;
                SubmitSum++;
            }

            sum += (LaboratoriesSum + ExerciseSum);
            if (SubmitSum != 0)
                sum /= SubmitSum;
            CourseScore = sum;
        }
    }
}