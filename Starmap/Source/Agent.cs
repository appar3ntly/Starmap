﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public abstract class Agent
	{
		#region Fields
		protected Vector2 position;
		protected Vector2 center;
		protected float heading;
		protected Texture2D agentTexture;
		#endregion

		#region Properties
		public float Heading { get { return heading; } }
		public Vector2 HeadingVector { get { return new Vector2 ((float)Math.Cos (heading), (float)Math.Sin (heading)); } }
		public Texture2D AgentTexture { 
			get { return agentTexture; } 
			set { agentTexture = value; }
		}
		public Vector2 Position { get { return position; } }
		public Vector2 Center { get { return center;} }
		public abstract Rectangle BoundingBox {
			get; 
		}
		public int Width {
			get { return AgentTexture.Width; }
		}
		public int Height {
			get { return AgentTexture.Height; }
		}
		#endregion

		#region Methods
		/*	very simple collision detection method
			all bounding boxes used are rectangular
			to allow use of the Intersects() method
		*/	
		public bool DetectCollision(Agent target){
			if (this.BoundingBox.Intersects (target.BoundingBox)) {
				return true;
			}
			return false;
		}
		#endregion

	}
}

