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
    public partial class EditEmployee : Window
    {
        string connectionString = MySqlConnectionString.GetConnectionMySql();
        public EditEmployee()
        {
            InitializeComponent();
            EmpJob();
            EmpId();
        }
        private void rus(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^а-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void noEng(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z]");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void number(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
        public void EmpJob()
        {
            combEmp.Items.Clear();
            string query = "SELECT J_Title FROM JobTitle";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string postdName = reader.GetString(0);
                        combEmp.Items.Add(postdName);
                    }
                }
            }
        }
        public void EmpId()
        {
            combID.Items.Clear();
            string query = "SELECT U_ID FROM users";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int postdName = reader.GetInt32(0);
                        combID.Items.Add(postdName);
                    }
                }
            }
        }
        private void combID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int EmpId = Convert.ToInt32(combID.SelectedItem);

            string query = "select U_SURNAME from users where U_ID = '" + EmpId + "'";
            string query1 = "select U_NAME from users where U_ID = '" + EmpId + "'";
            string query2 = "select U_Patronymic from users where U_ID = '" + EmpId + "'";
            string query3 = "select U_PHONE from users where U_ID = '" + EmpId + "'";
            string query4 = "select Logins from users where U_ID = '" + EmpId + "'";
            string query5 = "select Passwords from users where U_ID = '" + EmpId + "'";
            string query6 = "select J_Title from users inner join JobTitle on users.JobTitleID = JobTitle.JobTitleID where U_ID = '" + EmpId + "'";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                MySqlCommand Sqlcmd = new MySqlCommand(query, connection);
                Sur.Text = Convert.ToString(Sqlcmd.ExecuteScalar());
                MySqlCommand Sqlcmd1 = new MySqlCommand(query1, connection);
                Name.Text = Convert.ToString(Sqlcmd1.ExecuteScalar());
                MySqlCommand Sqlcmd2 = new MySqlCommand(query2, connection);
                FIO.Text = Convert.ToString(Sqlcmd2.ExecuteScalar());
                MySqlCommand Sqlcmd3 = new MySqlCommand(query3, connection);
                phone.Text = Convert.ToString(Sqlcmd3.ExecuteScalar());
                MySqlCommand Sqlcmd4 = new MySqlCommand(query4, connection);
                txtLogin.Text = Convert.ToString(Sqlcmd4.ExecuteScalar());
                MySqlCommand Sqlcmd5 = new MySqlCommand(query5, connection);
                txtPassword.Text = Convert.ToString(Sqlcmd5.ExecuteScalar());

                MySqlCommand Sqlcmd6 = new MySqlCommand(query6, connection);
                combEmp.SelectedItem = Convert.ToString(Sqlcmd6.ExecuteScalar());
                connection.Close();
            }
        }
        private void btnEditEmp_Click(object sender, RoutedEventArgs e)
        {
            combID.Visibility = Visibility.Visible;
            txtID.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
            btnSaveEdit.Visibility = Visibility.Visible;
            btnNoEditEmp.Visibility = Visibility.Visible;
            btnEditEmp.Visibility = Visibility.Hidden;
            btnAddNew.Visibility = Visibility.Hidden;
            btnEditEmp.Visibility = Visibility.Hidden;
            EmpJob();
            EmpId();
        }
        private void btnNoEditEmp_Click(object sender, RoutedEventArgs e)
        {
            combID.Visibility = Visibility.Hidden;
            txtID.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Hidden;
            btnSaveEdit.Visibility = Visibility.Hidden;
            btnNoEditEmp.Visibility = Visibility.Hidden;
            btnEditEmp.Visibility = Visibility.Visible;
            btnAddNew.Visibility = Visibility.Visible;
            btnEditEmp.Visibility = Visibility.Visible;
            EmpJob();
            EmpId();
        }
        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (Sur.Text == "" || Name.Text == "" || FIO.Text == "" || phone.Text == "" || txtLogin.Text == "" || txtPassword.Text == "" || combEmp.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не заполнили все данные");
            }
            else
            {
                MySqlConnection saveEmp = new MySqlConnection(connectionString);
                try
                {
                    saveEmp.Open();

                    string saveQuery1 = "SELECT JobTitleID FROM JobTitle WHERE J_Title ='" + combEmp.SelectedItem + "'";
                    MySqlCommand save1 = new MySqlCommand(saveQuery1, saveEmp);
                    string saveJobID = Convert.ToString(save1.ExecuteScalar());

                    string saveEmployee = "Insert into users (U_NAME,U_SURNAME,U_Patronymic,U_PHONE,JobTitleID,Logins,Passwords) values ('" + Name.Text + 
                        "','" + Sur.Text + "','" + FIO.Text + "','" + phone.Text + "','" + saveJobID + "','" + txtLogin.Text + "','" + txtPassword.Text + "')";
                    MySqlCommand se = new MySqlCommand(saveEmployee, saveEmp); se.ExecuteNonQuery();

                    saveEmp.Close();

                    combEmp.SelectedIndex = -1;
                    combID.SelectedIndex = -1;
                    Sur.Text = "";
                    Name.Text = "";
                    FIO.Text = "";
                    phone.Text = "";
                    txtLogin.Text = "";
                    txtPassword.Text = "";
                    EmpId();
                    MessageBox.Show("Данные занесены в БД");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
                finally
                {
                }
            }
        }
        private void btnSaveEdit_Click(object sender, RoutedEventArgs e)
        {
            if (combID.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали код сотрудника!");
            }
            else
            {
                if (Sur.Text == "" || Name.Text == "" || FIO.Text == "" || phone.Text == "" || txtLogin.Text == "" || txtPassword.Text == "" || combEmp.SelectedIndex == -1)
                {
                    MessageBox.Show("Вы не заполнили все данные");
                }
                else
                {
                    MySqlConnection saveEmp = new MySqlConnection(connectionString);

                    try
                    {
                        saveEmp.Open();

                        string saveQuery1 = "SELECT JobTitleID FROM JobTitle WHERE J_Title ='" + combEmp.SelectedItem + "'";
                        MySqlCommand save1 = new MySqlCommand(saveQuery1, saveEmp);
                        string saveJobID = Convert.ToString(save1.ExecuteScalar());

                        string updateEmp = "Update users set U_NAME = '" + Name.Text + "', U_SURNAME = '" + Sur.Text + "', U_Patronymic = '" + FIO.Text +
                            "', U_PHONE = '" + phone.Text + "', JobTitleID = '" + saveJobID + "', Logins = '" + txtLogin.Text + "', Passwords = '" +
                            txtPassword.Text + "' where U_ID =" + combID.SelectedItem;
                        MySqlCommand ue = new MySqlCommand(updateEmp, saveEmp); ue.ExecuteNonQuery();

                        saveEmp.Close();

                        combEmp.SelectedIndex = -1;
                        combID.SelectedIndex = -1;
                        Sur.Text = "";
                        Name.Text = "";
                        FIO.Text = "";
                        phone.Text = "";
                        txtLogin.Text = "";
                        txtPassword.Text = "";
                        EmpId();

                        MessageBox.Show("Данные обновлены");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw;
                    }
                    finally
                    {
                    }
                }
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (combID.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали код сотрудника!");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены что хотите удалить данного сотрудника?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    string query = "DELETE FROM users WHERE U_ID = @U_ID";

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@U_ID", Convert.ToString(combID.SelectedItem));

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            MessageBox.Show("Сотрудник удален");
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("SQL Error: " + ex.Message);
                        }
                    }
                    combEmp.SelectedIndex = -1;
                    combID.SelectedIndex = -1;
                    Sur.Text = "";
                    Name.Text = "";
                    FIO.Text = "";
                    phone.Text = "";
                    txtLogin.Text = "";
                    txtPassword.Text = "";
                    EmpId();
                }
                else if (result == MessageBoxResult.No)
                {
                }
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow pw = new AdminWindow();
            pw.Show();
            this.Close();
        }

        private void combEmp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
