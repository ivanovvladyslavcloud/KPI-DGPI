using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CipherBox.SelectedIndex = 0;
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            OutputText.Text = Process(true);
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            OutputText.Text = Process(false);
        }

        private string Process(bool encrypt)
        {
            string text = InputText.Text;
            string key = KeyBox.Text;
            string cipher = (CipherBox.SelectedItem as ComboBoxItem).Content.ToString();

            switch (cipher)
            {
                case "Caesar (shift)":
                    int shift = 3;
                    if (!int.TryParse(key, out shift))
                        shift = 3;
                    return Caesar(text, encrypt ? shift : -shift);

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
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    result.Append((char)(offset + (25 - (c - offset))));
                }
                else result.Append(c);
            }
            return result.ToString();
        }

        private string Vigenere(string text, string key, bool encrypt)
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
                char baseChar = isUpper ? 'A' : 'a';

                int shift = char.ToLower(key[j % key.Length]) - 'a';
                if (!encrypt) shift = -shift;

                int baseVal = c - baseChar;
                int newChar = (baseVal + shift + 26) % 26;

                result.Append((char)(newChar + baseChar));
                j++;
            }

            return result.ToString();
        }

        private string Xor(string text, string key)
        {
            var result = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                result.Append((char)(text[i] ^ key[i % key.Length]));
            }

            return result.ToString();
        }

        private string Reverse(string text)
        {
            char[] arr = text.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

    }
}