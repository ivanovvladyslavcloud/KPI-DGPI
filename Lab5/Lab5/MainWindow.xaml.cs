using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Linq;
using System.Windows;

namespace Lab5
{
    public partial class MainWindow : Window
    {
        Lab5Entities db = new Lab5Entities();

        public MainWindow()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            ClientsGrid.ItemsSource = db.Clients
                .Select(c => new
                {
                    c.ID,
                    c.Name,
                    c.Phone,
                    Company = c.Companies.CompanyName,
                    c.Income,
                    c.Expenses
                })
                .ToList();

            CompaniesGrid.ItemsSource = db.Companies.ToList();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            decimal minIncome;

            if (!decimal.TryParse(IncomeBox.Text, out minIncome))
            {
                MessageBox.Show("Enter a correct number!");
                return;
            }

            var result = db.Clients
                .Where(c => c.Income >= minIncome)
                .Select(c => new
                {
                    c.ID,
                    c.Name,
                    c.Phone,
                    Company = c.Companies.CompanyName,
                    c.Income,
                    c.Expenses
                })
                .ToList();

            RichClientsGrid.ItemsSource = result;
        }

        private void FilterByCompany_Click(object sender, RoutedEventArgs e)
        {
            int companyId;

            if (!int.TryParse(CompanyIdBox.Text, out companyId))
            {
                MessageBox.Show("Enter valid Company ID!");
                return;
            }

            var result = db.Clients
                .Where(c => c.CompanyID == companyId)
                .Select(c => new
                {
                    c.ID,
                    c.Name,
                    c.Phone,
                    Company = c.Companies.CompanyName,
                    c.Income,
                    c.Expenses
                })
                .ToList();

            CompanyClientsGrid.ItemsSource = result;
        }

        private void LoadTop_Click(object sender, RoutedEventArgs e)
        {
            var result = db.Clients
                .OrderByDescending(c => c.Income)
                .Take(5)
                .Select(c => new
                {
                    c.ID,
                    c.Name,
                    c.Phone,
                    Company = c.Companies.CompanyName,
                    c.Income
                })
                .ToList();

            TopClientsGrid.ItemsSource = result;
        }
    }
}
