using System;
using LabsAndProjsManagerServer.Objects;
using LabsAndProjsManagerServer.DbAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace StudentStatusTrackerServer.RequestAPI
{
    public class RequestAPI : Hub
    {
        private static DBController DBController;
        public StudentObj SignIn(string id)
        {
            DBController = new DBController();
            if (!DBController.IsConnected)
                throw new HubException("Cannot connect to DB");
            var student = DBController.Login(id);
            if (student != null)
            {
                student.StudentCourses = DBController.GetStudentCourses(student);
                student.SetCoursesScores();
            }
            return student;
        }

        public StudentObj SignUp(string id, string firstName, string lastName)
        {
            return DBController.SignUp(id, firstName, lastName);
        }
    }
}
