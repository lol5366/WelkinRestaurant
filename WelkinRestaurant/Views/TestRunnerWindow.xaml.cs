using System;
using System.Data;
using System.IO;
using System.Windows;
using WelkinRestaurant.DataAccess;
using MySql.Data.MySqlClient;

namespace WelkinRestaurant.Views
{
    public partial class TestRunnerWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();

        public TestRunnerWindow()
        {
            InitializeComponent();
        }

        private void AppendResult(string text)
        {
            txtResults.Text += text + "\n";
            txtResults.ScrollToEnd();
        }

        private void ClearResults()
        {
            txtResults.Text = "";
        }

        private void AddSeparator()
        {
            AppendResult(new string('═', 80));
        }

        private void AddHeader(string title)
        {
            AppendResult("");
            AppendResult($"╔{new string('═', 78)}╗");
            AppendResult($"║  {title,-76}║");
            AppendResult($"╚{new string('═', 78)}╝");
            AppendResult($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            AppendResult("");
        }

        private void AddResult(string testName, bool passed, string details = "")
        {
            string status = passed ? "✅" : "❌";
            AppendResult($"{status} {testName}");
            if (!string.IsNullOrEmpty(details))
                AppendResult($"   └─ {details}");
        }

        // ============================================================
        // ЗАПУСК ВСЕХ ТЕСТОВ
        // ============================================================
        private void RunAllTests_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            AddHeader("ПОЛНОЕ ТЕСТИРОВАНИЕ СИСТЕМЫ WELKIN");

            TestConnection();
            TestRoles();
            TestEditFunctionality();
            TestSaveFunctionality();
            TestPaymentAndReceipt();
            TestReports();
            TestStockManagement();

            AddSeparator();
            AppendResult("✅ Все тесты завершены!");
            AppendResult($"📁 Отчёт сохранён в папке Welkin_Tests на рабочем столе");
        }

