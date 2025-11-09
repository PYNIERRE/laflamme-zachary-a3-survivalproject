using System;
using System.Numerics;

namespace MohawkGame2D
{
	public class Bullet
	{
        public Vector2 position;
        public Vector2 velocity;

		float speed = 500.0f;

		public void Setup()
		{

		}

		public void Update()
		{
			ProcessPhysics();
			DrawBullet();
		}

		void ProcessPhysics()
		{
			position += speed * velocity * Time.DeltaTime; // tells us how far along we nudge that bullet every frame
		}

		void DrawBullet()
		{
			Draw.FillColor = Color.Red;
			Draw.LineSize = 2;
			Draw.LineColor = new Color("#FFA500");
			Draw.Circle(position, 7.0f);
		}
	}
}
