using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lingo
{
    public partial class MainWindow : Window
    {
        private string wordToGuess;
        private int currentRow = 1;
        private int maxAttempts = 5;

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            string[] wordList = { "APPLE", "brave", "chair", "dream", "eagle" };
            var random = new System.Random();
            wordToGuess = wordList[random.Next(wordList.Length)];

            // İlk harfi göster
            TextBox guessBox = (TextBox)this.FindName("GuessTextBox");
            if (guessBox != null)
            {
                guessBox.Text = wordToGuess[0].ToString();
            }
        }

        // Tahmin Yapma
        private void GuessButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentRow > maxAttempts)
            {
                MessageBox.Show("Tahmin hakkınız bitti!");
                return;
            }

            TextBox guessBox = (TextBox)this.FindName("GuessTextBox");
            if (guessBox != null)
            {
                string guess = guessBox.Text.ToUpper();

                if (guess.Length == 5)
                {
                    CheckGuess(guess);
                    currentRow++;
                }
                else
                {
                    MessageBox.Show("Lütfen 5 harfli bir kelime girin.");
                }
            }
        }

        private void CheckGuess(string guess)
        {
            // Her harfi kontrol et ve doğru konumda ise rengi değiştir ve harfi göster
            for (int i = 0; i < 5; i++)
            {
                char guessedLetter = guess[i];
                char actualLetter = wordToGuess[i];
                Rectangle rect = (Rectangle)this.FindName($"Row{currentRow}_Letter{i + 1}");

                if (rect != null)
                {
                    // Harf kutusu içinde gösterilecek
                    TextBlock letterBlock = new TextBlock
                    {
                        Text = guessedLetter.ToString(),
                        FontSize = 18,
                        Foreground = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Canvas.SetLeft(letterBlock, Canvas.GetLeft(rect) + 5); // Harfi ortalamak için ayarlama
                    Canvas.SetTop(letterBlock, Canvas.GetTop(rect) + 2);   // Harfi ortalamak için ayarlama
                    MyCanvas.Children.Add(letterBlock); // Harfi ekle

                    // Renk kontrolü
                    if (guessedLetter == actualLetter)
                    {
                        // Doğru harf, doğru yerde
                        rect.Fill = Brushes.Green;
                    }
                    else if (wordToGuess.Contains(guessedLetter))
                    {
                        // Doğru harf, yanlış yerde
                        rect.Fill = Brushes.Yellow;
                    }
                    else
                    {
                        // Yanlış harf
                        rect.Fill = Brushes.Gray;
                    }
                }
            }

            // Eğer tahmin doğruysa oyunu kazandır
            if (guess == wordToGuess)
            {
                MessageBox.Show("Tebrikler, doğru kelimeyi buldunuz!");
            }
        }
    }
}
