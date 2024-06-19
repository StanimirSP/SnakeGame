using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game currentGame;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Gameplay_GameOver(object sender, EventArgs e)
        {
            if (e is GameOverEventArgs g && currentGame != null)
            {
                mainCanvas.Children.Remove(currentGame);
                currentGame = null;
                lblScore.Content = $"Last game score: {g.Score}";
                MessageBox.Show($"{g.Reason}!\nScore: {g.Score}", "Game over!");
                btnStart.IsEnabled = true;
            }
        }

        private void GameStart(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            currentGame = new Game(uint.Parse(txtFoodCnt.Text), int.Parse(txtMinTime.Text), int.Parse(txtMaxTime.Text), uint.Parse(txtObstacles.Text));
            currentGame.Width = mainCanvas.ActualWidth;
            currentGame.Height = mainCanvas.ActualHeight;
            currentGame.GameOver += new EventHandler(Gameplay_GameOver);
            mainCanvas.Children.Add(currentGame);
        }
        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is ScrollBar s)
                switch (s.Name)
                {
                    case "scrbFood":
                        txtFoodCnt.Text = (s.Maximum - s.Value + s.Minimum).ToString();
                        break;
                    case "scrbObstacles":
                        txtObstacles.Text = (s.Maximum - s.Value + s.Minimum).ToString();
                        break;
                    case "scrbMinTime":
                        {
                            int ms = (int)(s.Maximum - s.Value + s.Minimum);
                            if (ms <= int.Parse(txtMaxTime.Text))
                                txtMinTime.Text = $"{ms}";
                            else
                                scrbMinTime.Value = s.Maximum - int.Parse(txtMinTime.Text) + s.Minimum;
                            break;
                        }
                    case "scrbMaxTime":
                        {
                            int ms = (int)(s.Maximum - s.Value + s.Minimum);
                            if (ms >= int.Parse(txtMinTime.Text))
                                txtMaxTime.Text = $"{ms}";
                            else
                                scrbMaxTime.Value = s.Maximum - int.Parse(txtMaxTime.Text) + s.Minimum;
                            break;
                        }
                }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Adjust the settings and start a game. To play the game just move your mouse and the head of the snake will follow your cursor. " + 
                            "After the game starts you have 3 seconds invincibility to get ready. Try to feed the snake by navigating it to the blue dots. " + 
                            "While doing so, avoid hitting the walls or crossing the snake body.",
                            "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
