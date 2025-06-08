using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Data;

namespace VkrProgramm
{
    public partial class EditAttendanceAndGrades : Window
    {
        private string connectionString = MySqlConnectionString.GetConnectionMySql();

        public class Group
        {
            public int GroupID { get; set; }
            public string GroupName { get; set; }
            public override string ToString() { return GroupName; }
        }

        public class ScheduleItem
        {
            public int ScheduleID { get; set; }
            public string DisplayName { get; set; }
            public override string ToString() => DisplayName;
        }

        public class StudentAttendanceGrade
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public string Grade { get; set; }
            public bool IsPresent { get; set; }
        }

        private List<StudentAttendanceGrade> studentsList = new List<StudentAttendanceGrade>();

        public EditAttendanceAndGrades()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            cmbGroup.Items.Clear();
            string query = "SELECT GroupID, GroupName FROM student_groups ORDER BY GroupName";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Group group = new Group();
                        group.GroupID = reader.GetInt32("GroupID");
                        group.GroupName = reader.GetString("GroupName");
                        cmbGroup.Items.Add(group);
                    }
                }
            }
        }

        private void cmbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Group selectedGroup = cmbGroup.SelectedItem as Group;
            if (selectedGroup == null)
            {
                return;
            }

            LoadSchedules(selectedGroup.GroupID);
        }

        private void LoadSchedules(int groupId)
        {
            List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

            string query = @"
                SELECT ScheduleID, Date, Time, subj.SubjectName
                FROM schedule s
                INNER JOIN subjects subj ON s.SubjectID = subj.SubjectID
                WHERE s.GroupID = @groupId
                ORDER BY s.Date, s.Time";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@groupId", groupId);
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ScheduleItem item = new ScheduleItem();
                        item.ScheduleID = reader.GetInt32("ScheduleID");
                        DateTime date = reader.GetDateTime("Date");
                        TimeSpan time = reader.GetTimeSpan("Time");
                        string subjectName = reader.GetString("SubjectName");
                        item.DisplayName = $"{date:dd.MM.yyyy} {time:hh\\:mm} — {subjectName}";
                        scheduleItems.Add(item);
                    }
                }
            }

            cmbSchedule.ItemsSource = scheduleItems;
        }

        private void cmbSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            studentsList.Clear();
            dataGridStudents.ItemsSource = null;

            ScheduleItem selectedSchedule = cmbSchedule.SelectedItem as ScheduleItem;
            Group selectedGroup = cmbGroup.SelectedItem as Group;

            if (selectedSchedule != null && selectedGroup != null)
            {
                LoadStudentsAndData(selectedGroup.GroupID, selectedSchedule.ScheduleID);
            }
        }

        private void LoadStudentsAndData(int groupId, int scheduleId)
        {
            studentsList.Clear();

            string queryStudents = @"SELECT StudentID, StudentFullName FROM students WHERE GroupID = @groupId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(queryStudents, connection))
            {
                command.Parameters.AddWithValue("@groupId", groupId);
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StudentAttendanceGrade student = new StudentAttendanceGrade();
                        student.StudentID = reader.GetInt32("StudentID");
                        student.StudentName = reader.GetString("StudentFullName");
                        student.Grade = "";
                        student.IsPresent = false;
                        studentsList.Add(student);
                    }
                }
            }

            string queryGrades = @"SELECT StudentID, Grade FROM student_grades WHERE ScheduleID = @scheduleId";
            Dictionary<int, string> gradesDict = new Dictionary<int, string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(queryGrades, connection))
            {
                command.Parameters.AddWithValue("@scheduleId", scheduleId);
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32("StudentID");
                        string grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? "" : reader.GetString("Grade");
                        gradesDict[studentId] = grade;
                    }
                }
            }

            string queryAttendance = @"SELECT StudentID, IsPresent FROM student_attendance WHERE ScheduleID = @scheduleId";
            Dictionary<int, bool> attendanceDict = new Dictionary<int, bool>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(queryAttendance, connection))
            {
                command.Parameters.AddWithValue("@scheduleId", scheduleId);
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32("StudentID");
                        bool isPresent = reader.GetBoolean("IsPresent");
                        attendanceDict[studentId] = isPresent;
                    }
                }
            }

            foreach (var student in studentsList)
            {
                if (gradesDict.ContainsKey(student.StudentID))
                    student.Grade = gradesDict[student.StudentID];
                if (attendanceDict.ContainsKey(student.StudentID))
                    student.IsPresent = attendanceDict[student.StudentID];
            }

            dataGridStudents.ItemsSource = null;
            dataGridStudents.ItemsSource = studentsList;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ScheduleItem selectedSchedule = cmbSchedule.SelectedItem as ScheduleItem;
            if (selectedSchedule == null)
            {
                MessageBox.Show("Пожалуйста, выберите занятие.");
                return;
            }

            SaveAttendanceAndGrades(selectedSchedule.ScheduleID, studentsList);
            MessageBox.Show("Данные успешно сохранены.");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow pw = new AdminWindow();
            pw.Show();
            this.Close();
        }

        private void SaveAttendanceAndGrades(int scheduleId, List<StudentAttendanceGrade> data)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                foreach (var item in data)
                {
                    string queryGrade = @"
                        INSERT INTO student_grades (StudentID, ScheduleID, Grade, Date)
                        VALUES (@studentId, @scheduleId, @grade, CURDATE())
                        ON DUPLICATE KEY UPDATE Grade = @grade";

                    using (MySqlCommand cmd = new MySqlCommand(queryGrade, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", item.StudentID);
                        cmd.Parameters.AddWithValue("@scheduleId", scheduleId);
                        cmd.Parameters.AddWithValue("@grade", string.IsNullOrWhiteSpace(item.Grade) ? (object)DBNull.Value : item.Grade);
                        cmd.ExecuteNonQuery();
                    }

                    string queryAttendance = @"
                        INSERT INTO student_attendance (StudentID, ScheduleID, IsPresent, Date)
                        VALUES (@studentId, @scheduleId, @isPresent, CURDATE())
                        ON DUPLICATE KEY UPDATE IsPresent = @isPresent";

                    using (MySqlCommand cmd = new MySqlCommand(queryAttendance, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", item.StudentID);
                        cmd.Parameters.AddWithValue("@scheduleId", scheduleId);
                        cmd.Parameters.AddWithValue("@isPresent", item.IsPresent);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}

