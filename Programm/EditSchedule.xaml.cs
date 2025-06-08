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
using Mysqlx.Crud;
using System.Xml.Linq;

namespace VkrProgramm
{
    public class Group
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }

        public override string ToString() => GroupName;
    }

    public class Subject
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }

        public override string ToString() => SubjectName;
    }
    public class Teacher
    {
        public int TeacherID { get; set; }
        public string TeacherFullName { get; set; }

        public override string ToString() => TeacherFullName;
    }

    public partial class EditSchedule : Window
    {
        string connectionString = MySqlConnectionString.GetConnectionMySql();

        public EditSchedule()
        {
            InitializeComponent();
            LoadGroups();
            LoadSubjects();
            LoadTeachers();
            LoadTypes();
        }

        public void LoadGroups()
        {
            cmbGroup.Items.Clear();
            string query = "SELECT GroupID, GroupName FROM student_groups";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbGroup.Items.Add(new Group
                        {
                            GroupID = reader.GetInt32("GroupID"),
                            GroupName = reader.GetString("GroupName")
                        });
                    }
                }
            }
        }

        public void LoadSubjects()
        {
            cmbSubject.Items.Clear();
            string query = "SELECT SubjectID, SubjectName FROM subjects";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbSubject.Items.Add(new Subject
                        {
                            SubjectID = reader.GetInt32("SubjectID"),
                            SubjectName = reader.GetString("SubjectName")
                        });
                    }
                }
            }
        }

        public void LoadTeachers()
        {
            cmbTeacher.Items.Clear();
            string query = "SELECT TeacherID, TeacherFullName FROM teachers";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbTeacher.Items.Add(new Teacher
                        {
                            TeacherID = reader.GetInt32("TeacherID"),
                            TeacherFullName = reader.GetString("TeacherFullName")
                        });
                    }
                }
            }
        }

        public void LoadTypes()
        {
            cmbType.Items.Clear();
            cmbType.Items.Add(new ComboBoxItem() { Content = "Лекция" });
            cmbType.Items.Add(new ComboBoxItem() { Content = "Практика" });
            cmbType.Items.Add(new ComboBoxItem() { Content = "Лабораторная" });
            cmbType.Items.Add(new ComboBoxItem() { Content = "Семинар" });
        }

        private void btnSaveEdit_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGroup.SelectedItem == null || cmbSubject.SelectedItem == null || cmbTeacher.SelectedItem == null ||
                dpDate.SelectedDate == null || string.IsNullOrWhiteSpace(txtTime.Text) ||
                string.IsNullOrWhiteSpace(txtRoom.Text) || cmbType.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!");
                return;
            }

            try
            {
                var selectedGroup = cmbGroup.SelectedItem as Group;
                var selectedSubject = cmbSubject.SelectedItem as Subject;
                var selectedTeacher = cmbTeacher.SelectedItem as Teacher;
                var selectedType = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO schedule 
                        (GroupID, SubjectID, TeacherID, Date, Time, Room, Type, Comment) 
                        VALUES (@group, @subject, @teacher, @date, @time, @room, @type, @comment)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@group", selectedGroup.GroupID);
                        command.Parameters.AddWithValue("@subject", selectedSubject.SubjectID);
                        command.Parameters.AddWithValue("@teacher", selectedTeacher.TeacherID);
                        command.Parameters.AddWithValue("@date", dpDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@time", txtTime.Text);
                        command.Parameters.AddWithValue("@room", txtRoom.Text);
                        command.Parameters.AddWithValue("@type", selectedType);
                        command.Parameters.AddWithValue("@comment", txtComment.Text);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Расписание успешно сохранено!");
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGroup.SelectedItem == null || cmbSubject.SelectedItem == null || dpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите группу, предмет и дату для удаления!");
                return;
            }

            var result = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var selectedGroup = cmbGroup.SelectedItem as Group;
                var selectedSubject = cmbSubject.SelectedItem as Subject;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"DELETE FROM schedule 
                                     WHERE GroupID = @group AND SubjectID = @subject AND Date = @date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@group", selectedGroup.GroupID);
                        command.Parameters.AddWithValue("@subject", selectedSubject.SubjectID);
                        command.Parameters.AddWithValue("@date", dpDate.SelectedDate.Value.ToString("yyyy-MM-dd"));

                        int rows = command.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Запись удалена!");
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Запись не найдена.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении: " + ex.Message);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow pw = new AdminWindow();
            pw.Show();
            this.Close();
        }

        private void ClearFields()
        {
            cmbGroup.SelectedIndex = -1;
            cmbSubject.SelectedIndex = -1;
            cmbTeacher.SelectedIndex = -1;
            dpDate.SelectedDate = null;
            txtTime.Clear();
            txtRoom.Clear();
            cmbType.SelectedIndex = -1;
            txtComment.Clear();
        }
    }
}
