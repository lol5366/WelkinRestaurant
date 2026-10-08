using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WelkinRestaurant.DataAccess;
using WelkinRestaurant.Models;
using WelkinRestaurant.Tests;


namespace WelkinRestaurant.Views
{
    public partial class MainWindow : Window
    {
        private User currentUser;
        private DatabaseHelper db = new DatabaseHelper();

        public MainWindow(User user)
        {
            InitializeComponent();
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => UpdateStatusBar();
            timer.Start();

            UpdateStatusBar();

            currentUser = user;
            txtUserInfo.Text = $"{user.FullName} ({user.Role})";
            if (user.Role == "admin")
            {
                btnCloseShift.Visibility = Visibility.Visible;
                btnStock.Visibility = Visibility.Visible;
                btnFunctionalTests.Visibility = Visibility.Visible;
                btnEmployees.Visibility = Visibility.Visible;
                rightPanel.Visibility = Visibility.Visible;
            }
            else
            {
                rightPanel.Visibility = Visibility.Collapsed;

                btnCloseShift.Visibility = Visibility.Collapsed;
                btnStock.Visibility = Visibility.Collapsed;
            }

            LoadTables();
            UpdateStatistics();
        }
        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            var employeesWindow = new EmployeesWindow();
            employeesWindow.Owner = this; 
            employeesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            employeesWindow.ShowDialog();
        }

