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
using DataTable = System.Data.DataTable;

namespace VkrProgramm
{
    public partial class WatchStudents : Window
    {
        private int groupId;
        private string groupName;

        public WatchStudents(int groupId, string groupName)
        {
            InitializeComponent();
            this.groupId = groupId;
            this.groupName = groupName;
            txtGroupTitle.Text = $"Список студентов группы {groupName}";
            LoadStudents();
        }

        private void LoadStudents()
        {
            List<Student> students = new List<Student>();
            string connectionString = MySqlConnectionString.GetConnectionMySql();
            string query = "SELECT StudentID, StudentFullName, S_Adress, S_Phone FROM students WHERE GroupID = @groupId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@groupId", groupId);
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            StudentID = reader.GetInt32("StudentID"),
                            StudentFullName = reader.GetString("StudentFullName"),
                            S_Adress = reader.GetString("S_Adress"),
                            S_Phone = reader.GetString("S_Phone")
                        };
                        students.Add(student);
                    }
                }
            }
            dataGridStudents.ItemsSource = students;
        }
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string StudentFullName { get; set; }
        public string S_Adress { get; set; }
        public string S_Phone { get; set; }
    }
}
