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
    public partial class WatchSchedule : Window
    {
        private int? _groupId = null;

        public WatchSchedule()
        {
            InitializeComponent();
            LoadSchedule();
        }

        public WatchSchedule(int groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            LoadSchedule();
        }

        private void LoadSchedule()
        {
            string query = @"
            SELECT 
              s.ScheduleID,
              g.GroupName AS 'Группа',
                subj.SubjectName AS 'Предмет',
             t.TeacherFullName AS 'Преподаватель',
             DATE_FORMAT(s.Date, '%d.%m.%Y') AS 'Дата',
             s.Time AS 'Время',
             s.Room AS 'Аудитория',
             s.Type AS 'Тип занятия',
              s.Comment AS 'Комментарий'
            FROM schedule s
            INNER JOIN student_groups g ON s.GroupID = g.GroupID
            INNER JOIN subjects subj ON s.SubjectID = subj.SubjectID
            INNER JOIN teachers t ON s.TeacherID = t.TeacherID
            ";
            if (_groupId.HasValue)
            {
                query += " WHERE s.GroupID = @groupId ";
            }
            query += " ORDER BY s.Date, s.Time";


            DataTable scheduleTable = new DataTable();
            string connectionString = MySqlConnectionString.GetConnectionMySql();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                if (_groupId.HasValue)
                    command.Parameters.AddWithValue("@groupId", _groupId.Value);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    scheduleTable.Load(reader);
                }
            }

            listSchedule.ItemsSource = new DataView(scheduleTable);
        }
    }
}