        private void UpdateStatusBar()
        {
            // Проверяем, что элементы существуют и currentUser не null
            if (txtUserStatus != null && currentUser != null)
                txtUserStatus.Text = $"{currentUser.FullName} ({currentUser.Role})";

            if (txtDateStatus != null)
                txtDateStatus.Text = DateTime.Now.ToString("dd.MM.yyyy");

            if (txtTimeStatus != null)
                txtTimeStatus.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private void UpdateStatistics()
        {
            try
            {
                // Общее количество столов
                DataTable dtTotal = db.ExecuteQuery("SELECT COUNT(*) as count FROM tables");
                int total = Convert.ToInt32(dtTotal.Rows[0]["count"]);
                txtTotalTables.Text = total.ToString();

                // Свободные столы
                DataTable dtFree = db.ExecuteQuery("SELECT COUNT(*) as count FROM tables WHERE status = 'free'");
                int free = Convert.ToInt32(dtFree.Rows[0]["count"]);
                txtFreeTables.Text = free.ToString();

                // Занятые столы
                DataTable dtOccupied = db.ExecuteQuery("SELECT COUNT(*) as count FROM tables WHERE status = 'opened'");
                int occupied = Convert.ToInt32(dtOccupied.Rows[0]["count"]);
                txtOccupiedTables.Text = occupied.ToString();

                // ВЫРУЧКА ЗА ТЕКУЩУЮ СМЕНУ (не за весь день)
                string getShiftQuery = "SELECT opened_at FROM shifts WHERE closed_at IS NULL ORDER BY id DESC LIMIT 1";
                DataTable shiftDt = db.ExecuteQuery(getShiftQuery);

                if (shiftDt.Rows.Count > 0)
                {
                    DateTime shiftOpenedAt = Convert.ToDateTime(shiftDt.Rows[0]["opened_at"]);

                    DataTable dtRevenue = db.ExecuteQuery(@"SELECT IFNULL(SUM(p.amount), 0) as total 
                                                   FROM payments p
                                                   JOIN orders o ON p.order_id = o.id
                                                   WHERE o.created_at >= @shift_start",
                        new MySqlParameter("@shift_start", shiftOpenedAt));

                    decimal revenue = Convert.ToDecimal(dtRevenue.Rows[0]["total"]);
                    txtTodayRevenue.Text = $"{revenue:F2} руб";
                }
                else
                {
                    txtTodayRevenue.Text = "0,00 руб";
                }
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки
            }
        }


        private void OpenTests_Click(object sender, RoutedEventArgs e)
        {
            var testWindow = new TestRunnerWindow();
            testWindow.Show();
        }

        private void LoadTables()
        {
            panelTables.Children.Clear();

            string query = @"SELECT t.*, u.full_name as waiter_name 
                     FROM tables t
                     LEFT JOIN users u ON t.current_waiter_id = u.id
                     ORDER BY t.number";

            DataTable dt = db.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                var table = new Table
                {
                    Id = Convert.ToInt32(row["id"]),
                    Number = Convert.ToInt32(row["number"]),
                    Status = row["status"].ToString(),
                    CurrentWaiterId = row["current_waiter_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["current_waiter_id"]),
                    CurrentOrderId = row["current_order_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["current_order_id"]),
                    WaiterName = row["waiter_name"]?.ToString()
                };

                Button btnTable = new Button
                {
                    Width = 140,
                    Height = 110,
                    Margin = new Thickness(10),
                    Tag = table,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                // Применяем стиль из App.xaml
                btnTable.Style = (Style)Application.Current.Resources[typeof(Button)];

                if (table.Status == "free")
                {
                    btnTable.Background = new SolidColorBrush(Color.FromRgb(37, 37, 37));
                    btnTable.Foreground = Brushes.White;
                    btnTable.Content = $"СТОЛ {table.Number}\nСВОБОДЕН";
                    btnTable.Click += OpenTable_Click;
                }
                else
                {
                    bool isCurrentWaiter = (table.CurrentWaiterId == currentUser.Id);
                    if (isCurrentWaiter)
                    {
                        btnTable.Background = new SolidColorBrush(Color.FromRgb(0, 120, 212));
                    }
                    else
                    {
                        btnTable.Background = new SolidColorBrush(Color.FromRgb(232, 17, 35));
                    }
                    btnTable.Foreground = Brushes.White;
                    btnTable.Content = $"СТОЛ {table.Number}\n{table.WaiterName}";
                    btnTable.Click += (s, e) => OpenExistingTable(table, isCurrentWaiter);
                }

                panelTables.Children.Add(btnTable);
                UpdateStatistics();
            }
        }

        private void OpenTable_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var table = btn.Tag as Table;


            // Создаём новый заказ
            string insertOrder = @"INSERT INTO orders (table_id, waiter_id, status) 
                                   VALUES (@table_id, @waiter_id, 'active');
                                   SELECT LAST_INSERT_ID();";

            int orderId = Convert.ToInt32(db.ExecuteScalar(insertOrder,
                new MySqlParameter("@table_id", table.Id),
                new MySqlParameter("@waiter_id", currentUser.Id)));

            // Обновляем стол
            db.ExecuteNonQuery(@"UPDATE tables 
                                SET status = 'opened', current_waiter_id = @waiter_id, current_order_id = @order_id 
                                WHERE id = @table_id",
                new MySqlParameter("@waiter_id", currentUser.Id),
                new MySqlParameter("@order_id", orderId),
                new MySqlParameter("@table_id", table.Id));

            var tableWindow = new TableWindow(table, currentUser, orderId, true);
            tableWindow.ShowDialog();
            LoadTables();
            UpdateStatistics();
        }

        private void OpenExistingTable(Table table, bool canEdit)
        {
            // Если пользователь админ - он может редактировать любой стол
            bool actualCanEdit = canEdit || (currentUser.Role == "admin");
            var tableWindow = new TableWindow(table, currentUser, table.CurrentOrderId.Value, actualCanEdit);
            tableWindow.ShowDialog();
            LoadTables();
        }

        private void CloseShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Проверяем, есть ли открытые столы
                string checkTablesQuery = "SELECT COUNT(*) FROM tables WHERE status = 'opened'";
                int openedTables = Convert.ToInt32(db.ExecuteScalar(checkTablesQuery));

                if (openedTables > 0)
                {
                    string getTablesList = @"SELECT t.number, u.full_name as waiter_name 
                                     FROM tables t
                                     LEFT JOIN users u ON t.current_waiter_id = u.id
                                     WHERE t.status = 'opened'";
                    DataTable openTables = db.ExecuteQuery(getTablesList);

                    string tablesList = "";
                    foreach (DataRow row in openTables.Rows)
                    {
                        int tableNum = Convert.ToInt32(row["number"]);
                        string waiterName = row["waiter_name"]?.ToString() ?? "неизвестно";
                        tablesList += $"  • Стол {tableNum} (официант: {waiterName})\n";
                    }

                    MessageBox.Show($"НЕВОЗМОЖНО ЗАКРЫТЬ СМЕНУ!\n\nОткрытые столы:\n{tablesList}\nПожалуйста, закройте все столы перед закрытием смены.",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Подтверждение закрытия смены
                var confirmResult = MessageBox.Show(
                    "ВНИМАНИЕ!\n\nВы действительно хотите закрыть смену?\n\nПосле закрытия смены:\n• Будет сформирован отчёт\n• Начнётся новая смена",
                    "Подтверждение закрытия смены",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmResult != MessageBoxResult.Yes)
                    return;

                // 3. Находим текущую открытую смену
                string getShiftQuery = "SELECT id, opened_at FROM shifts WHERE closed_at IS NULL ORDER BY id DESC LIMIT 1";
                DataTable shiftDt = db.ExecuteQuery(getShiftQuery);

                if (shiftDt.Rows.Count == 0)
                {
                    MessageBox.Show("Нет открытой смены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int shiftId = Convert.ToInt32(shiftDt.Rows[0]["id"]);
                DateTime shiftOpenedAt = Convert.ToDateTime(shiftDt.Rows[0]["opened_at"]);

                // 4. Получаем выручку за эту смену
                string query = @"SELECT u.full_name, p.payment_type, SUM(p.amount) as total
                 FROM payments p
                 JOIN users u ON p.waiter_id = u.id
                 WHERE DATE(p.created_at) = CURDATE()
                 GROUP BY u.id, p.payment_type";

                DataTable dt = db.ExecuteQuery(query, new MySqlParameter("@shift_start", shiftOpenedAt));

                // 5. Формируем отчёт
                string report = GenerateBeautifulReport(shiftOpenedAt, dt);

                // 6. Сохраняем отчёт на рабочий стол
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filename = System.IO.Path.Combine(desktopPath, $"shift_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                System.IO.File.WriteAllText(filename, report);

                // 7. Закрываем смену
                db.ExecuteNonQuery("UPDATE shifts SET closed_at = NOW(), closed_by_admin_id = @adminId WHERE id = @shiftId",
                    new MySqlParameter("@adminId", currentUser.Id),
                    new MySqlParameter("@shiftId", shiftId));

                // 8. Создаём новую смену
                db.ExecuteNonQuery(@"INSERT INTO shifts (opened_at, opened_by_admin_id) 
                             VALUES (NOW(), @adminId)",
                    new MySqlParameter("@adminId", currentUser.Id));

                // 9. Показываем сообщение об успехе
                MessageBox.Show($"Смена успешно закрыта!\n\nОтчёт сохранён: {filename}\nОбщая выручка: {CalculateTotalFromReport(report):F2} руб",
                    "Закрытие смены", MessageBoxButton.OK, MessageBoxImage.Information);

                // 10. Обновляем план столов и статистику
                LoadTables();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при закрытии смены: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string GenerateBeautifulReport(DateTime shiftOpenedAt, DataTable paymentsData)
        {
            var report = new System.Text.StringBuilder();

            report.AppendLine("╔════════════════════════════════════════════════════════════════╗");
            report.AppendLine("║                    WELKIN RESTAURANT                           ║");
            report.AppendLine("║                    ОТЧЁТ О ЗАКРЫТИИ СМЕНЫ                      ║");
            report.AppendLine("╠════════════════════════════════════════════════════════════════╣");
            report.AppendLine($"║  Смена открыта:  {shiftOpenedAt:dd.MM.yyyy HH:mm:ss}                                     ║");
            report.AppendLine($"║  Смена закрыта:  {DateTime.Now:dd.MM.yyyy HH:mm:ss}                                      ║");
            report.AppendLine($"║  Закрыл:         {currentUser.FullName}                                                ║");
            report.AppendLine("╠════════════════════════════════════════════════════════════════╣");
            report.AppendLine("║                                                              ║");
            report.AppendLine("║  ДЕТАЛИЗАЦИЯ ПО ОФИЦИАНТАМ И ТИПАМ ОПЛАТЫ:                    ║");
            report.AppendLine("║                                                              ║");

            decimal grandTotal = 0;
            decimal totalCash = 0;
            decimal totalCard = 0;
            decimal totalOnline = 0;
            var waiterTotals = new System.Collections.Generic.Dictionary<string, decimal>();
            var waiterCash = new System.Collections.Generic.Dictionary<string, decimal>();
            var waiterCard = new System.Collections.Generic.Dictionary<string, decimal>();
            var waiterOnline = new System.Collections.Generic.Dictionary<string, decimal>();

            foreach (DataRow row in paymentsData.Rows)
            {
                string waiter = row["full_name"].ToString();
                string type = row["payment_type"].ToString();
                decimal amount = Convert.ToDecimal(row["total"]);

                grandTotal += amount;

                if (!waiterTotals.ContainsKey(waiter))
                {
                    waiterTotals[waiter] = 0;
                    waiterCash[waiter] = 0;
                    waiterCard[waiter] = 0;
                    waiterOnline[waiter] = 0;
                }

                waiterTotals[waiter] += amount;

                if (type == "cash")
                {
                    totalCash += amount;
                    waiterCash[waiter] += amount;
                }
                else if (type == "card")
                {
                    totalCard += amount;
                    waiterCard[waiter] += amount;
                }
                else if (type == "online")
                {
                    totalOnline += amount;
                    waiterOnline[waiter] += amount;
                }
            }

            // Таблица по официантам
            report.AppendLine("║  ┌────────────────────────────────────────────────────────┐  ║");
            report.AppendLine("║  │ Официант          Наличные    Карта      Онлайн    Итого │  ║");
            report.AppendLine("║  ├────────────────────────────────────────────────────────┤  ║");

            foreach (var waiter in waiterTotals.Keys)
            {
                report.AppendLine($"║  │ {waiter,-18} {waiterCash[waiter],8:F2}   {waiterCard[waiter],7:F2}    {waiterOnline[waiter],6:F2}   {waiterTotals[waiter],8:F2} │  ║");
            }

            report.AppendLine("║  └────────────────────────────────────────────────────────┘  ║");
            report.AppendLine("║                                                              ║");
            report.AppendLine("╠════════════════════════════════════════════════════════════════╣");
            report.AppendLine("║                                                              ║");
            report.AppendLine("║  ИТОГОВАЯ ВЫРУЧКА ЗА СМЕНУ:                                   ║");
            report.AppendLine($"║                                                              ║");
            report.AppendLine($"║     💵 Наличные:              {totalCash,30:F2} руб ║");
            report.AppendLine($"║     💳 Карта:                 {totalCard,30:F2} руб ║");
            report.AppendLine($"║     📱 Онлайн:                {totalOnline,30:F2} руб ║");
            report.AppendLine($"║     {'─' * 40}║");
            report.AppendLine($"║     💰 ОБЩАЯ ВЫРУЧКА:         {grandTotal,30:F2} руб ║");
            report.AppendLine("║                                                              ║");
            report.AppendLine("╚════════════════════════════════════════════════════════════════╝");

            return report.ToString();
        }

        private void ShowShiftClosedReport(string report, string filename)
        {
            // Создаём отдельное окно для отображения отчёта
            var reportWindow = new Window
            {
                Title = "Отчёт о закрытии смены",
                Width = 700,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = System.Windows.Media.Brushes.White
            };

            var grid = new System.Windows.Controls.Grid();
            grid.Margin = new Thickness(10);

            var textBox = new System.Windows.Controls.TextBox
            {
                Text = report,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12,
                Background = new SolidColorBrush(Color.FromRgb(37, 37, 37)),
                Foreground = System.Windows.Media.Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
                IsReadOnly = true,
                TextWrapping = System.Windows.TextWrapping.NoWrap,
                HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto
            };

            var closeButton = new System.Windows.Controls.Button
            {
                Content = "ЗАКРЫТЬ",
                Width = 100,
                Height = 32,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
                Foreground = System.Windows.Media.Brushes.White,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };

            closeButton.Click += (s, e) => reportWindow.Close();

            var stackPanel = new System.Windows.Controls.StackPanel();
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(closeButton);

            reportWindow.Content = stackPanel;
            reportWindow.ShowDialog();

            // Дополнительное уведомление
            MessageBox.Show(
                $"СМЕНА УСПЕШНО ЗАКРЫТА!\n\nФайл отчёта: {filename}\nОбщая выручка: {CalculateTotalFromReport(report):F2} руб",
                "Закрытие смены",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private decimal CalculateTotalFromReport(string report)
        {
            // Простой парсинг общей выручки из отчёта
            var lines = report.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("ОБЩАЯ ВЫРУЧКА"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+[\.,]\d+)");
                    if (match.Success)
                    {
                        return decimal.Parse(match.Value.Replace(".", ","));
                    }
                }
            }
            return 0;
        }
        private void Stock_Click(object sender, RoutedEventArgs e)
        {
            var stockWindow = new StockWindow();
            stockWindow.Owner = this;
            stockWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            stockWindow.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}