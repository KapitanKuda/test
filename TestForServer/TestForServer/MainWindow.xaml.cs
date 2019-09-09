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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Collections.Specialized;

namespace TestForServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlDataAdapter adapter;
        DataTable Table;
        public MainWindow()
        {
            InitializeComponent();
            string connectionString;

            string sql = "SELECT * FROM test_bd";
            Table = new DataTable();
            SqlConnection connection = null;
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["testForServer.Properties.Settings.Database1ConnectionString"].ConnectionString;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры
                adapter.InsertCommand = new SqlCommand("add_value", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@value", SqlDbType.Int, 0, "Value"));
              

                connection.Open();
                adapter.Fill(Table);
                TestGrid.ItemsSource = Table.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url= "http://127.0.0.1:5000/take";
            RootObject response;
            using (var webClient = new WebClient())
            {
               
                var json = webClient.DownloadString(url);
                response = JsonConvert.DeserializeObject<RootObject>(json.Replace('[',' ').Replace(']',' '));

                MessageBox.Show(Convert.ToString(response.value));

            }
           

            Table.Rows.Add(new Object[] {response.value });
            UpdateDB();

        }
        private static readonly HttpClient client = new HttpClient();

        private void UpdateDB()
        {
            SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
            adapter.Update(Table);
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
            string url = "http://127.0.0.1:5000/add?name="; ;

            url = url + tb1.Text.Trim();
            using (var webClient = new WebClient())
            {
                // Создаём коллекцию параметров
                var pars = new NameValueCollection();

                // Добавляем необходимые параметры в виде пар ключ, значение
              

                // Посылаем параметры на сервер
                // Может быть ответ в виде массива байт
                var response = webClient.UploadValues(url, pars);
                string json=System.Text.Encoding.Default.GetString(response);
                var  respons = JsonConvert.DeserializeObject<RootObject>(json.Replace('[', ' ').Replace(']', ' '));

                MessageBox.Show(Convert.ToString(respons.value));
                if (Convert.ToInt16(respons.value) - Convert.ToInt16(tb1.Text) == 1)
                    MessageBox.Show("Всё верно");
                else
                    MessageBox.Show("эмммм откуда ошибка");
            }
        }
    }
}
