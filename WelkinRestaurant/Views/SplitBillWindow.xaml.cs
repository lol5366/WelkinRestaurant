using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WelkinRestaurant.DataAccess;
using MySql.Data.MySqlClient;
using WelkinRestaurant.Models;

namespace WelkinRestaurant.Views
{
    public partial class SplitBillWindow : Window
    {
        private ObservableCollection<SplitRow> rows;
        private List<string> guestOptions;
        private DatabaseHelper db = new DatabaseHelper();
        private int originalOrderId;
        private int tableId;
        private User currentUser;

        public List<string> GuestOptions => guestOptions;

        public SplitBillWindow(List<TempOrderItem> orderItems, int guestCount, int orderId, int tableId, User user)
        {
            InitializeComponent();
            DataContext = this;

            this.originalOrderId = orderId;
            this.tableId = tableId;
            this.currentUser = user;

            // Создаём список гостей (Гость 1, Гость 2, ...)
            guestOptions = new List<string>();
            for (int i = 1; i <= guestCount; i++)
            {
                guestOptions.Add($"Гость {i}");
            }

            // Создаём строки для каждого блюда (каждая порция отдельно)
            rows = new ObservableCollection<SplitRow>();
            foreach (var item in orderItems)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    rows.Add(new SplitRow
                    {
                        ItemName = item.ItemName,
                        Quantity = 1,
                        Price = item.Price,
                        SelectedGuest = "Гость 1"
                    });
                }
            }

            dgItems.ItemsSource = rows;
            UpdateTotals();

