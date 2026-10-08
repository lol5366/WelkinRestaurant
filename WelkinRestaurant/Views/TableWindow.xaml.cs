using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WelkinRestaurant.DataAccess;
using WelkinRestaurant.Models;
using MenuItemModel = WelkinRestaurant.Models.MenuItem;
using SysMenuItem = System.Windows.Controls.MenuItem;
using Newtonsoft.Json;


namespace WelkinRestaurant.Views
{
    public partial class TableWindow : Window
    {
        private Table currentTable;
        private User currentUser;
        private int orderId;
        private bool canEdit;
        private bool editMode = false;
        private bool hasUnsavedChanges = false;
        private const decimal MARKUP_PERCENT = 300;

        private List<TempOrderItem> tempItems = new List<TempOrderItem>();
        private DatabaseHelper db = new DatabaseHelper();
        private List<TempOrderItem> backupItems = new List<TempOrderItem>();

        private decimal ApplyMarkup(decimal originalPrice)
        {
            return originalPrice * (1 + MARKUP_PERCENT / 100);
        }

        public TableWindow(Table table, User user, int orderId, bool canEdit)
        {
            InitializeComponent();
            this.Closing += TableWindow_Closing;
            lvOrderItems.Background = new SolidColorBrush(Color.FromRgb(37, 37, 37));

            currentTable = table;
            currentUser = user;
            this.orderId = orderId;
            this.canEdit = canEdit;

            Title = $"Стол {table.Number} - Обслуживает: {table.WaiterName ?? user.FullName}";
            bool canDelete = (currentUser.Role == "admin");

            if (!canEdit)
            {
                btnSave.IsEnabled = false;
                btnEdit.IsEnabled = false;
                btnTransferTable.Visibility = Visibility.Visible;
            }
            else if (currentUser.Role == "waiter")
            {
                btnEdit.IsEnabled = false;
                btnEdit.Visibility = Visibility.Collapsed;
                btnTransferTable.Visibility = Visibility.Collapsed;
            }

            LoadOrderItems();
            LoadMenu();
        }
        private void LoadOrderItems()
        {
            string query = @"SELECT oi.id, oi.quantity, oi.price_at_time, mi.name 
                     FROM order_items oi
                     JOIN menu_items mi ON oi.menu_item_id = mi.id
                     WHERE oi.order_id = @order_id";

            DataTable dt = db.ExecuteQuery(query, new MySqlParameter("@order_id", orderId));

            tempItems.Clear();
            foreach (DataRow row in dt.Rows)
            {
                tempItems.Add(new TempOrderItem
                {
                    Id = Convert.ToInt32(row["id"]),
                    ItemName = row["name"].ToString(),
                    Quantity = Convert.ToInt32(row["quantity"]),
                    Price = Convert.ToDecimal(row["price_at_time"])
                });
            }

            RefreshOrderList();

            if (tempItems.Count > 0 && canEdit)
            {
                btnSplitBill.Visibility = Visibility.Visible;
            }
        }

        private void LoadMenu()
        {
            string query = "SELECT id, name, price FROM menu_items WHERE category = @cat ORDER BY name";

            LoadCategory(lstPastas, query, "Пасты");
            LoadCategory(lstSoups, query, "Супы");
            LoadCategory(lstHotDishes, query, "Горячие блюда");
            LoadCategory(lstDrinks, query, "Напитки");
        }

        private void LoadCategory(ListBox listBox, string query, string category)
        {
            var dt = db.ExecuteQuery(query, new MySqlParameter("@cat", category));
            listBox.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string name = row["name"].ToString();
                decimal originalPrice = Convert.ToDecimal(row["price"]);
                decimal priceWithMarkup = ApplyMarkup(originalPrice);

                var menuItem = new MenuItemModel
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = name,
                    DisplayName = $"{name} - {priceWithMarkup:F2} ₽",
                    Price = priceWithMarkup
                };
                listBox.Items.Add(menuItem);
            }

