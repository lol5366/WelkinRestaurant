using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace WelkinRestaurant.Views
{
    public partial class ChoiceWaiter : Window
    {
        public int SelectedWaiterId { get; private set; }

        public ChoiceWaiter(DataTable waiters, int tableNumber)
        {
            InitializeComponent();

            Title = $"Передача стола {tableNumber}";

            foreach (DataRow row in waiters.Rows)
            {
                int id = (int)row["id"];
                string name = row["full_name"].ToString();
                cmbWaiters.Items.Add(new ComboBoxItem { Tag = id, Content = name });
            }

            if (cmbWaiters.Items.Count > 0)
                cmbWaiters.SelectedIndex = 0;
        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            if (cmbWaiters.SelectedItem is ComboBoxItem waiterItem)
                SelectedWaiterId = (int)waiterItem.Tag;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}