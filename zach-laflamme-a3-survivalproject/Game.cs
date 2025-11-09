// Include the namespaces (code libraries) you need below.
using System;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

// The namespace your code is in.
namespace MohawkGame2D
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        // Place your variables here:
        Bullet[] bullets = new Bullet[100];
        int bulletIndex = 0;
        bool playerMoving = false;
        bool playerGrounded = false;
        bool isPlayerJumping = false;
        bool isWallJumpReady = false;
        float speedLimit = 600;
        Vector2 centerScreen = Window.Size / 2.0f;

        Vector2 plrPosition = new Vector2(400 - 20, 100);
        Vector2 plrSize = new Vector2(40, 60);
        Vector2 plrVelocity = new Vector2(0, 0);
        Vector2 plrAcceleration = new Vector2(0, 0);

        Vector2 gravity = new Vector2(0, 1250);

        float friction = 0.1f;
        float elasticity = 0.3f; // added this for the player bouncing off the walls

        bool gameRunning = false;

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetTitle("Bullet Spawning");
            Window.SetSize(800, 600);
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.White);

            ProcessInputs();
            if (gameRunning)
            {
                ProcessPlayerGravity();
                ProcessPlayerCollisions();
                ProcessPlayerMovement();

                if (Input.IsMouseButtonPressed(MouseInput.Left))
                {
                    SpawnBullet(); //test
                }

                DrawPlayer();
            }

            // call update on all bullets
            for (int i = 0; i < bullets.Length; i++)
            {
                // skip the bullet in the array if it hasnt been spawned yet
                if (bullets[i] == null) continue;

                bullets[i].Update();
            }


        }
        void ProcessInputs()
        {
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Escape))
            {
                // when reset button is pressed, move rectangle back to initial position
                plrPosition = centerScreen;
                plrVelocity = new Vector2(0, 0);
                gameRunning = false;
            }
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Enter))
            {
                gameRunning = true;
            }
        }
        void ProcessPlayerGravity()
        {
            plrVelocity += gravity * Time.DeltaTime; // velocity changes because of gravity, position changes because of velocity
            plrPosition += plrVelocity * Time.DeltaTime;
        }
        void DrawPlayer()
        {
            Draw.LineSize = 2;
            Draw.LineColor = Color.Black;
            Draw.FillColor = Color.White;
            Draw.Rectangle(plrPosition, plrSize);
        }
        void ProcessPlayerMovement()
        {
            bool isPlayerMovingLeft = (Input.IsKeyboardKeyDown(KeyboardInput.A)) || (Input.IsKeyboardKeyDown(KeyboardInput.Left));
            bool isPlayerMovingRight = (Input.IsKeyboardKeyDown(KeyboardInput.D)) || (Input.IsKeyboardKeyDown(KeyboardInput.Right));
            bool isPlayerMovingDown = (Input.IsKeyboardKeyDown(KeyboardInput.S)) || (Input.IsKeyboardKeyDown(KeyboardInput.Down));
            bool isPlayerMoving = false;
            float movementSpeed = 1;

            // vertical accelleration for jumping
            if (isPlayerJumping == true)
            {
                for (int i = 5; i > 0; i--)
                {
                    plrAcceleration.Y = i * 100;
                }
            }

            if (isPlayerMoving == true)
            {
                for (int i = 0; i < 8; i++)
                {
                    plrAcceleration.X = i * 50;
                }
            }



            // directional movement
            if (isPlayerMovingLeft && isPlayerMovingRight)
            {
                isPlayerMoving = false;
            }
            
            if (isPlayerMovingLeft == true)
            {
                isPlayerMoving = true;
                plrVelocity.X -= plrAcceleration.X + 100.0f;
            }

            if (isPlayerMovingRight == true)
            {
                isPlayerMoving = true;
                plrVelocity.X += plrAcceleration.X + 100.0f;
            }

            if (isPlayerMovingDown == true)
            {
                isPlayerMoving = true;
                if (plrVelocity.Y > 100.0f)
                {
                    plrVelocity.Y = 80.0f;
                }
                plrVelocity.Y += 600.0f;
            }

            if (!isPlayerMoving)
            {
                plrVelocity.X *= 0.9f;
            }

            // jumping
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Space) && isWallJumpReady == true && playerGrounded == false)
            {
                isWallJumpReady = false;
                plrVelocity.Y -= 400.0f; // jumping... again
                plrVelocity.X *= 2f;
            }

            if (Input.IsKeyboardKeyPressed(KeyboardInput.Space) && playerGrounded == true)
            {
                playerGrounded = false;
                isPlayerJumping = true;
                plrVelocity.Y -= (500.0f + plrAcceleration.Y); // jumping
            }

            // player speed limit
            if (plrVelocity.X > speedLimit)
            {
                plrVelocity.X = speedLimit;
            }
            if (plrVelocity.X < -speedLimit)
            {
                plrVelocity.X = -speedLimit;
            }
        }
        void ProcessPlayerCollisions()
        {
            float topEdge = plrPosition.Y;
            float bottomEdge = plrPosition.Y + plrSize.Y;
            float leftEdge = plrPosition.X;
            float rightEdge = plrPosition.X + plrSize.X;

            if (topEdge < 0)
            {
                plrVelocity.Y *= -1 * friction;
                plrPosition.Y = 0;
            }

            if (bottomEdge > Window.Height)
            {
                playerGrounded = true;
                isWallJumpReady = false;
                plrVelocity.Y *= 0;
                plrPosition.Y = Window.Height - plrSize.Y;
            }

            if (leftEdge < 0)
            {
                isWallJumpReady = true;
                plrVelocity.X *= -0.5f * elasticity;
                plrPosition.X = 0;
            }
            else isWallJumpReady = false;

            if (rightEdge > Window.Width)
            {
                isWallJumpReady = true;
                plrVelocity.X *= -0.5f * elasticity;
                plrPosition.X = Window.Width - plrSize.X;
            }
            else isWallJumpReady = false;

            if (bottomEdge < Window.Height)
            {
                playerGrounded = false;
            }
        }
        void SpawnBullet()
        {
            // if this bullet slot is already occupied, don't fire a new bullet / only spawn a new bullet if the current slot is unoccupied
            if (bullets[bulletIndex] != null) return;

            // when mouse button is pressed, spawn a bullet!
            Bullet bullet = new Bullet();

            bullet.position = centerScreen;

            Vector2 centerToMouse = Input.GetMousePosition() - centerScreen;
            bullet.velocity = Vector2.Normalize(centerToMouse); // normalize takes the same distance and rotation as this vector, and makes the distance 1 px

            bullets[bulletIndex] = bullet;
            bulletIndex++;

            if (bulletIndex >= bullets.Length) bulletIndex = 0;
        }
    }
}
