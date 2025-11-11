using System;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MohawkGame2D
{
	public class Ball
	{
        public Vector2 ballPosition;
        public Vector2 ballVelocity;
        public int ballSize = 50;

        float speed = 300.0f;
        Vector2 gravity = new Vector2(0f, 1.1f);
        int balR = 255;
        int balG = 0;
        int balB = 0;

        public void Setup()
		{

		}

		public void Update()
		{
			ProcessPhysics();
            ProcessBallCollisions();
            ProcessBallGravity();

            DrawBall();

        }

        void ProcessPhysics()
		{
			ballPosition += speed * ballVelocity * Time.DeltaTime; // tells us how far along we nudge that bullet every frame
		}
        void ProcessBallGravity()
        {
            ballVelocity += gravity * Time.DeltaTime; // velocity changes because of gravity, position changes because of velocity
            ballPosition += ballVelocity * Time.DeltaTime;
        }
        void ProcessBallCollisions()
        {
            bool ballHit = false;
            float topEdge = ballPosition.Y - ballSize;
            float bottomEdge = ballPosition.Y + ballSize;
            float leftEdge = ballPosition.X - ballSize;
            float rightEdge = ballPosition.X + ballSize;

            if (topEdge < 0)
            {
                ballVelocity.Y *= -1f;
                ballPosition.Y = 0 + ballSize;
                ballHit = true;
            }

            if (bottomEdge > Window.Height)
            {
                ballVelocity.Y *= -1f;
                ballPosition.Y = Window.Height - ballSize;
                ballHit = true;
            }

            if (leftEdge < 0)
            {
                ballVelocity.X *= -1f;
                ballPosition.X = 0 + ballSize;
                ballHit = true;
            }

            if (rightEdge > Window.Width)
            {
                ballVelocity.X *= -1f;
                ballPosition.X = Window.Width - ballSize;
                ballHit = true;
            }
            if (ballHit)
            {
                ballHit = false;
            }
        }

        void DrawBall()
		{
            Color ballColour = new Color(balR, balG, balB);

            Draw.FillColor = ballColour;
			Draw.LineSize = 2;
            Draw.LineColor = Color.Black;
			Draw.Circle(ballPosition, ballSize);
		}
	}
}
