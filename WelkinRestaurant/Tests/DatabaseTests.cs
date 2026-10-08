using System;
using System.Data;
using System.Windows;
using WelkinRestaurant.DataAccess;
using MySql.Data.MySqlClient;

namespace WelkinRestaurant.Tests
{
    public class DatabaseTests
    {
        private DatabaseHelper db = new DatabaseHelper();

        // Тест 1: Подключение к базе данных
        public bool TestConnection()
        {
            try
            {
                using (var conn = db.GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
                return false;
            }
        }

        // Тест 2: Проверка пользователей
        public bool TestUsers()
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT COUNT(*) as count FROM users");
                int count = Convert.ToInt32(dt.Rows[0]["count"]);

                if (count >= 3)
                {
                    MessageBox.Show($"Пользователей в БД: {count}");
                    return true;
                }
                else
                {
                    MessageBox.Show($"Пользователей太少: {count}, ожидается минимум 3");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка теста пользователей: {ex.Message}");
                return false;
            }
        }

        // Тест 3: Проверка меню
        public bool TestMenu()
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT COUNT(*) as count FROM menu_items");
                int count = Convert.ToInt32(dt.Rows[0]["count"]);

                if (count >= 20)
                {
                    MessageBox.Show($"Блюд в меню: {count}");
                    return true;
                }
                else
                {
                    MessageBox.Show($"Блюд太少: {count}, ожидается минимум 20");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка теста меню: {ex.Message}");
                return false;
            }
        }

        // Тест 4: Проверка столов
        public bool TestTables()
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT COUNT(*) as count FROM tables");
                int count = Convert.ToInt32(dt.Rows[0]["count"]);

                if (count >= 6)
                {
                    MessageBox.Show($"Столов в зале: {count}");
                    return true;
                }
                else
                {
                    MessageBox.Show($"Столов太少: {count}, ожидается минимум 6");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка теста столов: {ex.Message}");
                return false;
            }
        }

        // Тест 5: Проверка продуктов
        public bool TestProducts()
        {
            try
            {
                DataTable dt = db.ExecuteQuery("SELECT COUNT(*) as count FROM products");
                int count = Convert.ToInt32(dt.Rows[0]["count"]);

                if (count >= 8)
                {
                    MessageBox.Show($"Продуктов на складе: {count}");
                    return true;
                }
                else
                {
                    MessageBox.Show($"Продуктов太少: {count}, ожидается минимум 8");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка теста продуктов: {ex.Message}");
                return false;
            }
        }

        // Тест 6: Проверка авторизации (админ)
        public bool TestAdminLogin()
        {
            try
            {
                string query = "SELECT COUNT(*) as count FROM users WHERE login = 'admin' AND role = 'admin'";
                DataTable dt = db.ExecuteQuery(query);
                int count = Convert.ToInt32(dt.Rows[0]["count"]);

                if (count == 1)
                {
                    MessageBox.Show("Администратор найден");
                    return true;
                }
                else
                {
                    MessageBox.Show("Администратор не найден!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка теста админа: {ex.Message}");
                return false;
            }
        }

        // Запуск всех тестов
        public void RunAllTests()
        {
            string results = "=== РЕЗУЛЬТАТЫ ТЕСТИРОВАНИЯ ===\n\n";

            results += "1. Подключение к БД: " + (TestConnection() ? "✅ OK" : "❌ ОШИБКА") + "\n";
            results += "2. Пользователи: " + (TestUsers() ? "✅ OK" : "❌ ОШИБКА") + "\n";
            results += "3. Меню: " + (TestMenu() ? "✅ OK" : "❌ ОШИБКА") + "\n";
            results += "4. Столы: " + (TestTables() ? "✅ OK" : "❌ ОШИБКА") + "\n";
            results += "5. Продукты: " + (TestProducts() ? "✅ OK" : "❌ ОШИБКА") + "\n";
            results += "6. Администратор: " + (TestAdminLogin() ? "✅ OK" : "❌ ОШИБКА") + "\n";

            MessageBox.Show(results, "Результаты тестирования", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}