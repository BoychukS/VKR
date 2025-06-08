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
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadGroups();
            LoadGroupsForStudents();
        }
        private string connectionString = MySqlConnectionString.GetConnectionMySql();
        public class Group
        {
            public int GroupID { get; set; }
            public string GroupName { get; set; }
            public override string ToString() => GroupName;
        }
        private void LoadGroups()
        {
            cmbGroupFilter.Items.Clear();
            string query = "SELECT GroupID, GroupName FROM student_groups ORDER BY GroupName";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbGroupFilter.Items.Add(new Group
                        {
                            GroupID = reader.GetInt32("GroupID"),
                            GroupName = reader.GetString("GroupName")
                        });
                    }
                }
            }
        }

        private void LoadGroupsForStudents()
        {
            cmbGroupStudents.Items.Clear();
            string query = "SELECT GroupID, GroupName FROM student_groups ORDER BY GroupName";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Group group = new Group
                        {
                            GroupID = reader.GetInt32("GroupID"),
                            GroupName = reader.GetString("GroupName")
                        };
                        cmbGroupStudents.Items.Add(group);
                    }
                }
            }
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mn = new MainWindow();
            mn.Show();
            this.Close();
        }

        private void btnWatchSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGroupFilter.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите группу.");
                return;
            }

            var selectedGroup = cmbGroupFilter.SelectedItem as Group;

            var watchOrderWindow = new WatchSchedule(selectedGroup.GroupID);
            watchOrderWindow.Show();
        }


        private void btnWatchEmp_Click(object sender, RoutedEventArgs e)
        {
            Group selectedGroup = cmbGroupStudents.SelectedItem as Group;
            if (selectedGroup == null)
            {
                MessageBox.Show("Пожалуйста, выберите группу!");
                return;
            }
            WatchStudents wnd = new WatchStudents(selectedGroup.GroupID, selectedGroup.GroupName);
            wnd.ShowDialog();

        }

        private void btnWatchPrep_Click(object sender, RoutedEventArgs e)
        {
            WatchAttendanceAndGrades ws = new WatchAttendanceAndGrades();
            ws.Show();
        }

        private void btnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            EditSchedule eo = new EditSchedule();
            eo.Show();
            this.Close();
        }

        private void btnEditEmp_Click(object sender, RoutedEventArgs e)
        {
            EditEmployee ee = new EditEmployee();
            ee.Show();
            this.Close();
        }

        private void btnEditPrep_Click(object sender, RoutedEventArgs e)
        {
            EditAttendanceAndGrades ep = new EditAttendanceAndGrades();
            ep.Show();
            this.Close();
        }
    }
}
