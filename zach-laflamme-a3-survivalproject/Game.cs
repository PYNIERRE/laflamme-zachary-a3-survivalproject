// Include the namespaces (code libraries) you need below.
using System;
using System.Numerics;

// The namespace your code is in.
namespace MohawkGame2D
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        // Place your variables here:
        Bullet[] bullets = new Bullet[5];
        int bulletIndex = 0;

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetSize(800, 600);
            Window.SetTitle("Survival");
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(new Color("#F0F0F0"));
            DrawMajorGridLines();
            DrawMinorGridLines();

            if (Input.IsMouseButtonPressed(MouseInput.Left)) SpawnBullet();

            // call update on all bullets
            for (int i = 0; i < bullets.Length; i++)
            {
                // skip the bullet in the array if it hasnt been spawned yet
                if (bullets[i] == null) continue;

                bullets[i].Update();
            }
        }
        void DrawMajorGridLines()
        {
            Draw.LineSize = 2;
            Draw.LineColor = Color.LightGray;

            // draw vertical gridlines
            for (int x = 0; x < Window.Width; x += 100)
            {
                Draw.Line(x, 0, x, Window.Height);
            }

            // draw horizontal gridlines
            for (int y = 0; y < Window.Height; y += 100)
            {
                Draw.Line(0, y, Window.Width, y);
            }
        }
        void DrawMinorGridLines()
        {
            Draw.LineSize = 1;
            Draw.LineColor = Color.LightGray;

            // draw vertical gridlines
            for (int x = 0; x < Window.Width; x += 20)
            {
                Draw.Line(x, 0, x, Window.Height);
            }

            // draw horizontal gridlines
            for (int y = 0; y < Window.Height; y += 20)
            {
                Draw.Line(0, y, Window.Width, y);
            }
        }
        void SpawnBullet()
        {
            // if this bullet slot is already occupied, don't fire a new bullet / only spawn a new bullet if the current slot is unoccupied
            if (bullets[bulletIndex] != null) return;

            // when mouse button is pressed, spawn a bullet!
            Bullet bullet = new Bullet();

            Vector2 centerScreen = Window.Size / 2.0f;

            bullet.position = centerScreen;

            Vector2 centerToMouse = Input.GetMousePosition() - centerScreen;
            bullet.velocity = Vector2.Normalize(centerToMouse); // normalize takes the same distance and rotation as this vector, and makes the distance 1 px

            bullets[bulletIndex] = bullet;
            bulletIndex++;

            if (bulletIndex >= bullets.Length) bulletIndex = 0;
        }
    }



}
