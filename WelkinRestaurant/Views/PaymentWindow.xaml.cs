using System;
using System.Windows;

namespace WelkinRestaurant.Views
{
    public partial class PaymentWindow : Window
    {
        public string SelectedPaymentType { get; private set; }
        public bool IsPaid { get; private set; } = false;

        public PaymentWindow(decimal totalAmount)
        {
            InitializeComponent();
            txtAmount.Text = $"{totalAmount:F2}";
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли элемент
            if (cmbPaymentType.SelectedItem == null)
            {
                SelectedPaymentType = "cash";
                IsPaid = true;
                this.Close();
                return;
            }

            // Получаем выбранный текст
            string selectedText = cmbPaymentType.SelectedItem.ToString();

            // Определяем тип оплаты
            if (selectedText.Contains("НАЛИЧНЫЕ"))
                SelectedPaymentType = "cash";
            else if (selectedText.Contains("КАРТА"))
                SelectedPaymentType = "card";
            else if (selectedText.Contains("ОНЛАЙН"))
                SelectedPaymentType = "online";
            else
                SelectedPaymentType = "cash";

            IsPaid = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsPaid = false;
            this.Close();
        }
    }
}