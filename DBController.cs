using System;
using Microsoft.Data.SqlClient;
using LabsAndProjsManagerServer.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LabsAndProjsManagerServer.DbAccess
{
    public class DatabaseController
    {

        //For Alex : here you need to enter your connection string to make it work with your DB
        private readonly string CON_STRING = @"Data Source=DESKTOP-951MJR7;Initial Catalog=WebCourse;Integrated Security=True";
        private SqlConnection con;

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
        }

        public DatabaseController()
        {
            try
            {
                SqlDependency.Start(CON_STRING);
                con = new SqlConnection(CON_STRING);
                _isConnected = true;

            }
            catch (Exception)
            {
                _isConnected = false;
            }
        }

        public StudentModel Login(string id)
        {
            StudentModel student = new StudentModel();
            using (var cmd = new SqlCommand($"select * from Students where ID={id}", con))
            {
                var da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                var ds = new DataSet();

                try
                {
                    da.Fill(ds);
                    student = new StudentModel
                    {
                        ID = ds.Tables[0].Rows[0][0].ToString(),
                        FirstName = ds.Tables[0].Rows[0][1].ToString(),
                        LastName = ds.Tables[0].Rows[0][2].ToString(),
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
            string name = "";

            using (var cmd = new SqlCommand($"select Name from Courses where ID={CourseID}", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                da.Fill(ds);
                try
                {
                    name = ds.Tables[0].Rows[0][0].ToString();
                }
                catch
                {
                    return null;
                }
            }
            return name;
        }

        public List<CourseModel> GetStudentCourses(StudentModel student)
        {
            List<CourseModel> studentCourses = new List<CourseModel>();
            int CourseId;

            using (var cmd = new SqlCommand($"select CourseID,SubmissionType,SubmissionIndex,Score from Submissions where StudentID={student.ID}", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                da.Fill(ds);
                try
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        CourseId = int.Parse(ds.Tables[0].Rows[i][0].ToString());
                        if (!studentCourses.Any(x => x.ID == CourseId))
                        {
                            var course = new CourseModel()
                            {
                                Name = GetCourseName(CourseId),
                                ID = CourseId,
                            };
                            studentCourses.Add(course);
                        }

                        var submission = new Submission(ds.Tables[0].Rows[i][1].ToString(), int.Parse(ds.Tables[0].Rows[i][3].ToString()), int.Parse(ds.Tables[0].Rows[i][2].ToString()));

                        if (submission.TypeOfSubmission == "Lab")
                        {
                            studentCourses.First(x => x.ID == CourseId).Labs.Add(submission);
                        }
                        else
                        {
                            studentCourses.First(x => x.ID == CourseId).Projects.Add(submission);
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

        public StudentModel Register(string id, string firstName, string lastName)
        {
            using (var cmd = new SqlCommand($"insert into Students (ID, FirstName, LastName) values ('{id}', '{firstName}', '{lastName}')", con))
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return null;
                }
            }
            return new StudentModel() { ID = id, FirstName = firstName, LastName = lastName, StudentCourses = new List<CourseModel>() };
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {

            }
        }
    }
}