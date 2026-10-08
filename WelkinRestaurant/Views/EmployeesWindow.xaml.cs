using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using WelkinRestaurant.DataAccess;

namespace WelkinRestaurant.Views
{
    public partial class EmployeesWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();
        private ObservableCollection<EmployeeInfo> employees = new ObservableCollection<EmployeeInfo>();

        public EmployeesWindow()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            string query = @"
                SELECT 
                    u.full_name,
                    u.role,
                    COUNT(t.id) as tables_count,
                    IFNULL(GROUP_CONCAT(t.number ORDER BY t.number SEPARATOR ', '), '-') as table_numbers
                FROM users u
                LEFT JOIN tables t ON t.current_waiter_id = u.id AND t.status = 'opened'
                GROUP BY u.id
                ORDER BY u.role DESC, u.full_name";

            DataTable dt = db.ExecuteQuery(query);

            employees.Clear();
            foreach (DataRow row in dt.Rows)
            {
                employees.Add(new EmployeeInfo
                {
                    FullName = row["full_name"].ToString(),
                    Role = row["role"].ToString(),
                    TablesCount = row["tables_count"].ToString(),
                    TableNumbers = row["table_numbers"].ToString()
                });
            }

            dgEmployees.ItemsSource = employees;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class EmployeeInfo
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public string TablesCount { get; set; }
        public string TableNumbers { get; set; }
    }
}