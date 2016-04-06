using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gunfight
{
    class Gunfight
    {
        class Point
        {
            public int row;
            public int col;
        }

        //константа
        const int WindowWidth = 100;
        const int WindowHeight = 20;
        const int ScreenUpperBorder = 2;// това го правя, за да може човечето да не започва от началото, а от средата, за да може в началото да имам контроли. И като се опитам да отида на горе не ми позволява. 
        const int EnemyStartOffset = WindowWidth;
        const int ScreenLowerBorder = WindowHeight - 2;
        static bool IsGameOver = false;
        const int CollisionAOE = 1;
        static Random randGenerator = new Random();

        /*Player info*/
        static int playerRow = 0;
        static int playerCol = 0;
        static string playerFigure = "[0]";
        static ConsoleColor playerColor = ConsoleColor.Red;

        /*Enemi info*/
        static List<Point> enemies = new List<Point>();
        static string enemyFigure = "%";
        static ConsoleColor enemyColor = ConsoleColor.DarkMagenta;
        static int enemySpawnChance = 10;

        /*Bullet info*/
        static List<Point> bullets = new List<Point>();
        static string bulletFigure = "-";
        static ConsoleColor bulletColor = ConsoleColor.White;


        static void Main(string[] args)
        {
            InitialaseSettings();
            while (!IsGameOver)
            {
                Clear(); // 1.трием
                ChechCollisions();
                Update();// 2.ъпдейтваме
                Draw();//3.чертаем

                Thread.Sleep(100); // за да не мига човечето
            }
        }

        #region Utility Methods

        static void PrintOnPosition(int row, int col, string text, ConsoleColor color)
        {
            Console.SetCursorPosition(col, row); // това нещо премества курсора на дадена позиция
            Console.ForegroundColor = color;
            Console.Write(text); //ще изпечата текста, който сме задали
        }


        //метод, с който да си настроим екрана.Това, което ще прави е ще сетва размера на екрана
        static void InitialaseSettings()
        {
            Console.WindowWidth= WindowWidth;
            Console.WindowHeight = WindowHeight;
            //За да махнем скролерите на прозореца
            Console.BufferWidth = WindowWidth;
            Console.BufferHeight = WindowHeight;
            //За да махнем курсора. Иначе мига :)
            Console.CursorVisible = false;
        }
        static bool IsObjectInBounds(int row, int col)
        {
            return row >= ScreenUpperBorder && row <= WindowHeight - 1 && col >= 0 && col <= WindowWidth - 1;

        }

        static bool DoObjectsCollide(int firstRow, int firstCol, int secondRow, int secondCol)
        {
            
            int colOffset = Math.Abs(firstCol-secondCol);
            return firstRow==secondRow && colOffset <= CollisionAOE;
        }

        #endregion

        #region Player Methods

        static void ClearPlayer()
        {
            PrintOnPosition(playerRow,playerCol,"   ", playerColor);
        }

        static void DrawPlayer() //да накарам човечето да се мърда
        {
            PrintOnPosition(playerRow,playerCol,playerFigure, playerColor);
        }

        static void UpdatePlayer() //да ъпдейтва координатите и разни други логики
        {
            ReadInput();
        }
        static void ReadInput()
        {
            if (Console.KeyAvailable)//това значи, че ако имам натиснат някакъв клавиш ще влезе в скобите между иф-а
            {
                ConsoleKeyInfo userInput = Console.ReadKey(); // чете текущия клавиш от клавиатурата

                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }

                //след като сме прочели клавиша трябва да го обработим по някакъв начин -- >следващия иф
                if (userInput.Key == ConsoleKey.LeftArrow && playerCol > 0)
                {
                    playerCol--;
                }
                else if (userInput.Key == ConsoleKey.RightArrow && playerCol < WindowWidth-3)
                {
                    playerCol++;
                }
                else if (userInput.Key == ConsoleKey.DownArrow && playerRow < ScreenLowerBorder)
                {
                    playerRow++;
                }
                else if (userInput.Key == ConsoleKey.UpArrow && playerRow > ScreenUpperBorder)
                {
                    playerRow--;
                }
                else if (userInput.Key == ConsoleKey.Spacebar)
                {
                    bullets.Add(new Point()
                    {
                        row = playerRow,
                        col= playerCol+2
                    });
                }
            }
        }
        #endregion

        #region Enemy Methods

        static void ClearEnemies()
        {
            for (int cnt = 0; cnt < enemies.Count; cnt++)
            {
                if (IsObjectInBounds(enemies[cnt].row, enemies[cnt].col))
                {
                    PrintOnPosition(enemies[cnt].row, enemies[cnt].col, " ", enemyColor);
                }
                    
            }
        }

        // Как да се движат enemies, как да изглеждат
        static void DrawEnemies()
        {
            for (int cnt = 0; cnt < enemies.Count; cnt++)
            {
                if (IsObjectInBounds(enemies[cnt].row, enemies[cnt].col))
                {
                    PrintOnPosition(enemies[cnt].row, enemies[cnt].col, enemyFigure, enemyColor);
                }
                
            }
        }

       

        static void UpdateEnemies()
        {
            SpawnEnemies();
            for (int cnt = 0; cnt < enemies.Count; cnt++)
            {
                enemies[cnt].col--;
                if (enemies[cnt].col <0)
                {
                    enemies.RemoveAt(cnt);
                    cnt--;
                }
            }
        }
        static void SpawnEnemies()
        {
            int chance = randGenerator.Next(100);
            if (chance < enemySpawnChance)
            {
                int row = randGenerator.Next(ScreenUpperBorder, ScreenLowerBorder);
                int col = randGenerator.Next(0, WindowWidth)+EnemyStartOffset+20;

                enemies.Add(new Point()
                {
                    row = row,
                    col = col
                });
            }
        }

        #endregion

        #region Bullet Methods

        static void ClearBullets()
        {
            for (int cnt = 0; cnt < bullets.Count; cnt++)
            {
                if (IsObjectInBounds(bullets[cnt].row,bullets[cnt].col))
                {
                    PrintOnPosition(bullets[cnt].row, bullets[cnt].col, " ", bulletColor);
                }
               
            }
        }
        static void DrawBullets()
        {
            for (int cnt = 0; cnt < bullets.Count; cnt++)
            {
                if (IsObjectInBounds(bullets[cnt].row, bullets[cnt].col))
                {
                    PrintOnPosition(bullets[cnt].row, bullets[cnt].col, bulletFigure, bulletColor);
                }
                
            }
        }
        static void UpdateBullets()
        {
            for (int cnt = 0; cnt < bullets.Count; cnt++)
            {
               bullets[cnt].col++;
                if (bullets[cnt].col > WindowWidth -1)
                {
                    bullets.RemoveAt(cnt);
                    cnt--;
                }
            }
        }

        #endregion

        #region Collisions Methods
        static void ChechEnemyBulletsCollision()
        {
            for (int bulletIndex = 0; bulletIndex < bullets.Count; bulletIndex++)
            {
                for (int enemiesIndex = 0; enemiesIndex < enemies.Count; enemiesIndex++)
                {
                    
                    if (DoObjectsCollide(bullets[bulletIndex].row, bullets[bulletIndex].col, enemies[enemiesIndex].row, enemies[enemiesIndex].col))
                    {
                        bullets.RemoveAt(bulletIndex);
                        enemies.RemoveAt(enemiesIndex);
                        bulletIndex--;
                        enemiesIndex--;
                        break;
                    }
                }
            }
        }

        static void ChechEnemyPlayerCollision()
        {
            for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
            {
               
                if (DoObjectsCollide(enemies[enemyIndex].row, enemies[enemyIndex].col, playerRow, playerCol+2) )
                {
                    IsGameOver = true;
                }
            }
        }


        static void ChechCollisions()
        {
            ChechEnemyBulletsCollision();
            ChechEnemyPlayerCollision();
        }
        #endregion

        #region Main Game Methods
        static void Clear()
        {
            ClearPlayer();
            ClearEnemies();
            ClearBullets();
        }

        static void Draw()
        {
            DrawPlayer();
            DrawEnemies();
            DrawBullets();
        }
        static void Update()
        {
            UpdatePlayer();
            UpdateEnemies();
            UpdateBullets();
        }
        #endregion
    }
}