            // Подписываемся на окончание редактирования
            dgItems.CellEditEnding += DgItems_CellEditEnding;
        }

        private void DgItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal total = rows.Sum(r => r.Price * r.Quantity);
            txtTotal.Text = $"Общая сумма: {total:F2} руб";

            // Проверяем, всё ли распределено
            var unassigned = rows.Where(r => string.IsNullOrEmpty(r.SelectedGuest)).ToList();
            if (unassigned.Any())
            {
                txtRemaining.Text = $"⚠️ Осталось распределить: {unassigned.Count} позиций";
                txtRemaining.Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                txtRemaining.Text = "✅ Все блюда распределены";
                txtRemaining.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, все ли строки распределены по гостям
            var unassigned = rows.Where(r => string.IsNullOrEmpty(r.SelectedGuest)).ToList();
            if (unassigned.Any())
            {
                MessageBox.Show($"Не все блюда распределены!\nОсталось: {unassigned.Count} позиций",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем, что количество порций положительное
            foreach (var row in rows)
            {
                if (row.Quantity <= 0)
                {
                    MessageBox.Show("Количество должно быть больше 0!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Группируем по гостям
            var ordersByGuest = new Dictionary<string, List<SplitRow>>();
            foreach (var guest in guestOptions)
            {
                ordersByGuest[guest] = new List<SplitRow>();
            }

            foreach (var row in rows)
            {
                ordersByGuest[row.SelectedGuest].Add(row);
            }

            try
            {
                // Удаляем исходный заказ
                db.ExecuteNonQuery("DELETE FROM order_items WHERE order_id = @oid",
                    new MySqlParameter("@oid", originalOrderId));
                db.ExecuteNonQuery("UPDATE orders SET status = 'cancelled' WHERE id = @oid",
                    new MySqlParameter("@oid", originalOrderId));

                // Создаём отдельный заказ для каждого гостя
                foreach (var guest in ordersByGuest.Keys)
                {
                    var guestRows = ordersByGuest[guest];
                    if (guestRows.Count == 0) continue;

                    // Получаем номер гостя
                    int guestNumber = int.Parse(guest.Split(' ')[1]);

                    // Создаём новый заказ
                    string insertOrder = @"INSERT INTO orders (table_id, waiter_id, status, created_at) 
                                           VALUES (@table_id, @waiter_id, 'paid', NOW());
                                           SELECT LAST_INSERT_ID();";
                    int newOrderId = Convert.ToInt32(db.ExecuteScalar(insertOrder,
                        new MySqlParameter("@table_id", tableId),
                        new MySqlParameter("@waiter_id", currentUser.Id)));

                    decimal total = 0;
                    foreach (var row in guestRows)
                    {
                        // Получаем ID блюда
                        string getIdQuery = "SELECT id FROM menu_items WHERE name = @name";
                        int menuItemId = Convert.ToInt32(db.ExecuteScalar(getIdQuery,
                            new MySqlParameter("@name", row.ItemName)));

                        db.ExecuteNonQuery(@"INSERT INTO order_items (order_id, menu_item_id, quantity, price_at_time) 
                                             VALUES (@oid, @mid, @qty, @price)",
                            new MySqlParameter("@oid", newOrderId),
                            new MySqlParameter("@mid", menuItemId),
                            new MySqlParameter("@qty", row.Quantity),
                            new MySqlParameter("@price", row.Price));

                        total += row.Price * row.Quantity;
                    }

                    // Обновляем сумму заказа
                    db.ExecuteNonQuery("UPDATE orders SET total = @total WHERE id = @oid",
                        new MySqlParameter("@total", total),
                        new MySqlParameter("@oid", newOrderId));

                    // Сохраняем оплату
                    db.ExecuteNonQuery(@"INSERT INTO payments (order_id, amount, payment_type, waiter_id, created_at) 
                                         VALUES (@oid, @amount, 'cash', @wid, NOW())",
                        new MySqlParameter("@oid", newOrderId),
                        new MySqlParameter("@amount", total),
                        new MySqlParameter("@wid", currentUser.Id));

                    // Сохраняем чек
                    SaveReceiptForGuest(total, guestNumber, guestRows);
                }

                // Освобождаем стол
                db.ExecuteNonQuery(@"UPDATE tables 
                                    SET status = 'free', current_waiter_id = NULL, current_order_id = NULL 
                                    WHERE id = @tid",
                    new MySqlParameter("@tid", tableId));

                MessageBox.Show($"✅ Счёт разделён на {guestOptions.Count} гостей!",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveReceiptForGuest(decimal total, int guestNumber, List<SplitRow> items)
        {
            try
            {
                string chequesDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Welkin_Cheques");
                if (!System.IO.Directory.Exists(chequesDir))
                    System.IO.Directory.CreateDirectory(chequesDir);

                string filename = System.IO.Path.Combine(chequesDir, $"receipt_table{tableId}_guest{guestNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine("╔════════════════════════════════════╗");
                    writer.WriteLine("║           WELKIN RESTAURANT         ║");
                    writer.WriteLine("╠════════════════════════════════════╣");
                    writer.WriteLine($"║ Стол: {tableId,-28}║");
                    writer.WriteLine($"║ Гость: {guestNumber,-27}║");
                    writer.WriteLine($"║ Официант: {currentUser.FullName,-25}║");
                    writer.WriteLine($"║ Дата: {DateTime.Now:dd.MM.yyyy HH:mm,-25}║");
                    writer.WriteLine("╠════════════════════════════════════╣");

                    foreach (var item in items)
                    {
                        writer.WriteLine($"║ {item.ItemName,-20} x{item.Quantity} = {item.Price * item.Quantity,6:F2} ║");
                    }

                    writer.WriteLine("╠════════════════════════════════════╣");
                    writer.WriteLine($"║ ИТОГО: {total,24:F2} ║");
                    writer.WriteLine("╚════════════════════════════════════╝");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении чека: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }

    // Строка распределения
    public class SplitRow : System.ComponentModel.INotifyPropertyChanged
    {
        public string ItemName { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public decimal Price { get; set; }

        private string _selectedGuest;
        public string SelectedGuest
        {
            get => _selectedGuest;
            set
            {
                _selectedGuest = value;
                OnPropertyChanged(nameof(SelectedGuest));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}