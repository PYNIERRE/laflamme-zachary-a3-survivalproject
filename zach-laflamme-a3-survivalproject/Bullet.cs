using System;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MohawkGame2D
{
	public class Ball
	{
        public Vector2 ballPosition;
        public Vector2 ballVelocity;
        public int ballSize = Random.Integer(40, 100);
        public bool ballHit = false;

        float speed = 300.0f;
        Vector2 gravity = new Vector2(0f, 1f);
        int balR = 255;
        int balG = 0;
        int balB = Random.Integer(0, 30);

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
