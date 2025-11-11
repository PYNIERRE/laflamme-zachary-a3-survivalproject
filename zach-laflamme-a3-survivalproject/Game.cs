// Include the namespaces (code libraries) you need below.
using System;
using System.ComponentModel.Design;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        Ball[] balls = new Ball[100];
        int ballIndex = 0;
        bool playerMoving = false;
        bool playerGrounded = false;
        bool isPlayerJumping = false;

        bool isWallJumpReady = false;
        bool leftWall;
        bool rightWall;

        float speedLimit = 600;
        float playerSpeed = 1;
        float slowValue = 20.0f;

        Vector2 centerScreen = Window.Size / 2.0f;

        Vector2 plrSize = new Vector2(50, 50);
        float hitboxSize = 12.5f;
        Vector2 hitboxOffset = new Vector2(0,0);
        Vector2 plrPosition = new Vector2(400 - 25, Window.Height - 80);
        Vector2 plrVelocity = new Vector2(0, 0);
        Vector2 plrAcceleration = new Vector2(0, 0);

        // player colour
        int plrR = 255;
        int plrG = 255;
        int plrB = 255;
        string plrFace = "0_0";
        float faceTransparency = 1.0f;
        float hitboxTransparency = 0.2f;

        Vector2 gravity = new Vector2(0, 1500);

        float friction = 0.3f;
        float elasticity = 0f; // added this for the player bouncing off the walls, however it will not be needed

        bool gameRunning = false;

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetTitle("FUBA");
            Window.SetSize(600, 800);
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.White);
            Draw4x4GridLines();
            Draw2x2GridLines();
            Draw1x1GridLines();

            ProcessInputs();
            if (gameRunning)
            {
                ProcessPlayerGravity();
                ProcessPlayerCollisions();
                ProcessPlayerMovement();

                if (Input.IsMouseButtonPressed(MouseInput.Left))
                {
                    SpawnBalls(); //test
                }

                DrawPlayer();
                DrawCorners();
            }

            // call update on all balls
            for (int i = 0; i < balls.Length; i++)
            {
                // skip the ball in the array if it hasnt been spawned yet
                if (balls[i] == null) continue;

                balls[i].Update();
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

        void DrawCorners()
        {
            // corner balls
            Draw.LineSize = 2;
            Draw.LineColor = new ColorF(0.0f, 1f);
            Draw.FillColor = new Color(255, 0, 0);
            Draw.Circle(0, 0, 50);

            Draw.LineSize = 2;
            Draw.LineColor = new ColorF(0.0f, 1f);
            Draw.FillColor = new Color(255, 0, 0);
            Draw.Circle(Window.Width, 0, 50);

            Draw.LineSize = 2;
            Draw.LineColor = new ColorF(0.0f, 1f);
            Draw.FillColor = new Color(255, 0, 0);
            Draw.Circle(Window.Width, Window.Height, 50);

            Draw.LineSize = 2;
            Draw.LineColor = new ColorF(0.0f, 1f);
            Draw.FillColor = new Color(255, 0, 0);
            Draw.Circle(0, Window.Height, 50);
        }
        void DrawPlayer()
        {
            float bodyColour = 0f;
            Color playerColour = new Color(plrR, plrG, plrB);

            Draw.LineSize = 2;
            Draw.LineColor = new ColorF(bodyColour, 1.0f);
            Draw.FillColor = playerColour;
            Draw.Rectangle(plrPosition, plrSize); //drawing player rectangle

            Draw.LineSize = 0;
            Draw.FillColor = new ColorF(0.0f, 0f);
            Draw.FillColor = new ColorF(0.0f, hitboxTransparency);
            Draw.Circle(plrSize.X/2 + plrPosition.X + hitboxOffset.X, plrSize.Y/2 + plrPosition.Y + hitboxOffset.Y, hitboxSize);


            Vector2 faceOffset = new Vector2(-4, -13); // face position
            faceOffset -= plrVelocity / 50; // faceoffset 

            Text.Draw(plrFace, plrPosition - faceOffset);
            Text.Size = 25;
            Text.Color = new ColorF(bodyColour, faceTransparency);

            if (plrR < 0) plrR = 0;
            if (plrG < 0) plrG = 0;
            if (plrB < 0) plrB = 0;

            if (isWallJumpReady == false)
            {
                plrB = 255;
            }
            if (isWallJumpReady == true)
            {
                plrB = 50;
            }
            if (isWallJumpReady == false)
            {
                plrB = 255;
                plrG = 255;
            }
        }
        void ProcessPlayerMovement()
        {
            bool isPlayerMovingLeft = (Input.IsKeyboardKeyDown(KeyboardInput.A)) || (Input.IsKeyboardKeyDown(KeyboardInput.Left));
            bool isPlayerMovingRight = (Input.IsKeyboardKeyDown(KeyboardInput.D)) || (Input.IsKeyboardKeyDown(KeyboardInput.Right));
            bool isPlayerMovingDown = (Input.IsKeyboardKeyDown(KeyboardInput.S)) || (Input.IsKeyboardKeyDown(KeyboardInput.Down));
            bool isPlayerMovingSlow = (Input.IsKeyboardKeyDown(KeyboardInput.LeftShift)) || (Input.IsKeyboardKeyDown(KeyboardInput.RightShift));
            bool isPlayerMoving = false;
            hitboxSize = slowValue;
            hitboxOffset.X = 0;
            hitboxOffset.Y = 0;

            // vertical accelleration for jumping
            if (isPlayerJumping == true)
            {
                for (int i = 5; i > 0; i--)
                {
                    plrAcceleration.Y = i * 100;
                    if (i > 0) isPlayerJumping = false;
                }
            }

            // horizontal accelleration for moving. balances out because of the speed limit
            if (isPlayerMoving == true)
            {
                for (int i = 0; i < 16; i++)
                {
                    plrAcceleration.X = i * 20;
                }
            }

            // directional movement
            if (isPlayerMovingLeft && isPlayerMovingRight)
            {
                isPlayerMoving = false;
                isPlayerMovingLeft = false;
                isPlayerMovingRight = false;
            }
            
            if (isPlayerMovingLeft == true)
            {
                isPlayerMoving = true;
                plrVelocity.X -= plrAcceleration.X + 150.0f * playerSpeed;
                plrFace = "<_<";
                if (isPlayerMovingSlow) hitboxOffset.X = -10;
            }

            if (isPlayerMovingRight == true)
            {
                isPlayerMoving = true;
                plrVelocity.X += plrAcceleration.X + 150.0f * playerSpeed;
                plrFace = ">_>";
                if (isPlayerMovingSlow) hitboxOffset.X = 10;
            }

            if (!isPlayerMoving)
            {
                plrFace = "0_0";
                plrVelocity.X *= 0.9f;
            }

            if (isPlayerMovingDown == true)
            {
                if (plrVelocity.Y < 0f)
                {
                    plrVelocity.Y = 100f;
                }
                for (int i = 0; i < 15; i++)
                {
                    plrVelocity.Y *= 1f;
                }
                plrVelocity.Y += 40f;
                plrFace = "u_u";
            }

            if (isPlayerMovingSlow == true)
            {
                playerSpeed = 0.65f;
                plrVelocity.X *= 0.75f;
                plrFace = "-_-";
            }
            if (isPlayerMovingSlow == false)
            {
                playerSpeed = 1f;
            }
            if (isPlayerMovingSlow && isPlayerMovingDown)
            {
                plrFace = "~_~";
                hitboxOffset.Y = 10;
            }

            if (isWallJumpReady == true && isPlayerMovingDown == false && plrVelocity.Y < -50.0f) 
            {
                plrVelocity.Y = -50.0f;
            }
            else

            // walljumping
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Space) && isWallJumpReady == true && playerGrounded == false)
            {
                isWallJumpReady = false;

                if (leftWall == true)
                {
                    for (int i = 10; i > 0; i--)
                    {
                        plrVelocity.X += i * 20;
                    }
                    plrVelocity.X += 300f + plrAcceleration.X;
                    plrPosition.X += 10f;
                }

                if (rightWall == true)
                {
                    for (int i = 10; i > 0; i--)
                    {
                        plrVelocity.X -= i * 20;
                    }
                    plrVelocity.X -= 300f + plrAcceleration.X;
                    plrPosition.X -= 10f;
                }
                if (plrVelocity.Y > 40)
                {
                    plrVelocity.Y = 40;
                    plrVelocity.Y -= 75.0f; // additional boost incase player is moving downwards
                }

                plrVelocity.Y -= (500.0f + plrAcceleration.Y); // jumping... again
            }

            // regular jumping
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Space) && playerGrounded == true)
            {
                playerGrounded = false;
                isPlayerJumping = true;
                plrVelocity.Y -= (610.0f + plrAcceleration.Y); // jumping
            }

            // player speed limit
            if (plrVelocity.X > speedLimit)
            {
                plrVelocity.X = speedLimit + 0.1f * plrVelocity.X;
            }
            if (plrVelocity.X < -speedLimit)
            {
                plrVelocity.X = -(speedLimit + 0.1f * -plrVelocity.X);
            }

            // slow mode transparency
            if (isPlayerMovingSlow == true)
            {
                faceTransparency = 0.5f;
                hitboxTransparency = 0.5f;
                hitboxSize = 7.5f;
            }
            else
            {
                faceTransparency = 1.0f;
                hitboxTransparency = 0.2f;
                hitboxSize = 12.5f;
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
                isWallJumpReady = false;
                playerGrounded = true;
                plrVelocity.Y *= 0;
                plrPosition.Y = Window.Height - plrSize.Y;
            }

            if (leftEdge < 0)
            {
                leftWall = true;
                isWallJumpReady = true;
                plrVelocity.X *= -0.5f * elasticity;
                plrVelocity.Y *= 0.5f;
                plrPosition.X = 0;
            }

            if (rightEdge > Window.Width)
            {
                rightWall = true;
                isWallJumpReady = true;
                plrVelocity.X *= -0.5f * elasticity;
                plrVelocity.Y *= 0.5f;
                plrPosition.X = Window.Width - plrSize.X;
            }

            if (bottomEdge < Window.Height)
            {
                playerGrounded = false;
            }

            // i have no idea what kind of sorcery is happening here but it works. my theory is that i swapped the parameters around so hard that the zones now turn off with eachother's seperate zones instead
            if (leftEdge > 0 + 20 && leftEdge < centerScreen.X - 50)
            {
                isWallJumpReady = false;
                leftWall = false;
            }
            else if (rightEdge < Window.Width - 20 && rightEdge > centerScreen.X + 50)
            {
                isWallJumpReady = false;
                rightWall = false;
            }
        }
        void SpawnBalls()
        {
            // if this bullet slot is already occupied, don't fire a new bullet / only spawn a new bullet if the current slot is unoccupied
            if (balls[ballIndex] != null) return;

            // when mouse button is pressed, spawn a bullet!
            Ball ball = new Ball();

            ball.ballPosition = centerScreen;

            Vector2 centerToMouse = Input.GetMousePosition() - centerScreen;
            ball.ballVelocity = Vector2.Normalize(centerToMouse); // normalize takes the same distance and rotation as this vector, and makes the distance 1 px

            balls[ballIndex] = ball;
            ballIndex++;

            if (ballIndex >= balls.Length) ballIndex = 0;
        }
        void Draw4x4GridLines()
        {
            Draw.LineSize = 3;
            Draw.LineColor = Color.DarkGray;

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

            int randomx = 0;
            int randomy = 0;
        }
        void Draw2x2GridLines()
        {
            Draw.LineSize = 2;
            Draw.LineColor = Color.LightGray;

            // draw vertical gridlines
            for (int x = 0; x < Window.Width; x += 50)
            {
                Draw.Line(x, 0, x, Window.Height);
            }

            // draw horizontal gridlines
            for (int y = 0; y < Window.Height; y += 50)
            {
                Draw.Line(0, y, Window.Width, y);
            }

            int randomx = 0;
            int randomy = 0;
        }

        void Draw1x1GridLines()
        {
            Draw.LineSize = 1;
            Draw.LineColor = Color.LightGray;

            // draw vertical gridlines
            for (int x = 0; x < Window.Width; x += 25)
            {
                Draw.Line(x, 0, x, Window.Height);
            }

            // draw horizontal gridlines
            for (int y = 0; y < Window.Height; y += 25)
            {
                Draw.Line(0, y, Window.Width, y);
            }
            int randomx = 0;
            int randomy = 0;
        }
    }
}
