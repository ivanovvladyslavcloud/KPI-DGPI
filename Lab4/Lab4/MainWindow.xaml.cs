using System;
using System.Data;
using System.Windows;

namespace Lab4
{
    public partial class MainWindow : Window
    {
        AdoAssistant db;
        DataTable table;

        public MainWindow()
        {
            InitializeComponent();

            string cs =
                System.Configuration.ConfigurationManager
                .ConnectionStrings["connectionString_ADO"]
                .ConnectionString;

            db = new AdoAssistant(cs);

            LoadData();
        }

        private void LoadData()
        {
            table = db.GetAll();
            ListClients.ItemsSource = table.DefaultView;
        }

        private bool IsValidInput()
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(AddressBox.Text) ||
                string.IsNullOrWhiteSpace(AmountBox.Text))
            {
                MessageBox.Show("Fill all fields!");
                return false;
            }

            if (!decimal.TryParse(AmountBox.Text, out _))
            {
                MessageBox.Show("Amount must be a number!");
                return false;
            }

            return true;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidInput()) return;

            if (db.PhoneExists(PhoneBox.Text))
            {
                MessageBox.Show("Phone already exists!");
                return;
            }

            db.Insert(NameBox.Text, PhoneBox.Text, AddressBox.Text,
                      decimal.Parse(AmountBox.Text));

            LoadData();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (ListClients.SelectedItem == null)
            {
                MessageBox.Show("Select client!");
                return;
            }

            if (!IsValidInput()) return;

            DataRowView row = (DataRowView)ListClients.SelectedItem;
            int id = (int)row["ID"];

            if (db.PhoneExists(PhoneBox.Text))
            {
                int existingId = db.GetIdByPhone(PhoneBox.Text);

                if (existingId != id)
                {
                    MessageBox.Show("Phone belongs to another client!");
                    return;
                }
            }

            db.Update(id,
                NameBox.Text,
                PhoneBox.Text,
                AddressBox.Text,
                decimal.Parse(AmountBox.Text));

            LoadData();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ListClients.SelectedItem == null)
            {
                MessageBox.Show("Select client!");
                return;
            }

            DataRowView row = (DataRowView)ListClients.SelectedItem;

            db.Delete((int)row["ID"]);

            LoadData();
        }

        private void ListClients_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ListClients.SelectedItem == null) return;

            DataRowView row = (DataRowView)ListClients.SelectedItem;

            NameBox.Text = row["Name"].ToString();
            PhoneBox.Text = row["Phone"].ToString();
            AddressBox.Text = row["Address"].ToString();
            AmountBox.Text = row["OrderAmount"].ToString();
        }
    }
}