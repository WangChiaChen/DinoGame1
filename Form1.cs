using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DinoGame1
{
    public partial class GameForm : Form
    {
        private System.Windows.Forms.Timer gameTimer;
        private bool isJumping;
        private int jumpSpeed;
        private int gravity;
        private int obstacleSpeed;
        private int score;
        private int dinoX, dinoY, dinoWidth, dinoHeight;
        private int obstacleX, obstacleY, obstacleWidth, obstacleHeight;
        private int[] starX, starY; // 添加小星星的位置数组
        private int[] cactusX, cactusY;
        
        private int backgroundSpeed;
        private bool isPaused;
        private int spiderWidth = 100;
        private int spiderHeight = 100;

        private List<int> spiderXPositions;
        private int initialSpiderX = 800; // 初始位置设置为窗口外面
        private int spiderSpacing = 200; // 蜘蛛之间的间距



        public GameForm()
        {
            InitializeComponent();
            InitializeGame();
            isPaused = false; // 初始化时游戏不暂停
        }

        private void InitializeGame()
        {
            // 设置窗体
            this.Width = 800;
            this.Height = 450;
            this.BackColor = Color.SkyBlue;
            this.Text = "Dino Game";
            this.DoubleBuffered = true;

            // 初始化游戏变量
            isJumping = false;
            jumpSpeed = 0;
            gravity = 15;
            obstacleSpeed = 10;
            score = 0;
            backgroundSpeed = 5;

            // 设置计时器
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20; // 50帧每秒
            gameTimer.Tick += GameTimer_Tick;

            // 初始化恐龙位置和大小
            dinoWidth = 50;
            dinoHeight = 50;
            dinoX = 50;
            dinoY = this.ClientSize.Height - dinoHeight - 50;

            // 初始化障碍物位置和大小
            obstacleWidth = 50;
            obstacleHeight = 50;
            obstacleX = this.ClientSize.Width;
            obstacleY = this.ClientSize.Height - obstacleHeight - 50;

            // 初始化小星星位置
            starX = new int[] { 200, 350, 500, 650 }; // 根据需要调整位置
            starY = new int[] { 100, 80, 120, 90 }; // 根据需要调整位置

            // 初始化仙人掌的位置
            cactusX = new int[] { 100, 300, 500, 700 };
            cactusY = new int[] { this.ClientSize.Height - 100, this.ClientSize.Height - 120, this.ClientSize.Height - 110, this.ClientSize.Height - 130 };

            

            // 初始化蝎子的位置
            spiderXPositions = new List<int>();

            int initialScorpionCount = 3; // 初始蝎子的数量
            spiderSpacing = 400; // 蝎子之间的间隔

            for (int i = 0; i < initialScorpionCount; i++)
            {
                spiderXPositions.Add(initialSpiderX + (i * spiderSpacing));
            }




            // 启动计时器
            gameTimer.Start();

            // 设置按键事件
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // 更新恐龙位置
            if (isJumping)
            {
                dinoY -= jumpSpeed;
                jumpSpeed -= 1;
                if (dinoY + dinoHeight >= this.ClientSize.Height - 50)
                {
                    dinoY = this.ClientSize.Height - dinoHeight - 50;
                    isJumping = false;
                }
            }
            else
            {
                dinoY += gravity;
                if (dinoY + dinoHeight > this.ClientSize.Height - 50)
                {
                    dinoY = this.ClientSize.Height - dinoHeight - 50;
                }
            }

            // 更新蜘蛛的位置，并检查跳过
            for (int i = 0; i < spiderXPositions.Count; i++)
            {
                spiderXPositions[i] -= obstacleSpeed;

                // 如果蜘蛛移出了窗口，将其移动到最后一只蜘蛛后面，以保持间隔
                if (spiderXPositions[i] + spiderWidth < 0)
                {
                    int maxSpiderX = spiderXPositions.Max();
                    spiderXPositions[i] = maxSpiderX + spiderSpacing;
                }

                // Adjust the collision rectangle for the dinosaur and spiders
                Rectangle spiderRect = new Rectangle(spiderXPositions[i] + 20, this.ClientSize.Height - spiderHeight - 30, spiderWidth - 40, spiderHeight - 20);
                Rectangle dinoRectangle = new Rectangle(dinoX, dinoY, dinoWidth, dinoHeight);

                if (spiderRect.IntersectsWith(dinoRectangle))
                {
                    gameTimer.Stop(); // 游戏结束
                    MessageBox.Show($"Game Over! Your score is: {score}");
                    Application.Restart(); // 重启游戏
                }

                // 检查恐龙是否跳过蜘蛛
                if (spiderXPositions[i] + spiderWidth < dinoX)
                {
                    // 增加分数并移除该蜘蛛
                    score += 1;
                    // 将该蜘蛛移到最右边
                    int maxSpiderX = spiderXPositions.Max();
                    spiderXPositions[i] = maxSpiderX + spiderSpacing;

                    // 检查是否达到10分
                    if (score >= 10)
                    {
                        gameTimer.Stop(); // 停止游戏
                        MessageBox.Show("恭喜通關！");
                        Application.Restart(); // 重启游戏或根据需要进行其他处理
                    }
                }
            }

            // 更新云的位置
            for (int i = 0; i < starX.Length; i++)
            {
                starX[i] -= backgroundSpeed;
                if (starX[i] < -60) // 60 是云的宽度
                {
                    starX[i] = this.ClientSize.Width;
                }
            }

            // 更新树的位置
            for (int i = 0; i < cactusX.Length; i++)
            {
                cactusX[i] -= backgroundSpeed;
                if (cactusX[i] < -60) // 60 是树的宽度
                {
                    cactusX[i] = this.ClientSize.Width;
                }
            }

            

            // 检查碰撞
            Rectangle dinoRect = new Rectangle(dinoX, dinoY, dinoWidth, dinoHeight);
            Rectangle obstacleRect = new Rectangle(obstacleX, obstacleY, obstacleWidth, obstacleHeight);
            if (dinoRect.IntersectsWith(obstacleRect))
            {
                gameTimer.Stop();
                MessageBox.Show($"Game Over! Your score is: {score}");
                Application.Restart();
            }

            // 重绘
            this.Invalidate();

            
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W && !isJumping) // 当按下 'W' 键且恐龙没有在跳跃时
            {
                isJumping = true;
                jumpSpeed = 15;
            }
            else if (e.KeyCode == Keys.Space) // 当按下空格键时
            {
                if (isPaused)
                {
                    gameTimer.Start(); // 继续游戏
                    isPaused = false;
                }
                else
                {
                    gameTimer.Stop(); // 暂停游戏
                    isPaused = true;
                }
                Invalidate(); // 重新绘制以显示暂停文本
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                isJumping = false;
            }
        }



        private void DrawScore(Graphics g)
        {
            string scoreText = $"Score: {score}";
            Font scoreFont = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold); // 使用字体大小为 15
            g.DrawString(scoreText, scoreFont, Brushes.Black, new PointF(10, 10));
            scoreFont.Dispose(); // 释放字体资源
        }



        private void DrawCat(Graphics g, int x, int y)
        {
            // 绘制猫咪的头
            g.FillEllipse(Brushes.Gray, x, y, 50, 50);

            // 绘制猫咪的耳朵
            Point[] leftEarPoints = {
                new Point(x + 5, y + 10),
                new Point(x + 20, y - 10),
                new Point(x + 30, y + 10),
                new Point(x + 15, y + 20)
            };
            g.FillPolygon(Brushes.Gray, leftEarPoints);

            Point[] rightEarPoints = {
                new Point(x + 20, y + 10),
                new Point(x + 35, y - 10),
                new Point(x + 40, y + 10),
                new Point(x + 25, y + 20)
            };
            g.FillPolygon(Brushes.Gray, rightEarPoints);

            // 绘制猫咪的眼睛
            g.FillEllipse(Brushes.White, x + 15, y + 15, 10, 10);
            g.FillEllipse(Brushes.White, x + 25, y + 15, 10, 10);
            g.FillEllipse(Brushes.Black, x + 18, y + 18, 5, 5);
            g.FillEllipse(Brushes.Black, x + 28, y + 18, 5, 5);

            // 绘制猫咪的鼻子
            g.FillEllipse(Brushes.Pink, x + 22, y + 30, 6, 6);

            // 绘制猫咪的嘴巴
            g.FillPie(Brushes.Black, x + 20, y + 32, 10, 10, 0, -180);

            // 绘制猫咪的胡须
            g.DrawLine(Pens.Black, x + 15, y + 30, x + 5, y + 25);
            g.DrawLine(Pens.Black, x + 15, y + 33, x + 5, y + 33);
            g.DrawLine(Pens.Black, x + 15, y + 36, x + 5, y + 41);
            g.DrawLine(Pens.Black, x + 35, y + 30, x + 45, y + 25);
            g.DrawLine(Pens.Black, x + 35, y + 33, x + 45, y + 33);
            g.DrawLine(Pens.Black, x + 35, y + 36, x + 45, y + 41);

            // 绘制猫咪的尾巴
            Point[] tailPoints = {
            new Point(x+5, y + 35),     // 尾巴起始点，位置在猫头左侧稍下方
            new Point(x - 30, y + 40),  // 尾巴向上弯曲一些
            new Point(x - 35, y + 35),  // 尾巴第三个点，向上弯曲一些
            new Point(x - 40, y + 30)   // 尾巴最终点，向上彎曲
            };
            g.DrawCurve(new Pen(Color.Gray, 3), tailPoints, tension: 0.5f);  // 使用曲线绘制尾巴，使其柔和且更自然
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // 绘制背景
            DrawBackground(g);

            // 绘制猫咪
            DrawCat(g, dinoX, dinoY);

            // 绘制障碍物
            g.FillRectangle(Brushes.Red, obstacleX, obstacleY, obstacleWidth, obstacleHeight);

            // 如果游戏暂停，绘制暂停文本
            if (isPaused)
            {
                DrawPausedText(g);
            }
            // 绘制蜘蛛
            for (int i = 0; i < spiderXPositions.Count; i++)
            {
                Color spiderColor = i switch
                {
                    0 => Color.Green,
                    1 => Color.Red,
                    2 => Color.Blue,
                    _ => Color.Black // 如果需要添加更多蜘蛛，可以使用其他颜色
                };
                DrawSpider(g, spiderXPositions[i], this.ClientSize.Height - spiderHeight - 10, spiderColor); // 将蜘蛛位置调整到地面上方
            }

            // 绘制分数
            DrawScore(g);
        }
        private void DrawPausedText(Graphics g)
        {
            string pausedText = "Game Paused";
            Font pausedFont = new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold);
            SizeF pausedTextSize = g.MeasureString(pausedText, pausedFont);
            PointF pausedTextLocation = new PointF((this.ClientSize.Width - pausedTextSize.Width) / 2, (this.ClientSize.Height - pausedTextSize.Height) / 2);
            g.DrawString(pausedText, pausedFont, Brushes.Black, pausedTextLocation);
            pausedFont.Dispose();
        }

        private void DrawSpider(Graphics g, int x, int y, Color color)
        {
            // 绘制蜘蛛的身体
            g.FillEllipse(new SolidBrush(color), x + 20, y + 20, 60, 40); // 身体部分
            g.FillEllipse(new SolidBrush(color), x + 35, y, 30, 30); // 头部

            // 绘制蜘蛛的腿
            Pen legPen = new Pen(color, 4);
            int legLength = 40;//短一些的腿长度
                                // 左侧的腿
            g.DrawLine(legPen, x + 40, y + 30, x + 40 - legLength, y + 30 - legLength);
            g.DrawLine(legPen, x + 40, y + 30, x + 40 - legLength, y + 30 + legLength);
            g.DrawLine(legPen, x + 40, y + 35, x + 40 - legLength, y + 35 - legLength / 2);
            g.DrawLine(legPen, x + 40, y + 35, x + 40 - legLength, y + 35 + legLength / 2);

            // 右侧的腿
            g.DrawLine(legPen, x + 60, y + 30, x + 60 + legLength, y + 30 - legLength);
            g.DrawLine(legPen, x + 60, y + 30, x + 60 + legLength, y + 30 + legLength);
            g.DrawLine(legPen, x + 60, y + 35, x + 60 + legLength, y + 35 - legLength / 2);
            g.DrawLine(legPen, x + 60, y + 35, x + 60 + legLength, y + 35 + legLength / 2);

            // 绘制蜘蛛的眼睛
            g.FillEllipse(Brushes.White, x + 40, y + 10, 10, 10); // 左眼
            g.FillEllipse(Brushes.White, x + 50, y + 10, 10, 10); // 右眼
            g.FillEllipse(Brushes.Black, x + 43, y + 13, 5, 5);   // 左眼瞳孔
            g.FillEllipse(Brushes.Black, x + 53, y + 13, 5, 5);   // 右眼瞳孔

            // 绘制蜘蛛的嘴巴
            g.FillPie(Brushes.Black,x + 42, y + 20, 15, 10, 0, -180); // 嘴巴

            // 绘制蜘蛛的触角
            g.DrawLine(legPen, x + 40, y + 20, x + 30, y + 10);
            g.DrawLine(legPen, x + 60, y + 20, x + 70, y + 10);
        }

        public enum SnakeType
        {
            Green,
            Red,
            Blue
        }

        private void DrawBackground(Graphics g)
        {
            // Change background color to light brown for the entire background
            g.Clear(Color.FromArgb(222, 184, 135)); // Light brown color

            // Change ground color to a slightly darker shade of brown
            g.FillRectangle(Brushes.SaddleBrown, 0, this.ClientSize.Height - 50, this.ClientSize.Width, 50);

            // Calculate the height of the deep blue section (1/4 of screen height)
            int deepBlueHeight = this.ClientSize.Height / 4;

            // 绘制深蓝色背景
            g.FillRectangle(Brushes.DarkBlue, 0, 0, this.ClientSize.Width, deepBlueHeight);

            // 绘制小星星
            foreach (var (x, y) in starX.Zip(starY, Tuple.Create))
            {
                // 调整小星星位置，向上偏移一些
                int adjustedY = y - 45; // 可以根据需要调整偏移量
                                        // 限制小星星在深蓝色背景范围内
                if (adjustedY >= 0 && adjustedY <= deepBlueHeight)
                {
                    DrawStar(g, x, adjustedY);
                }
            }

            // 绘制仙人掌
            foreach (var (x, y) in cactusX.Zip(cactusY, Tuple.Create))
            {
                DrawCactus(g, x, y);
            }
        }

        private void DrawStar(Graphics g, int x, int y)
        {
            // 绘制小星星
            Point[] starPoints = new Point[]
            {
        new Point(x, y + 15),
        new Point(x + 5, y + 5),
        new Point(x + 15, y),
        new Point(x + 5, y - 5),
        new Point(x, y - 15),
        new Point(x - 5, y - 5),
        new Point(x - 15, y),
        new Point(x - 5, y + 5),
        new Point(x, y + 15)
            };
            g.FillPolygon(Brushes.Yellow, starPoints);
        }

        private void DrawCactus(Graphics g, int x, int y)
        {
            // 绘制仙人掌
            g.FillRectangle(Brushes.Green, x + 15, y, 30, 75); // 主干
            g.FillRectangle(Brushes.Green, x, y + 15, 15, 30); // 左臂
            g.FillRectangle(Brushes.Green, x + 45, y + 15, 15, 30); // 右臂
           
            // 左臂
            Point[] leftArmPoints = new Point[]
            {
        new Point(x, y + 15),
        new Point(x, y - 75)
            };
            g.FillPolygon(Brushes.Green, leftArmPoints);

            // 右臂
            Point[] rightArmPoints = new Point[]
            {
        new Point(x + 60, y + 15),
        new Point(x + 60, y - 75)
            };
            g.FillPolygon(Brushes.Green, rightArmPoints);
        }
    }
}
