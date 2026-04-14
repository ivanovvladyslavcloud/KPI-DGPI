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

                case "Vernam":
                    return Vernam(text, key);

                case "RSA (simple)":
                    return RSA(text, encrypt);

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
            var result = new StringBuilder();
            int j = 0;

            foreach (char c in text)
            {
                if (!char.IsLetter(c))
                {
                    result.Append(c);
                    continue;
                }

                int shift = key[j % key.Length] - 'a';
                if (!encrypt) shift = -shift;

                result.Append((char)(c + shift));
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

        private string Vernam(string text, string key)
        {
            if (string.IsNullOrEmpty(key))
                return text;

            var result = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                result.Append((char)(text[i] ^ key[i % key.Length]));
            }

            return result.ToString();
        }

        private string RSA(string text, bool encrypt)
        {
            int p = 61;
            int q = 53;
            int n = p * q;     // 3233
            int e = 17;
            int d = 2753;

            if (encrypt)
            {
                var result = new List<int>();

                foreach (char c in text)
                {
                    int m = c;
                    int enc = ModPow(m, e, n);
                    result.Add(enc);
                }

                return string.Join(" ", result);
            }
            else
            {
                var parts = text.Split(' ');
                var result = new StringBuilder();

                foreach (var part in parts)
                {
                    if (int.TryParse(part, out int num))
                    {
                        int dec = ModPow(num, d, n);
                        result.Append((char)dec);
                    }
                }

                return result.ToString();
            }
        }

        private int ModPow(int baseVal, int exp, int mod)
        {
            long result = 1;
            long b = baseVal;

            while (exp > 0)
            {
                if ((exp & 1) == 1)
                    result = (result * b) % mod;

                b = (b * b) % mod;
                exp >>= 1;
            }

            return (int)result;
        }


    }
}