            listBox.DisplayMemberPath = "DisplayName";
        }

        private void MenuItem_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (editMode) return;
            if (!canEdit) return;

            var listBox = sender as ListBox;
            if (listBox == null || listBox.SelectedItem == null) return;

            var selectedItem = listBox.SelectedItem as MenuItemModel;
            if (selectedItem == null) return;

            var existing = tempItems.FirstOrDefault(x => x.ItemName == selectedItem.Name);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                tempItems.Add(new TempOrderItem
                {
                    ItemName = selectedItem.Name,
                    Quantity = 1,
                    Price = selectedItem.Price
                });
            }

            hasUnsavedChanges = true;
            RefreshOrderList();
            listBox.SelectedItem = null;
        }
        private void RefreshOrderList()
        {
            lvOrderItems.ItemsSource = null;
            lvOrderItems.ItemsSource = tempItems;

            decimal total = tempItems.Sum(x => x.Total);
            txtTotal.Text = $"{total:F2} руб";
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "admin")
            {
                MessageBox.Show("Только администратор может редактировать заказ!",
                                "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            backupItems.Clear();
            foreach (var item in tempItems)
            {
                backupItems.Add(new TempOrderItem
                {
                    Id = item.Id,
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            editMode = true;
            btnEdit.Visibility = Visibility.Collapsed;
            btnCancelEdit.Visibility = Visibility.Visible;
            btnSave.IsEnabled = true;

            lvOrderItems.ContextMenu = new ContextMenu();
            var deleteMenuItem = new System.Windows.Controls.MenuItem();
            deleteMenuItem.Header = "УДАЛИТЬ";
            deleteMenuItem.Click += (s, ev) => {
                if (lvOrderItems.SelectedItem != null)
                {
                    tempItems.Remove((TempOrderItem)lvOrderItems.SelectedItem);
                    hasUnsavedChanges = true;
                    RefreshOrderList();
                }
            };
            lvOrderItems.ContextMenu.Items.Add(deleteMenuItem);
        }
        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            tempItems.Clear();
            foreach (var item in backupItems)
            {
                tempItems.Add(new TempOrderItem
                {
                    Id = item.Id,
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            RefreshOrderList();

            editMode = false;
            btnEdit.Visibility = Visibility.Visible;
            btnCancelEdit.Visibility = Visibility.Collapsed;
            btnSave.IsEnabled = true;
            lvOrderItems.ContextMenu = null;
            hasUnsavedChanges = false;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in tempItems)
                {
                    string getRecipeQuery = "SELECT recipe FROM menu_items WHERE name = @name";
                    string recipeJson = db.ExecuteScalar(getRecipeQuery, new MySqlParameter("@name", item.ItemName))?.ToString();

                    if (!string.IsNullOrEmpty(recipeJson) && recipeJson != "{}")
                    {
                        var recipe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, decimal>>(recipeJson);

                        foreach (var ingredient in recipe)
                        {
                            int productId = Convert.ToInt32(ingredient.Key);
                            decimal needed = ingredient.Value * item.Quantity;

                            string checkStockQuery = "SELECT quantity FROM products WHERE id = @id";
                            decimal available = Convert.ToDecimal(db.ExecuteScalar(checkStockQuery, new MySqlParameter("@id", productId)));

                            if (available < needed)
                            {
                                string productName = db.ExecuteScalar("SELECT name FROM products WHERE id = @id", new MySqlParameter("@id", productId))?.ToString();
                                MessageBox.Show($"Не хватает продукта: {productName}\nНужно: {needed}, в наличии: {available}",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                    }
                }

                db.ExecuteNonQuery("DELETE FROM order_items WHERE order_id = @oid",
                    new MySqlParameter("@oid", orderId));

                foreach (var item in tempItems)
                {
                    string getIdQuery = "SELECT id FROM menu_items WHERE name = @name";
                    int menuItemId = Convert.ToInt32(db.ExecuteScalar(getIdQuery,
                        new MySqlParameter("@name", item.ItemName)));

                    db.ExecuteNonQuery(@"INSERT INTO order_items (order_id, menu_item_id, quantity, price_at_time) 
                                 VALUES (@oid, @mid, @qty, @price)",
                        new MySqlParameter("@oid", orderId),
                        new MySqlParameter("@mid", menuItemId),
                        new MySqlParameter("@qty", item.Quantity),
                        new MySqlParameter("@price", item.Price));

                    string getRecipeQuery = "SELECT recipe FROM menu_items WHERE id = @id";
                    string recipeJson = db.ExecuteScalar(getRecipeQuery, new MySqlParameter("@id", menuItemId))?.ToString();

                    if (!string.IsNullOrEmpty(recipeJson) && recipeJson != "{}")
                    {
                        var recipe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, decimal>>(recipeJson);

                        foreach (var ingredient in recipe)
                        {
                            int productId = Convert.ToInt32(ingredient.Key);
                            decimal needed = ingredient.Value * item.Quantity;

                            db.ExecuteNonQuery("UPDATE products SET quantity = quantity - @needed WHERE id = @id",
                                new MySqlParameter("@needed", needed),
                                new MySqlParameter("@id", productId));
                        }
                    }
                }

                decimal total = tempItems.Sum(x => x.Total);
                db.ExecuteNonQuery("UPDATE orders SET total = @total WHERE id = @oid",
                    new MySqlParameter("@total", total),
                    new MySqlParameter("@oid", orderId));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            hasUnsavedChanges = false;
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (!canEdit)
            {
                MessageBox.Show("Этот стол обслуживает другой официант!\nВы не можете оплатить этот заказ.",
                                "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (tempItems.Count == 0)
            {
                MessageBox.Show("Заказ пуст! Добавьте блюда.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal total = tempItems.Sum(x => x.Total);

            var paymentWindow = new PaymentWindow(total);
            paymentWindow.Owner = this;
            paymentWindow.ShowDialog();

            if (!paymentWindow.IsPaid)
                return;

            string paymentType = paymentWindow.SelectedPaymentType;

            try
            {
                db.ExecuteNonQuery(@"INSERT INTO payments (order_id, amount, payment_type, waiter_id) 
                             VALUES (@oid, @amount, @type, @wid)",
                    new MySqlParameter("@oid", orderId),
                    new MySqlParameter("@amount", total),
                    new MySqlParameter("@type", paymentType),
                    new MySqlParameter("@wid", currentUser.Id));

                db.ExecuteNonQuery("UPDATE orders SET status = 'paid', closed_at = NOW() WHERE id = @oid",
                    new MySqlParameter("@oid", orderId));

                db.ExecuteNonQuery(@"UPDATE tables 
                            SET status = 'free', current_waiter_id = NULL, current_order_id = NULL 
                            WHERE id = @tid",
                    new MySqlParameter("@tid", currentTable.Id));

                SaveReceipt(total, paymentType);

                string paymentText = paymentType == "cash" ? "наличными" : (paymentType == "card" ? "картой" : "онлайн");
                MessageBox.Show($"Оплачено {total:F2} руб {paymentText}.\nЧек сохранён!",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при оплате: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string SaveReceipt(decimal total, string paymentType)
        {
            try
            {
                string chequesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Welkin_Cheques");
                if (!Directory.Exists(chequesDir))
                    Directory.CreateDirectory(chequesDir);

                string filename = Path.Combine(chequesDir, $"receipt_table{currentTable.Number}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    writer.WriteLine("╔════════════════════════════════════╗");
                    writer.WriteLine("║           WELKIN RESTAURANT         ║");
                    writer.WriteLine("╠════════════════════════════════════╣");
                    writer.WriteLine($"║ Стол: {currentTable.Number,-28}║");
                    writer.WriteLine($"║ Официант: {currentUser.FullName,-25}║");
                    writer.WriteLine($"║ Дата: {DateTime.Now:dd.MM.yyyy HH:mm,-25}║");
                    writer.WriteLine("╠════════════════════════════════════╣");
                    writer.WriteLine("║ Позиции:                           ║");

                    foreach (var item in tempItems)
                    {
                        writer.WriteLine($"║ {item.ItemName,-20} x{item.Quantity} = {item.Total,6:F2} ║");
                    }

                    writer.WriteLine("╠════════════════════════════════════╣");
                    writer.WriteLine($"║ Итого: {total,25:F2} ║");
                    writer.WriteLine($"║ Оплата: {(paymentType == "cash" ? "Наличные" : "Карта"),23} ║");
                    writer.WriteLine("╚════════════════════════════════════╝");
                }

                return filename;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении чека: " + ex.Message);
                return "ошибка сохранения";
            }
        }
        private void SplitBill_Click(object sender, RoutedEventArgs e)
        {
            if (tempItems.Count == 0)
            {
                MessageBox.Show("Нет блюд для разделения!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "На сколько гостей разделить счёт?\n(каждый получит отдельный чек)",
                "Разделение счёта",
                "2",
                -1, -1);

            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            if (!int.TryParse(input, out int guestCount))
            {
                MessageBox.Show("Введите корректное число (цифрами)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (guestCount < 2)
            {
                MessageBox.Show("Количество гостей должно быть не менее 2!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (guestCount > 10)
            {
                var result = MessageBox.Show($"Вы выбрали {guestCount} гостей.\nПродолжить?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;
            }

            var splitWindow = new SplitBillWindow(tempItems, guestCount, orderId, currentTable.Id, currentUser);
            splitWindow.ShowDialog();

            this.Close();
        }
        private void TableWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (hasUnsavedChanges && tempItems.Count > 0)
            {
                var result = MessageBox.Show(
                    "У вас есть несохранённые изменения!\n\n" +
                    "• Нажмите ДА - сохранить заказ и закрыть стол\n" +
                    "• Нажмите НЕТ - закрыть стол без сохранения\n" +
                    "• Нажмите ОТМЕНА - вернуться к редактированию",
                    "Несохранённые изменения",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveOrder();
                }
                else if (result == MessageBoxResult.No)
                {
                    DeleteEmptyOrder();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (tempItems.Count == 0 && canEdit)
            {
                var result = MessageBox.Show(
                    "Заказ пуст.\n\nУдалить этот стол?",
                    "Пустой заказ",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteEmptyOrder();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        private void SaveOrder()
        {
            try
            {
                db.ExecuteNonQuery("DELETE FROM order_items WHERE order_id = @oid",
                    new MySqlParameter("@oid", orderId));

                foreach (var item in tempItems)
                {
                    string getIdQuery = "SELECT id FROM menu_items WHERE name = @name";
                    int menuItemId = Convert.ToInt32(db.ExecuteScalar(getIdQuery,
                        new MySqlParameter("@name", item.ItemName)));

                    db.ExecuteNonQuery(@"INSERT INTO order_items (order_id, menu_item_id, quantity, price_at_time) 
                                 VALUES (@oid, @mid, @qty, @price)",
                        new MySqlParameter("@oid", orderId),
                        new MySqlParameter("@mid", menuItemId),
                        new MySqlParameter("@qty", item.Quantity),
                        new MySqlParameter("@price", item.Price));
                }

                decimal total = tempItems.Sum(x => x.Total);
                db.ExecuteNonQuery("UPDATE orders SET total = @total WHERE id = @oid",
                    new MySqlParameter("@total", total),
                    new MySqlParameter("@oid", orderId));

                hasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void DeleteEmptyOrder()
        {
            try
            {
                db.ExecuteNonQuery("DELETE FROM order_items WHERE order_id = @oid",
                    new MySqlParameter("@oid", orderId));
                db.ExecuteNonQuery("DELETE FROM orders WHERE id = @oid",
                    new MySqlParameter("@oid", orderId));

                db.ExecuteNonQuery(@"UPDATE tables 
                            SET status = 'free', current_waiter_id = NULL, current_order_id = NULL 
                            WHERE id = @tid",
                    new MySqlParameter("@tid", currentTable.Id));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении: " + ex.Message);
            }
        }
        private void TransferTable_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "admin")
            {
                MessageBox.Show("Только администратор может передавать стол другому официанту!",
                                "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataTable waiters = db.ExecuteQuery("SELECT id, full_name FROM users WHERE role = 'waiter'");

            if (waiters.Rows.Count == 0)
            {
                MessageBox.Show("Нет официантов для передачи стола!",
                                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selectWaiterWindow = new Window
            {
                Title = "Выбор официанта",
                Width = 350,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Выберите официанта для передачи стола:",
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 15),
                FontSize = 13
            });
            var comboBox = new ComboBox
            {
                Height = 35,
                Margin = new Thickness(0, 0, 0, 20),
                Background = new SolidColorBrush(Color.FromRgb(45, 45, 45)),
                Foreground = Brushes.Black,
                BorderBrush = new SolidColorBrush(Color.FromRgb(61, 61, 61))
            };

            foreach (DataRow row in waiters.Rows)
            {
                string name = row["full_name"].ToString();
                int id = Convert.ToInt32(row["id"]);

                var item = new ComboBoxItem();
                item.Content = name;
                item.Tag = id;
                item.Foreground = Brushes.Black;
                item.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));

                comboBox.Items.Add(item);
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var okButton = new Button
            {
                Content = "ПЕРЕДАТЬ",
                Width = 100,
                Height = 32,
                Margin = new Thickness(0, 0, 10, 0),
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
                Foreground = Brushes.White,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var cancelButton = new Button
            {
                Content = "ОТМЕНА",
                Width = 100,
                Height = 32,
                Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
                Foreground = Brushes.White,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            stackPanel.Children.Add(comboBox);
            stackPanel.Children.Add(buttonPanel);
            selectWaiterWindow.Content = stackPanel;

            int selectedWaiterId = 0;

            okButton.Click += (s, args) =>
            {
                if (comboBox.SelectedItem != null)
                {
                    var selected = comboBox.SelectedItem as ComboBoxItem;
                    if (selected != null)
                    {
                        selectedWaiterId = (int)selected.Tag;
                    }
                }
                selectWaiterWindow.DialogResult = true;
                selectWaiterWindow.Close();
            };

            cancelButton.Click += (s, args) =>
            {
                selectWaiterWindow.DialogResult = false;
                selectWaiterWindow.Close();
            };

            if (selectWaiterWindow.ShowDialog() == true && selectedWaiterId != 0)
            {
                DataTable newWaiter = db.ExecuteQuery("SELECT full_name FROM users WHERE id = @id",
                    new MySqlParameter("@id", selectedWaiterId));
                string newWaiterName = newWaiter.Rows[0]["full_name"].ToString();

                db.ExecuteNonQuery("UPDATE tables SET current_waiter_id = @waiter_id WHERE id = @table_id",
                    new MySqlParameter("@waiter_id", selectedWaiterId),
                    new MySqlParameter("@table_id", currentTable.Id));

                db.ExecuteNonQuery("UPDATE orders SET waiter_id = @waiter_id WHERE id = @order_id",
                    new MySqlParameter("@waiter_id", selectedWaiterId),
                    new MySqlParameter("@order_id", orderId));

                MessageBox.Show($"Стол {currentTable.Number} передан официанту {newWaiterName}!",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Title = $"Стол {currentTable.Number} - Обслуживает: {newWaiterName}";

                canEdit = false;
                btnSave.IsEnabled = false;
                btnEdit.IsEnabled = false;
                btnSplitBill.IsEnabled = false;
                btnTransferTable.IsEnabled = false;
                btnCheck.IsEnabled = false;

                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += (s, args) => { this.Close(); timer.Stop(); };
                timer.Start();
            }
        }
    }

    public class TempOrderItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public string DisplayText => $"{ItemName} x{Quantity} = {Total:F2} руб";
    }
}