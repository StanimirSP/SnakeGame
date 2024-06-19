using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;

namespace Gameplay
{
    /// <summary>
    /// Interaction logic for Gameplay.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        public event EventHandler GameOver;
        private double snakeLength;
        private uint foodCnt;
        private double toGrow;
        private int msMin, msMax;
        private Ellipse[] food;
        private uint obstacleCnt;
        private Line[] obstacles;
        private uint score;
        private Polyline snakeBody;
        private DispatcherTimer timer;
        private bool invincibility;
        private Random rnd;
        public readonly double obstacleLength = 150;
        private void InitSnakeBody()
        {
            snakeBody = new Polyline
            {
                Points = new PointCollection(),
                Stroke = Brushes.Green,
                StrokeThickness = 6
            };
            GameCanvas.Children.Add(snakeBody);
        }
        private void InitFoodShapes()
        {
            food = new Ellipse[foodCnt];
            for (int i = 0; i < foodCnt; i++)
            {
                food[i] = new Ellipse
                {
                    Visibility = Visibility.Hidden,
                    Fill = Brushes.Blue,
                    Width = 15,
                    Height = 15
                };
                food[i].MouseEnter += new MouseEventHandler(FoodEaten);
                GameCanvas.Children.Add(food[i]);
            }
        }
        public void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(RearrangeFood);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 3000);
            timer.Start();
        }
        private void InitObstacles()
        {
            obstacles = new Line[obstacleCnt];
            for (int i = 0; i < obstacleCnt; i++)
            {
                obstacles[i] = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 6
                };
                obstacles[i].MouseEnter += new MouseEventHandler(GameCanvas_MouseLeave);
                if (rnd.Next(0, 2) == 0) // make horizontal obstacle
                {
                    obstacles[i].X1 = rnd.Next(0, (int)(GameCanvas.ActualWidth - obstacleLength));
                    obstacles[i].X2 = obstacles[i].X1 + obstacleLength;
                    obstacles[i].Y1 = obstacles[i].Y2 = rnd.Next(0, (int)(GameCanvas.ActualHeight - obstacleLength));
                }
                else // make vertical obstacle
                {
                    obstacles[i].X1 = obstacles[i].X2 = rnd.Next(0, (int)(GameCanvas.ActualWidth - obstacleLength));
                    obstacles[i].Y1 = rnd.Next(0, (int)(GameCanvas.ActualHeight - obstacleLength));
                    obstacles[i].Y2 = obstacles[i].Y1 + obstacleLength;
                }
                obstacles[i].Visibility = Visibility.Visible;
                GameCanvas.Children.Add(obstacles[i]);
            }
        }
        public Game() : this(1, 500, 3000, 0) { }
        public Game(uint foodCnt, int msMin, int msMax, uint obstacleCnt)
        {
            InitializeComponent();
            snakeLength = 0;
            toGrow = 150;
            score = 0;
            invincibility = true;
            FoodCnt = foodCnt;
            if (msMin > msMax) throw new InvalidOperationException("msMin > msMax");
            MsMin = msMin;
            MsMax = msMax;
            rnd = new Random();
            ObstacleCnt = obstacleCnt;
            InitSnakeBody();
            InitFoodShapes();
            InitTimer();
            lblScore.Content = "Score: 0";
        }
        private void RearrangeFood(object sender, EventArgs args)
        {
            timer.Stop();
            invincibility = false;
            for (int i = 0; i < foodCnt; i++)
            {
                Canvas.SetLeft(food[i], rnd.Next(0, (int)(GameCanvas.ActualWidth - food[i].ActualWidth)));
                Canvas.SetTop(food[i], rnd.Next(0, (int)(GameCanvas.ActualHeight - food[i].ActualHeight)));
                food[i].Visibility = Visibility.Visible;
            }
            timer.Interval = new TimeSpan(0, 0, 0, 0, rnd.Next(msMin, msMax + 1));
            timer.Start();
        }
        private void FoodEaten(object sender, MouseEventArgs args)
        {
            if(sender is Ellipse e)
            {
                e.Visibility = Visibility.Hidden;
                lblScore.Content = $"Score: {++score}";
                toGrow += 10;
            }
        }
        public uint FoodCnt
        {
            get => foodCnt;
            private set => foodCnt = value > 0 && value <= 10 ? value : throw new InvalidOperationException("food count must be between 1 and 10");
        }
        public int MsMin
        {
            get => msMin;
            private set => msMin = value >= 100 & value <= 5000 ? value : throw new InvalidOperationException("msMin must be between 100 and 5000");
        }
        public int MsMax
        {
            get => msMax;
            private set => msMax = value >= 100 & value <= 5000 ? value : throw new InvalidOperationException("msMax must be between 100 and 5000");
        }
        public uint ObstacleCnt
        {
            get => obstacleCnt;
            private set => obstacleCnt = value >= 0 && value <= 10 ? value : throw new InvalidOperationException("number of obstacles must be between 0 and 10");
        }
        private void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(SnakeHead, GameCanvas.ActualWidth / 2 - SnakeHead.Width / 2);
            Canvas.SetTop(SnakeHead, GameCanvas.ActualHeight / 2 - SnakeHead.Height / 2);
            Window mainWindow = Application.Current.MainWindow;
            int centerX = (int)(Canvas.GetLeft(SnakeHead) + SnakeHead.Width / 2),
                centerY = (int)(Canvas.GetTop(SnakeHead) + SnakeHead.Height / 2);
            snakeBody.Points.Add(new Point(centerX - snakeLength, centerY));
            snakeBody.Points.Add(new Point(centerX, centerY));
            InitObstacles();
            SetCursorPos((int)(mainWindow.Left + GameCanvas.ActualWidth / 2), (int)(mainWindow.Top + GameCanvas.ActualHeight / 2));
        }
        private void RaiseGameOverEvent(string reason)
        {
            if(!invincibility) GameOver?.Invoke(this, new GameOverEventArgs(score, reason));
        }
        private void GameCanvas_MouseLeave(object sender, MouseEventArgs e)
            => RaiseGameOverEvent("Snake hit a wall");
        private void CutTail(double cutLength)
        {
            double dist;
            while ((dist = IntPoint.Distance(snakeBody.Points[0], snakeBody.Points[1])) <= cutLength)
            {
                snakeBody.Points.RemoveAt(0);
                cutLength -= dist;
            }
            if (cutLength <= 0) return;
            Point toMove = snakeBody.Points[0];
            snakeBody.Points.RemoveAt(0);
            toMove.X += cutLength / dist * (snakeBody.Points[0].X - toMove.X);
            toMove.Y += cutLength / dist * (snakeBody.Points[0].Y - toMove.Y);
            snakeBody.Points.Insert(0, toMove);
        }

        private bool SelfIntersect()
        {
            int cnt = snakeBody.Points.Count;
            Segment last = new Segment((IntPoint)snakeBody.Points[cnt - 1], (IntPoint)snakeBody.Points[cnt - 2]);
            for (int i = 0; i < cnt - 3; i++)
                if (Segment.Intersect(last, new Segment((IntPoint)snakeBody.Points[i], (IntPoint)snakeBody.Points[i + 1])))
                    return true;
            return false;
        }
        private bool HitObstacle()
        {
            if (obstacleCnt == 0) return false;
            int cnt = snakeBody.Points.Count;
            Segment last = new Segment((IntPoint)snakeBody.Points[cnt - 1], (IntPoint)snakeBody.Points[cnt - 2]);
            for (int i = 0; i < obstacleCnt; i++)
                if (Segment.Intersect(last, new Segment((IntPoint)new Point(obstacles[i].X1, obstacles[i].Y1), (IntPoint)new Point(obstacles[i].X2, obstacles[i].Y2))))
                    return true;
            return false;
        }
        private void GameCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point cursorPos = e.GetPosition(GameCanvas);
            if (!invincibility && (cursorPos.X < 0 || cursorPos.Y < 0 || cursorPos.X >= GameCanvas.ActualWidth || cursorPos.Y >= GameCanvas.ActualHeight))
                RaiseGameOverEvent("Snake hit a wall");
            double newSegmentLen = IntPoint.Distance(snakeBody.Points.Last(), cursorPos);
            if (newSegmentLen < 2) return;
            Canvas.SetLeft(SnakeHead, cursorPos.X - SnakeHead.Width / 2);
            Canvas.SetTop(SnakeHead, cursorPos.Y - SnakeHead.Height / 2);
            snakeBody.Points.Add(cursorPos);
            double cutLen = newSegmentLen;
            if(toGrow > 0)
            {
                snakeLength += Math.Min(toGrow, newSegmentLen);
                if(toGrow > newSegmentLen)
                {
                    cutLen = 0;
                    toGrow -= newSegmentLen;
                }
                else
                {
                    cutLen -= toGrow;
                    toGrow = 0;
                }
            }
            CutTail(cutLen);
            if (invincibility) return;
            if (SelfIntersect()) RaiseGameOverEvent("Snake crossed its own body");
            if (HitObstacle()) RaiseGameOverEvent("Snake hit a wall");
        }
    }
}
