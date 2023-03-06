using Omu.ValueInjecter;
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
using System.Windows.Threading;

namespace GameWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer timer = new DispatcherTimer();

        bool moveLeft, moveRight;

        List<Rectangle> itemRemover = new List<Rectangle>();

        Random random = new Random();

        int enemySpriteCounter = 0;
        int enemyCounter = 100;
        int playerSpeed = 22;
        int limit = 50;
        int score = 0;
        int damage = 0;
        int enemySpeed = 10;

        Rect playerHitBox;

        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += GameLoop;
            timer.Start();

            myCanvas.Focus();

            ImageBrush bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/purple.png"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            bg.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            myCanvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/player.png"));
            player.Fill = playerImage;

        }

        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            enemyCounter -= 1;

            scoreText.Content = "Score: " + score;
            damageText.Content = "Damage: " + damage;

            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (moveLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            }

            if (moveRight == true && Canvas.GetLeft(player) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }


            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet" || (string)x.Tag == "RotatedBulletLeft" || (string)x.Tag == "RotatedBulletRight")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if ((string)x.Tag == "RotatedBulletLeft")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - 10);

                    }

                    if ((string)x.Tag == "RotatedBulletRight")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + 10);

                    }


                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemRemover.Add(x);
                    }

                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(enemyHit))
                            {
                                itemRemover.Add(y);
                                itemRemover.Add(x);
                                score++;
                            }
                        }
                    }

                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);

                    if (Canvas.GetTop(x) > 1050)
                    {
                        itemRemover.Add(x);
                        damage += 10;
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        itemRemover.Add(x);
                        damage += 5;
                    }

                }

            }

            foreach (Rectangle i in itemRemover)
            {
                myCanvas.Children.Remove(i);
            }

            if (score > 5)
            {
                limit = 20;
            }
            if (score >20)
            {
                limit = 5;
            }
            if (score > 50)
            {
                limit = 1;
            }


            if (damage > 99)
            {
                timer.Stop();
                damageText.Content = "Damage: 100";
                damageText.Foreground = Brushes.Red;
                MessageBox.Show("Następnym razem postaraj się bardziej." + Environment.NewLine + "Twój wynik to: " + score, "Sebek pozdrawia");

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }


        }

        private void rotateBullet(Rectangle rect, int i)
        {
            double left = Canvas.GetLeft(rect);
            double top = Canvas.GetTop(rect);

            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);

            RotateTransform rotation = new RotateTransform();
            rotation.Angle = i;
            rotation.CenterX = rect.Width / 2;
            rotation.CenterY = rect.Height / 2;
            rect.RenderTransform = rotation;

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);
            i++;
        }


        private void craftWeapon(int a, int type)
        {
            Rectangle newBullet = new Rectangle
            {
                Tag = "bullet",
                Height = 20,
                Width = a,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };


            if (type == 1)
            {


                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);

                myCanvas.Children.Add(newBullet);
            }

            if (type == 2)
            {
                newBullet.Stroke = Brushes.Blue;
                Rectangle newBullet2 = new Rectangle();
                newBullet2.InjectFrom(newBullet);


                Canvas.SetLeft(newBullet2, Canvas.GetLeft(player) + player.Width);
                Canvas.SetTop(newBullet2, Canvas.GetTop(player) - newBullet2.Height);

                Canvas.SetLeft(newBullet, Canvas.GetLeft(player));
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);

                myCanvas.Children.Add(newBullet2);
                myCanvas.Children.Add(newBullet);

            }

            if (type == 3)
            {
                newBullet.Stroke = Brushes.Green;
                Rectangle newBullet2 = new Rectangle();
                Rectangle newBullet3 = new Rectangle();
                Rectangle newBullet4 = new Rectangle();
                newBullet2.InjectFrom(newBullet);
                newBullet3.InjectFrom(newBullet);
                newBullet4.InjectFrom(newBullet);

                rotateBullet(newBullet4, 30);
                rotateBullet(newBullet3, -30);

                newBullet3.Tag = "RotatedBulletLeft";
                newBullet4.Tag = "RotatedBulletRight";


                Canvas.SetLeft(newBullet4, Canvas.GetLeft(player) + player.Width);
                Canvas.SetTop(newBullet4, Canvas.GetTop(player) - newBullet4.Height);


                Canvas.SetLeft(newBullet3, Canvas.GetLeft(player));
                Canvas.SetTop(newBullet3, Canvas.GetTop(player) - newBullet3.Height);


                Canvas.SetLeft(newBullet2, Canvas.GetLeft(player) + player.Width);
                Canvas.SetTop(newBullet2, Canvas.GetTop(player) - newBullet2.Height);

                Canvas.SetLeft(newBullet, Canvas.GetLeft(player));
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);

                myCanvas.Children.Add(newBullet4);
                myCanvas.Children.Add(newBullet3);
                myCanvas.Children.Add(newBullet2);
                myCanvas.Children.Add(newBullet);

            }

        }

        private void myCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                moveLeft = false;
            }
            if (e.Key == Key.D)
            {
                moveRight = false;
            }
            if (e.Key == Key.Space)
            {

                if (score < 10)
                {

                    craftWeapon(5, 1);


                }
                if (score >= 10 && score < 20)
                {
                    craftWeapon(20, 1);
                }

                if (score >= 20 && score < 30)
                {

                    craftWeapon(10, 2);

                }
                if (score >= 30)
                {
                    craftWeapon(10, 3);

                }


            }

        }

        private void myCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                moveLeft = true;
            }
            if (e.Key == Key.D)
            {
                moveRight = true;
            }

        }

        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();

            enemySpriteCounter = random.Next(1, 5);

            switch (enemySpriteCounter)
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/1.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/2.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/3.png"));
                    break;
                case 4:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/4.png"));
                    break;
                case 5:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/5.png"));
                    break;
            }

            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 50,
                Width = 56,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, random.Next(30, 1850));
            myCanvas.Children.Add(newEnemy);
        }
    }
}