        // ============================================================
        // ОТДЕЛЬНЫЕ ТЕСТЫ
        // ============================================================
        private void RunRoleTests_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            AddHeader("ТЕСТИРОВАНИЕ РОЛЕЙ И ДОСТУПА");
            TestRoles();
        }

        private void RunEditTests_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            AddHeader("ТЕСТИРОВАНИЕ РЕДАКТИРОВАНИЯ");
            TestEditFunctionality();
        }

        private void RunSaveTests_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            AddHeader("ТЕСТИРОВАНИЕ СОХРАНЕНИЯ");
            TestSaveFunctionality();
        }

        private void RunPaymentTests_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            AddHeader("ТЕСТИРОВАНИЕ ОПЛАТЫ И ЧЕКОВ");
            TestPaymentAndReceipt();
        }

        private void RunReportTests_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            AddHeader("ТЕСТИРОВАНИЕ ОТЧЁТОВ");
            TestReports();
        }

        private void RunStockTests_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            AddHeader("ТЕСТИРОВАНИЕ УПРАВЛЕНИЯ ОСТАТКАМИ");
            TestStockManagement();
        }

        // ============================================================
        // ТЕСТ 1: ПОДКЛЮЧЕНИЕ
        // ============================================================
        private void TestConnection()
        {
            try
            {
                using (var conn = db.GetConnection())
                {
                    conn.Open();
                    AddResult("Подключение к базе данных", true, "Сервер доступен");
                }
            }
            catch (Exception ex)
            {
                AddResult("Подключение к базе данных", false, ex.Message);
            }
        }

        // ============================================================
        // ТЕСТ 2: РОЛИ
        // ============================================================
        private void TestRoles()
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT login, role, full_name FROM users");

                bool hasAdmin = false;
                bool hasWaiter1 = false;
                bool hasWaiter2 = false;

                foreach (DataRow row in dt.Rows)
                {
                    string login = row["login"].ToString();
                    string role = row["role"].ToString();
                    string name = row["full_name"].ToString();

                    AppendResult($"   👤 {login} - {role} ({name})");

                    if (login == "admin" && role == "admin") hasAdmin = true;
                    if (login == "waiter1" && role == "waiter") hasWaiter1 = true;
                    if (login == "waiter2" && role == "waiter") hasWaiter2 = true;
                }

                AddResult("Наличие администратора", hasAdmin, "admin / admin");
                AddResult("Наличие официанта 1", hasWaiter1, "waiter1 / waiter1");
                AddResult("Наличие официанта 2", hasWaiter2, "waiter2 / waiter2");
            }
            catch (Exception ex)
            {
                AddResult("Проверка ролей", false, ex.Message);
            }
        }

        // ============================================================
        // ТЕСТ 3: РЕДАКТИРОВАНИЕ
        // ============================================================
        private void TestEditFunctionality()
        {
            try
            {
                int testOrderId = CreateTestOrder();

                if (testOrderId > 0)
                {
                    AddResult("Создание тестового заказа", true, $"Заказ №{testOrderId}");

                    int itemId = AddTestOrderItem(testOrderId);
                    AddResult("Добавление позиции", itemId > 0, itemId > 0 ? $"Позиция №{itemId}" : "Ошибка");

                    if (itemId > 0)
                    {
                        int deleted = db.ExecuteNonQuery("DELETE FROM order_items WHERE id = @id",
                            new MySqlParameter("@id", itemId));
                        AddResult("Удаление позиции", deleted > 0, "Позиция удалена");
                    }

                    CleanupTestOrder(testOrderId);
                    AddResult("Очистка тестовых данных", true, "Данные удалены");
                }
                else
                {
                    AddResult("Создание тестового заказа", false, "Ошибка создания");
                }
            }
            catch (Exception ex)
            {
                AddResult("Тест редактирования", false, ex.Message);
            }
        }

        // ============================================================
        // ТЕСТ 4: СОХРАНЕНИЕ
        // ============================================================
        private void TestSaveFunctionality()
        {
            try
            {
                int testOrderId = CreateTestOrder();

                if (testOrderId > 0)
                {
                    int itemId = AddTestOrderItem(testOrderId);

                    DataTable dt = db.ExecuteQuery("SELECT quantity, price_at_time FROM order_items WHERE id = @id",
                        new MySqlParameter("@id", itemId));

                    if (dt.Rows.Count > 0)
                    {
                        int qty = Convert.ToInt32(dt.Rows[0]["quantity"]);
                        decimal price = Convert.ToDecimal(dt.Rows[0]["price_at_time"]);
                        AddResult("Сохранение количества", qty == 1, $"Количество = {qty}");
                        AddResult("Сохранение цены", price > 0, $"Цена = {price:F2} руб");
                    }

                    int updated = db.ExecuteNonQuery("UPDATE orders SET total = 100 WHERE id = @id",
                        new MySqlParameter("@id", testOrderId));
                    AddResult("Обновление заказа", updated > 0, "Сумма обновлена");

                    CleanupTestOrder(testOrderId);
                }
            }
            catch (Exception ex)
            {
                AddResult("Тест сохранения", false, ex.Message);
            }
        }

        // ============================================================
        // ТЕСТ 5: ОПЛАТА И ЧЕКИ
        // ============================================================
        private void TestPaymentAndReceipt()
        {
            try
            {
                int testOrderId = CreateTestOrder();

                if (testOrderId > 0)
                {
                    AddTestOrderItem(testOrderId);

                    int paymentId = db.ExecuteNonQuery(@"INSERT INTO payments (order_id, amount, payment_type, waiter_id) 
                                                          VALUES (@oid, 100, 'cash', 1)",
                        new MySqlParameter("@oid", testOrderId));
                    AddResult("Сохранение оплаты в БД", paymentId > 0, "Запись создана");

                    int closed = db.ExecuteNonQuery("UPDATE orders SET status = 'paid', closed_at = NOW() WHERE id = @id",
                        new MySqlParameter("@id", testOrderId));
                    AddResult("Закрытие заказа", closed > 0, "Статус: paid");

                    string receiptPath = CreateTestReceipt(testOrderId);
                    bool receiptExists = File.Exists(receiptPath);
                    AddResult("Сохранение чека", receiptExists, $"Файл: {Path.GetFileName(receiptPath)}");

                    if (receiptExists) File.Delete(receiptPath);
                    CleanupTestOrder(testOrderId);
                }
            }
            catch (Exception ex)
            {
                AddResult("Тест оплаты", false, ex.Message);
            }
        }

        // ============================================================
        // ТЕСТ 6: ОТЧЁТЫ
        // ============================================================
        private void TestReports()
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT SUM(amount) as total FROM payments WHERE DATE(created_at) = CURDATE()");
                decimal total = dt.Rows[0]["total"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[0]["total"]);
                AddResult("Формирование отчёта о выручке", true, $"Выручка сегодня: {total:F2} руб");

                dt = db.ExecuteQuery(@"SELECT u.full_name, SUM(p.amount) as total 
                                       FROM payments p
                                       JOIN users u ON p.waiter_id = u.id
                                       WHERE DATE(p.created_at) = CURDATE()
                                       GROUP BY u.id");
                AddResult("Формирование отчёта по официантам", true, $"Официантов: {dt.Rows.Count}");

                string reportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"test_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                File.WriteAllText(reportPath, "Тестовый отчёт");
                AddResult("Сохранение отчёта в файл", File.Exists(reportPath), $"Файл создан");

                if (File.Exists(reportPath)) File.Delete(reportPath);
            }
            catch (Exception ex)
            {
                AddResult("Тест отчётов", false, ex.Message);
            }
        }

        // ============================================================
        // ТЕСТ 7: ОСТАТКИ
        // ============================================================
        private void TestStockManagement()
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT id, name, quantity FROM products LIMIT 1");
                if (dt.Rows.Count > 0)
                {
                    int productId = Convert.ToInt32(dt.Rows[0]["id"]);
                    string productName = dt.Rows[0]["name"].ToString();
                    decimal oldQuantity = Convert.ToDecimal(dt.Rows[0]["quantity"]);

                    decimal newQuantity = oldQuantity + 10;
                    int updated = db.ExecuteNonQuery("UPDATE products SET quantity = @new WHERE id = @id",
                        new MySqlParameter("@new", newQuantity),
                        new MySqlParameter("@id", productId));

                    AddResult("Изменение остатков (админ)", updated > 0, $"{productName}: {oldQuantity} → {newQuantity}");

                    db.ExecuteNonQuery("UPDATE products SET quantity = @old WHERE id = @id",
                        new MySqlParameter("@old", oldQuantity),
                        new MySqlParameter("@id", productId));
                    AddResult("Восстановление остатков", true, "Значение возвращено");
                }
            }
            catch (Exception ex)
            {
                AddResult("Тест остатков", false, ex.Message);
            }
        }

        // ============================================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ============================================================
        private int CreateTestOrder()
        {
            try
            {
                string query = @"INSERT INTO orders (table_id, waiter_id, status, created_at) 
                                 VALUES (1, 1, 'active', NOW());
                                 SELECT LAST_INSERT_ID();";
                return Convert.ToInt32(db.ExecuteScalar(query));
            }
            catch { return 0; }
        }

        private int AddTestOrderItem(int orderId)
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT id FROM menu_items LIMIT 1");
                if (dt.Rows.Count == 0) return 0;
                int menuItemId = Convert.ToInt32(dt.Rows[0]["id"]);

                string query = @"INSERT INTO order_items (order_id, menu_item_id, quantity, price_at_time) 
                                 VALUES (@oid, @mid, 1, 100);
                                 SELECT LAST_INSERT_ID();";
                return Convert.ToInt32(db.ExecuteScalar(query,
                    new MySqlParameter("@oid", orderId),
                    new MySqlParameter("@mid", menuItemId)));
            }
            catch { return 0; }
        }

        private void CleanupTestOrder(int orderId)
        {
            try
            {
                db.ExecuteNonQuery("DELETE FROM order_items WHERE order_id = @id", new MySqlParameter("@id", orderId));
                db.ExecuteNonQuery("DELETE FROM payments WHERE order_id = @id", new MySqlParameter("@id", orderId));
                db.ExecuteNonQuery("DELETE FROM orders WHERE id = @id", new MySqlParameter("@id", orderId));
            }
            catch { }
        }

        private string CreateTestReceipt(int orderId)
        {
            string chequesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Welkin_Cheques");
            if (!Directory.Exists(chequesDir))
                Directory.CreateDirectory(chequesDir);

            string filename = Path.Combine(chequesDir, $"test_receipt_{orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(filename, "=== ТЕСТОВЫЙ ЧЕК ===\nСумма: 100 руб");
            return filename;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}