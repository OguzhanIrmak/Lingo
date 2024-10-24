using System;
using System.Collections.Generic;
using System.IO;
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
        private static Random random;

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            //Kelime dosyasının yolu
            string filePath = "C:\\Users\\oguzh\\source\\repos\\Lingo\\Kelimeler.txt";

            // Dosyadaki toplam satır sayısını dinamik olarak bulma 
            // 5 harfli yeni kelimeler direk eklenebilir
            int totalLines = GetTotalLineCount(filePath);

            if (totalLines == 0)
            {
                MessageBox.Show("Kelime dosyasından hiç kelime yüklenemedi.");
                return;
            }

            // Daha karmaşık rastgelelik ile satır seçimi
            int selectedLine = GetRandomLine(totalLines);

            // Seçilen satırdaki kelimeyi al
            wordToGuess = GetWordFromLine(filePath, selectedLine).Trim();

            if (string.IsNullOrEmpty(wordToGuess))
            {
                MessageBox.Show("Bir hata oluştu, kelime seçilemedi.");
                return;
            }

            // İlk harfi tahmin kutusuna yerleştirme
            Label firstLetterLabel = (Label)this.FindName("FirstLetterLabel");
            if (firstLetterLabel != null)
            {
                firstLetterLabel.Content = wordToGuess[0].ToString();
            }
        }
        #region Rastgelelik ile ilgili metotlar
        // Toplam satır sayısını  hesaplayan fonksiyon
        private int GetTotalLineCount(string filePath)
        {
            try
            {

                return File.ReadAllLines(filePath, System.Text.Encoding.UTF8).Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya okunurken bir hata oluştu: {ex.Message}");
                return 0;
            }
        }

        // Rastgele satır numarası seçen fonksiyon
        private int GetRandomLine(int totalLines)
        {

            int seed = Guid.NewGuid().GetHashCode() ^ DateTime.Now.Millisecond;
            var random = new Random(seed);


            return random.Next(0, totalLines);
        }

        // Seçilen satırdaki kelimeyi okuyan fonksiyon
        private string GetWordFromLine(string filePath, int lineNumber)
        {
            try
            {
                using (var sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
                {
                    for (int i = 0; i < lineNumber; i++)
                    {
                        sr.ReadLine();
                    }
                    string selectedWord = sr.ReadLine();
                    return selectedWord?.ToUpper();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosyadan kelime okuma hatası: {ex.Message}");
                return null;
            }
        }
        #endregion

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
                string guess = guessBox.Text.ToUpper().Trim();
                MessageBox.Show($"Tahmin: {guess}, Doğru Kelime: {wordToGuess}");

                if (guess.Length == 5)
                {
                    // Doğru tahmin kontrolü
                    if (guess.Equals(wordToGuess, StringComparison.OrdinalIgnoreCase))
                    {
                        // Tüm kutuları yeşil renkte göster ve harfleri yerleştir
                        for (int i = 0; i < 5; i++)
                        {
                            Rectangle rect = (Rectangle)this.FindName($"Row{currentRow}_Letter{i + 1}");
                            if (rect != null)
                            {
                                rect.Fill = Brushes.Green;


                                TextBlock letterBlock = new TextBlock
                                {
                                    Text = guess[i].ToString(),
                                    FontSize = 18,
                                    Foreground = Brushes.Black,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                };

                                Canvas.SetLeft(letterBlock, Canvas.GetLeft(rect) + 5);
                                Canvas.SetTop(letterBlock, Canvas.GetTop(rect) - 3);
                                MyCanvas.Children.Add(letterBlock);
                            }
                        }

                        MessageBox.Show("Tebrikler, doğru kelimeyi buldunuz!");
                        return;
                    }

                    CheckGuess(guess); // Tahmin kontrolü metodu
                    currentRow++; // Yeni tahmin satırına geçiş
                    guessBox.Clear(); // Tahmin kutusunu temizle
                }
                else
                {
                    MessageBox.Show("Lütfen 5 harfli bir kelime girin.");
                }
            }
        }


        private void CheckGuess(string guess)
        {
            // HARF KONTROLÜ VE RENK AYARI
            for (int i = 0; i < 5; i++)
            {
                char guessedLetter = guess[i];
                char actualLetter = wordToGuess[i];
                Rectangle rect = (Rectangle)this.FindName($"Row{currentRow}_Letter{i + 1}");

                if (rect != null)
                {
                    // HARF KUTUSU İÇİNDE OLACAK
                    TextBlock letterBlock = new TextBlock
                    {
                        Text = guessedLetter.ToString(),
                        FontSize = 18,
                        Foreground = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Canvas.SetLeft(letterBlock, Canvas.GetLeft(rect) + 5);
                    Canvas.SetTop(letterBlock, Canvas.GetTop(rect) - 3);
                    MyCanvas.Children.Add(letterBlock);

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
        }
    }
}

