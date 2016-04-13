﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Starmap
{
	public class Unit : Agent
	{

		#region Fields
		private float moveSpeed = 2.0f;
		private float turnSpeed = MathHelper.ToRadians(5.0f); 
		private int reward;
		private PathNodeSensor pathSensor;
		private const float ANGLE_TOLERANCE = 0.5f;
		private const float DISTANCE_TOLERANCE = 5.0f;
		private float time;
		private int frameIndex;
		private int totalFrames = Game1.Instance.GameSettings.SpriteAnimationFrames;
		private int frameWidth = Game1.Instance.GameSettings.SpriteWidth;
		private int frameHeight = Game1.Instance.GameSettings.SpriteHeight;
		private LinkedList<Tile> path;
		private Thread pathThread;
		#endregion

		#region Properties
		public int HitPoints { get; set; }
		public float MoveSpeed { get { return moveSpeed; } }
		public int Reward { get { return reward; } }
		public override Rectangle BoundingBox {
			get {
				return new Rectangle ((int)position.X, (int)position.Y, Width, Height);
			}
		}
		#endregion

		#region Methods
		public Unit(int reward, Texture2D texture, Vector2 position, List<Tile> path)
		{
			this.reward = reward;
			this.agentTexture = texture;
			this.position = position;
			this.path = new LinkedList<Tile> (path);
			Initialize ();
		}

		public Unit (float moveSpeed, float turnSpeed, int reward, Texture2D texture, Vector2 position)
		{
			this.moveSpeed = moveSpeed;
			this.reward = reward;
			this.agentTexture = texture;
			this.position = position;
			Initialize ();
		}

		private void Initialize()
		{
			pathSensor = new PathNodeSensor (this, 100);
			pathThread = new Thread (FollowPath);
			pathThread.Start ();
		}

		public void Update(Tile goalTile)
		{
			Seek (goalTile.Center);
		}

		public void Draw(SpriteBatch sb, GameTime deltaTime)
		{
			time += (float)deltaTime.ElapsedGameTime.TotalSeconds;
			while (time > Game1.Instance.GameSettings.AnimationFrameTime) {
				frameIndex++;
				if (frameIndex >= totalFrames) {
					frameIndex = 0;
				}
				time = 0f;
			}
			Rectangle source = new Rectangle (frameIndex * frameWidth, 0, frameWidth, frameHeight); 
			sb.Draw (agentTexture, position, source, Color.White);
		}	

		private void FollowPath()
		{
			Point currentLocation;
			while (path.Count > 0) {
				currentLocation = new Point ((int)position.X / Game1.Instance.GameSettings.TileWidth, 
					(int)position.Y / Game1.Instance.GameSettings.TileWidth);
				if (currentLocation == path.First.Value.GridLocation) {
					path.RemoveFirst();
				}
				if (path.Count > 0) {
					Seek (path.First.Value.Center);
					Thread.Sleep (50);
				}
			}
		}

		private void Seek(Vector2 goal)
		{
			Vector2 goalVector = goal - this.Position;

			float velX = 0.0f;
			float velY = 0.0f;
			float relativeHeading = (float)Math.Acos(Vector2.Dot(Vector2.Normalize(this.HeadingVector), Vector2.Normalize(goalVector)));

			// Cross product to determine whether angle is positive/negative
			Vector3 crossResult = Vector3.Cross (new Vector3(HeadingVector.X, HeadingVector.Y, 0), new Vector3(goalVector.X, goalVector.Y, 0));

			if (relativeHeading > ANGLE_TOLERANCE)
			{
				// Rotate in the direction that will reach appropriate
				//  heading the fastest
				if (crossResult.Z >= 0)
					heading += turnSpeed;
				else
					heading -= turnSpeed;
			}
			else
			{
				if (Vector2.Distance (position, goal) > DISTANCE_TOLERANCE)
				{
					velX = (float)(Math.Cos (Heading) * moveSpeed);
					velY = (float)(Math.Sin (Heading) * moveSpeed);
				}
			}

			position += new Vector2 (velX, velY);
		}
		#endregion
	}
}

