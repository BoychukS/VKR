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
using MySql.Data.MySqlClient;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Data;
using LiveCharts;
using LiveCharts.Wpf;
using DataTable = System.Data.DataTable;

namespace VkrProgramm
{
    public partial class WatchAttendanceAndGrades : Window
    {
        public WatchAttendanceAndGrades()
        {
            InitializeComponent();
            DataContext = this;
            LoadGroups();
        }

        public class Group
        {
            public int GroupID { get; set; }
            public string GroupName { get; set; }
            public override string ToString() => GroupName;
        }

        public class Student
        {
            public int StudentID { get; set; }
            public string StudentFullName { get; set; }
            public override string ToString() => StudentFullName;
        }

        public class AttendanceGrade
        {
            public DateTime Date { get; set; }
            public string Subject { get; set; }
            public bool IsPresent { get; set; }
            public string Grade { get; set; }
        }

        public SeriesCollection AttendanceSeries { get; set; } = new SeriesCollection();
        public List<string> AttendanceLabels { get; set; } = new List<string>();
        public SeriesCollection GradesSeries { get; set; } = new SeriesCollection();
        public List<string> GradesLabels { get; set; } = new List<string>();

        private void LoadGroups()
        {
            try
            {
                string connectionString = MySqlConnectionString.GetConnectionMySql();
                string query = "SELECT GroupID, GroupName FROM student_groups ORDER BY GroupName";
                List<Group> groups = new List<Group>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            groups.Add(new Group
                            {
                                GroupID = reader.GetInt32("GroupID"),
                                GroupName = reader.GetString("GroupName")
                            });
                        }
                    }
                }

                cmbGroups.ItemsSource = groups;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGroup = cmbGroups.SelectedItem as Group;
            if (selectedGroup != null)
            {
                LoadStudents(selectedGroup.GroupID);
            }
            else
            {
                cmbStudents.ItemsSource = null;
                dataGridAttendanceGrades.ItemsSource = null;
                ClearCharts();
            }
        }

        private void LoadStudents(int groupId)
        {
            try
            {
                string connectionString = MySqlConnectionString.GetConnectionMySql();
                string query = "SELECT StudentID, StudentFullName FROM students WHERE GroupID = @groupId ORDER BY StudentFullName";
                List<Student> students = new List<Student>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@groupId", groupId);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new Student
                            {
                                StudentID = reader.GetInt32("StudentID"),
                                StudentFullName = reader.GetString("StudentFullName")
                            });
                        }
                    }
                }

                cmbStudents.ItemsSource = students;
                dataGridAttendanceGrades.ItemsSource = null;
                ClearCharts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedStudent = cmbStudents.SelectedItem as Student;
            if (selectedStudent != null)
            {
                LoadAttendanceAndGrades(selectedStudent.StudentID);
            }
            else
            {
                dataGridAttendanceGrades.ItemsSource = null;
                ClearCharts();
            }
        }

        private void LoadAttendanceAndGrades(int studentId)
        {
            try
            {
                string connectionString = MySqlConnectionString.GetConnectionMySql();
                string query = @"
                    SELECT 
                        s.Date AS Date,
                        subj.SubjectName AS Subject,
                        sa.IsPresent AS IsPresent,
                        sg.Grade AS Grade
                    FROM schedule s
                    INNER JOIN subjects subj ON s.SubjectID = subj.SubjectID
                    LEFT JOIN student_attendance sa ON sa.ScheduleID = s.ScheduleID AND sa.StudentID = @studentId
                    LEFT JOIN student_grades sg ON sg.ScheduleID = s.ScheduleID AND sg.StudentID = @studentId
                    WHERE s.ScheduleID IN (
                        SELECT ScheduleID FROM student_attendance WHERE StudentID = @studentId
                        UNION
                        SELECT ScheduleID FROM student_grades WHERE StudentID = @studentId
                    )
                    ORDER BY s.Date";

                List<AttendanceGrade> records = new List<AttendanceGrade>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@studentId", studentId);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            records.Add(new AttendanceGrade
                            {
                                Date = reader.GetDateTime("Date"),
                                Subject = reader.GetString("Subject"),
                                IsPresent = reader.IsDBNull(reader.GetOrdinal("IsPresent")) ? false : reader.GetBoolean("IsPresent"),
                                Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? "" : reader.GetString("Grade")
                            });
                        }
                    }
                }

                dataGridAttendanceGrades.ItemsSource = records;
                UpdateCharts(records);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки посещаемости и оценок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ClearCharts();
            }
        }

        private void UpdateCharts(List<AttendanceGrade> records)
        {
            AttendanceLabels = new List<string>();
            var attendanceValues = new ChartValues<int>();

            GradesLabels = new List<string>();
            var gradesValues = new ChartValues<double>();

            foreach (var rec in records)
            {
                string dateStr = rec.Date.ToString("dd.MM.yyyy");
                AttendanceLabels.Add(dateStr);
                GradesLabels.Add(dateStr);
                attendanceValues.Add(rec.IsPresent ? 1 : 0);

                if (double.TryParse(rec.Grade, out double grade))
                    gradesValues.Add(grade);
                else
                    gradesValues.Add(0);
            }

            AttendanceSeries.Clear();
            GradesSeries.Clear();

            AttendanceSeries.Add(new ColumnSeries
            {
                Title = "Посещение",
                Values = attendanceValues,
                DataLabels = false
            });

            GradesSeries.Add(new LineSeries
            {
                Title = "Оценка",
                Values = gradesValues,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 10
            });

            attendanceChart.Series = AttendanceSeries;
            attendanceChart.AxisX[0].Labels = AttendanceLabels;

            gradesChart.Series = GradesSeries;
            gradesChart.AxisX[0].Labels = GradesLabels;
        }

        private void ClearCharts()
        {
            AttendanceSeries.Clear();
            GradesSeries.Clear();
            if (attendanceChart != null)
            {
                attendanceChart.Series = AttendanceSeries;
                attendanceChart.AxisX[0].Labels = new List<string>();
            }
            if (gradesChart != null)
            {
                gradesChart.Series = GradesSeries;
                gradesChart.AxisX[0].Labels = new List<string>();
            }
        }
    }
}


