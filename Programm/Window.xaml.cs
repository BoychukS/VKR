using MySql.Data.MySqlClient;
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

namespace VkrProgramm
{
    public partial class StudentWindow : Window
    {
        public StudentWindow()
        {
            InitializeComponent();
            LoadGroups();
        }

        public class Group
        {
            public int GroupID { get; set; }
            public string GroupName { get; set; }
            public override string ToString() => GroupName;
        }

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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mn = new MainWindow();
            mn.Show();
            this.Close();
        }

        private void btnWatchAttendanceAndGrades_Click(object sender, RoutedEventArgs e)
        {
            WatchAttendanceAndGrades ws = new WatchAttendanceAndGrades();
            ws.Show();
        }

        private void btnWatchSchedule_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = cmbGroups.SelectedItem as Group;
            if (selectedGroup == null)
            {
                MessageBox.Show("Пожалуйста, выберите группу!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            WatchSchedule ws = new WatchSchedule(selectedGroup.GroupID);
            ws.ShowDialog();
        }
    }
}
