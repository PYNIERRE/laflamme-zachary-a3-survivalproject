using System;
using System.Collections.Generic;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MohawkGame2D
{
	public class Ball
	{
        public float fillSize;
        public Vector2 ballPosition;
        public Vector2 ballPosition2;
        public Vector2 ballVelocity;
        public int ballSize = Random.Integer(40, 100);
        public bool ballHit = false;

        public Vector2 circlePosition1 = new Vector2(0, 0);

        float speed = 350.0f;
        Vector2 gravity = new Vector2(0f, 1f);
        int balR = 255;
        int balG = 0;
        int balB = 0;

        public float ballTimeElapsed;
        public int ballTimeElapsedSeconds;

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
            // player hitbox detection
            Game player = new Game();

            /* Vector2 circlePosition1 = new Vector2(0, 0);
            Vector2 circlePosition2 = new Vector2(0, 0);
            float circleRadius1;
            float circleRadius2;

            circlePosition1 = ballPosition;
            circlePosition2 = player.plrPosition + player.hitboxOffset; // checking hitbox position
            circleRadius1 = ballSize;
            circleRadius2 = player.hitboxSize; // checking hitboxm size

            Draw.FillColor = new Color(0, 255, 0);
            Draw.Circle(circlePosition1, 100);
            Draw.Circle(circlePosition2, 100); 

            // Check to see if radii overlap
            float circlesRadii = circleRadius1 + circleRadius2;
            bool doCirclesOverlap = Vector2.Distance(circlePosition1, circlePosition2) <= circlesRadii;
            if (doCirclesOverlap == true)
            {
                Console.WriteLine("test");
                player.gameOver = true;
                player.plrFace = "x_x";
            } 

            Draw.FillColor = new Color(0, 255, 0);
            Draw.Circle(circlePosition1, 100);
            Draw.Circle(circlePosition2, 100); */ 
            //debugging, i wanted to properly incorporate hitboxes but i ran out of time and things didnt add up

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
            ballPosition2 = ballPosition;

            int textSize = 50;

            // adding timer to the ball
            balG += (int)ballVelocity.Y;
            ballTimeElapsedSeconds = (int)ballTimeElapsed; // converting it into tangible whole numbers
            ballTimeElapsed += Time.DeltaTime;

            Color ballColour = new Color(balR, balG, balB);
            fillSize = ballSize;

            Draw.LineColor = ballColour;
            Draw.LineSize = fillSize;
            Draw.FillColor = new ColorF(0.0f, 0.0f);
            Draw.Circle(ballPosition, ballSize - fillSize / 2);

            Draw.LineSize = 2;
            Draw.LineColor = Color.Black;
            Draw.FillColor = new ColorF(0.0f, 0.0f);
            Draw.Circle(ballPosition, ballSize);

            Text.Draw($"{ballTimeElapsedSeconds}", ballPosition.X - 5, ballPosition.Y - 10);
        }
	}
}
