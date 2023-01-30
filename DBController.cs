using System;
using System.Data.SqlClient;
using StudentStatusTrackerServer.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LabsAndProjsManagerServer.DbAccess
{
    public class DBController
    {

        //Execution Instructions : here you need to enter your connection string to make it work with your DB
        private readonly string CONNECTIOM_STRING = @"Data Source=DESKTOP-951MJR7;Initial Catalog=WebCourse;Integrated Security=True";
        private SqlConnection sqlConnection;

        private bool isConnected;
        public bool Connected
        {
            get => isConnected;
        }

        public DBController()
        {
            try
            {
                SqlDependency.Start(CONNECTIOM_STRING);
                sqlConnection = new SqlConnection(CONNECTIOM_STRING);
                isConnected = true;

            }
            catch (Exception)
            {
                isConnected = false;
            }
        }

        public StudentObject SignIn(string studentId)
        {
            StudentObject student = new StudentObject();
            using (var cmd = new SqlCommand($"select * from Students where ID={studentId}", sqlConnection))
            {
                var dataAdapter = new SqlDataAdapter(cmd);
                var sqlDependency = new SqlDependency(cmd);
                sqlDependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                var dataSet = new DataSet();

                try
                {
                    dataAdapter.Fill(dataSet);
                    student = new StudentObject
                    {
                        StudentID = dataSet.Tables[0].Rows[0][0].ToString(),
                        StudentFirstName = dataSet.Tables[0].Rows[0][1].ToString(),
                        StudentLastName = dataSet.Tables[0].Rows[0][2].ToString(),
                    };
                }
                catch
                {
                    return null;
                }
            }
            return student;
        }

        public string GetCourseName(int CourseID)
        {
            string Coursename = "";

            using (var cmd = new SqlCommand($"select Name from Courses where ID={CourseID}", sqlConnection))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                try
                {
                    Coursename = dataSet.Tables[0].Rows[0][0].ToString();
                }
                catch
                {
                    return null;
                }
            }
            return Coursename;
        }

        public List<CourseObject> GetStudentCourses(StudentObject student)
        {
            List<CourseObject> studentCourses = new List<CourseObject>();
            int CourseId;

            using (var cmd = new SqlCommand($"select CourseID,SubmissionType,SubmissionIndex,Score from Submissions where StudentID={student.StudentID}", sqlConnection))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                var sqlDependency = new SqlDependency(cmd);
                sqlDependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                try
                {
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        CourseId = int.Parse(dataSet.Tables[0].Rows[i][0].ToString());
                        if (!studentCourses.Any(x => x.CourseID == CourseId))
                        {
                            var course = new CourseObject()
                            {
                                CourseName = GetCourseName(CourseId),
                                CourseID = CourseId,
                            };
                            studentCourses.Add(course);
                        }

                        var submit = new SubmitObject(dataSet.Tables[0].Rows[i][1].ToString(), int.Parse(dataSet.Tables[0].Rows[i][3].ToString()), int.Parse(dataSet.Tables[0].Rows[i][2].ToString()));

                        if (submit.SubmitType == "Lab")
                        {
                            studentCourses.First(currentCourse => currentCourse.CourseID == CourseId).Laboratories.Add(submit);
                        }
                        else
                        {
                            studentCourses.First(currentCourse => currentCourse.CourseID == CourseId).Exercises.Add(submit);
                        }
                    }

                }
                catch
                {
                    return null;
                }
            }
            return studentCourses;
        }

        public StudentObject SignUp(string studentId, string studentFirstName, string studentLastName)
        {
            using (var command = new SqlCommand($"insert into Students (ID, FirstName, LastName) values ('{studentId}', '{studentFirstName}', '{studentLastName}')", sqlConnection))
            {
                if (command.Connection.State != ConnectionState.Open)
                    command.Connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return null;
                }
            }
            return new StudentObject() { StudentID = studentId, StudentFirstName = studentFirstName, StudentLastName = studentLastName, StudentCoursesList = new List<CourseObject>() };
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs sqlEvent)
        {
            if (sqlEvent.Type == SqlNotificationType.Change)
            {
                Console.Write(sqlEvent.Type.ToString());
            }
        }
    }
}