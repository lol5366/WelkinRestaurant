using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using WelkinRestaurant.DataAccess;
using MySql.Data.MySqlClient;

namespace WelkinRestaurant.Views
{
    public partial class StockWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();
        private ObservableCollection<ProductStock> products = new ObservableCollection<ProductStock>();

        public StockWindow()
        {
            InitializeComponent();

            LoadProducts();
        }

        private void LoadProducts()
        {
            string query = "SELECT id, name, unit, quantity, min_quantity FROM products ORDER BY name";
            DataTable dt = db.ExecuteQuery(query);

            products.Clear();
            foreach (DataRow row in dt.Rows)
            {
                products.Add(new ProductStock
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString(),
                    Unit = row["unit"].ToString(),
                    Quantity = Convert.ToDecimal(row["quantity"]),
                    MinQuantity = row["min_quantity"] == DBNull.Value ? 0 : Convert.ToDecimal(row["min_quantity"])
                });
            }

            dgProducts.ItemsSource = products;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var product in products)
                {
                    db.ExecuteNonQuery(@"UPDATE products SET quantity = @qty, min_quantity = @min WHERE id = @id",
                        new MySqlParameter("@qty", product.Quantity),
                        new MySqlParameter("@min", product.MinQuantity),
                        new MySqlParameter("@id", product.Id));
                }
                MessageBox.Show("Остатки сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class ProductStock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinQuantity { get; set; }
    }
}