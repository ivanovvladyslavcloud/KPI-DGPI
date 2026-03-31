using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Lab6
{
    public partial class MainWindow : Window
    {
        private string cs =
            ConfigurationManager
            .ConnectionStrings["CryptoDB"]
            .ConnectionString;

        public MainWindow()
        {
            InitializeComponent();
            CipherBox.SelectedIndex = 0;
            FilterBox.SelectedIndex = 0;
        }
        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            string result = Process(true);

            OutputText.Text = result;

            SaveToDatabase(
                InputText.Text,
                CipherBox.Text,
                KeyBox.Text,
                result,
                "Encrypt");
        }
        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            string result = Process(false);

            OutputText.Text = result;

            SaveToDatabase(
                InputText.Text,
                CipherBox.Text,
                KeyBox.Text,
                result,
                "Decrypt");
        }
        private string Process(bool encrypt)
        {
            string text = InputText.Text;
            string key = KeyBox.Text;
            string cipher =
                (CipherBox.SelectedItem as ComboBoxItem)
                .Content
                .ToString();

            switch (cipher)
            {
                case "Caesar (shift)":

                    int shift = 3;

                    if (!int.TryParse(key, out shift))
                        shift = 3;

                    return Caesar(text,
                        encrypt ? shift : -shift);

                case "Atbash (mirrored)":
                    return Atbash(text);

                case "Vigenere (requires the key)":
                    return Vigenere(text, key, encrypt);

                case "XOR (binary)":
                    return Xor(text, key);

                case "Reverse":
                    return Reverse(text);

                default:
                    return text;
            }
        }
        private string Caesar(string text, int shift)
        {
            var result = new StringBuilder();

            foreach (char c in text)
            {
                result.Append((char)(c + shift));
            }

            return result.ToString();
        }
        private string Atbash(string text)
        {
            var result = new StringBuilder();

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char offset =
                        char.IsUpper(c) ? 'A' : 'a';

                    result.Append(
                        (char)(offset + (25 - (c - offset))));
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        private string Vigenere(string text,
                               string key,
                               bool encrypt)
        {
            if (string.IsNullOrEmpty(key))
                return text;

            var result = new StringBuilder();

            int j = 0;

            foreach (char c in text)
            {
                if (!char.IsLetter(c))
                {
                    result.Append(c);
                    continue;
                }

                bool isUpper = char.IsUpper(c);

                char baseChar =
                    isUpper ? 'A' : 'a';

                int shift =
                    char.ToLower(key[j % key.Length]) - 'a';

                if (!encrypt)
                    shift = -shift;

                int baseVal = c - baseChar;

                int newChar =
                    (baseVal + shift + 26) % 26;

                result.Append(
                    (char)(newChar + baseChar));

                j++;
            }

            return result.ToString();
        }
        private string Xor(string text, string key)
        {
            if (string.IsNullOrEmpty(key))
                return text;

            var result = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                result.Append(
                    (char)(text[i] ^ key[i % key.Length]));
            }

            return result.ToString();
        }
        private string Reverse(string text)
        {
            char[] arr = text.ToCharArray();

            Array.Reverse(arr);

            return new string(arr);
        }
        private void SaveToDatabase(string input,
                                   string method,
                                   string key,
                                   string result,
                                   string operation)
        {
            using (SqlConnection conn =
                   new SqlConnection(cs))
            {
                conn.Open();

                string sql =
                    @"INSERT INTO EncryptionHistory
                    (InputText,
                     CipherMethod,
                     KeyValue,
                     ResultText,
                     OperationType)

                    VALUES
                    (@i,@m,@k,@r,@o)";

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@i", input);
                cmd.Parameters.AddWithValue("@m", method);
                cmd.Parameters.AddWithValue("@k", key);
                cmd.Parameters.AddWithValue("@r", result);
                cmd.Parameters.AddWithValue("@o", operation);

                cmd.ExecuteNonQuery();
            }
        }
        private void LoadHistory_Click(object sender,
                               RoutedEventArgs e)
        {
            using (SqlConnection conn =
                   new SqlConnection(cs))
            {
                conn.Open();

                string filter =
                    (FilterBox.SelectedItem as ComboBoxItem)
                    .Content
                    .ToString();

                string sql;

                if (filter == "All")
                {
                    sql = @"SELECT TOP 20 *
                    FROM EncryptionHistory
                    ORDER BY ID DESC";
                }
                else
                {
                    sql = @"SELECT TOP 20 *
                    FROM EncryptionHistory
                    WHERE CipherMethod = @method
                    ORDER BY ID DESC";
                }

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                if (filter != "All")
                {
                    cmd.Parameters.AddWithValue("@method", filter);
                }

                SqlDataReader reader =
                    cmd.ExecuteReader();

                StringBuilder sb =
                    new StringBuilder();

                while (reader.Read())
                {
                    sb.AppendLine(
                        $"[{reader["ID"]}] " +
                        $"{reader["CipherMethod"]} | " +
                        $"{reader["OperationType"]}");

                    sb.AppendLine(
                        $"Input: {reader["InputText"]}");

                    sb.AppendLine(
                        $"Result: {reader["ResultText"]}");

                    sb.AppendLine(new string('-', 35));
                }

                OutputText.Text = sb.ToString();
            }
        }

        private void ClearHistory_Click(object sender,
                                RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Delete all history?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            using (SqlConnection conn =
                   new SqlConnection(cs))
            {
                conn.Open();

                string sql =
                    "DELETE FROM EncryptionHistory";

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                cmd.ExecuteNonQuery();
            }

            OutputText.Clear();

            MessageBox.Show("History cleared!");
        }
    }
}