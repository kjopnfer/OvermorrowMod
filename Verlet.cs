using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;
using System.Collections.Generic;

namespace OvermorrowMod
{
	public class VerletPoint
	{
		public Vector2 prevPosition;
		public Vector2 position;
		public VerletPoint[] connections;
		public bool locked;

		public VerletPoint(Vector2 _prevPosition, Vector2 _position, VerletPoint[] _connections,bool _locked)
        {
			prevPosition = _prevPosition;
			position = _position;
			connections = _connections;
			locked = _locked;
        }
	}



	public class VerletStick
	{
		public VerletPoint point1;
		public VerletPoint point2;
		public float length;

		public VerletStick(VerletPoint _point1, VerletPoint _point2)
		{
			point1 = _point1;
			point2 = _point2;
			length = Vector2.Distance(point1.position, point2.position);
		}
	}

	static class Verlet
	{
		public static VerletPoint[] CalculateVerlet(VerletPoint[] points, Vector2 down, float delta, int depth = 10,float gravity = 100f)
		{
			int stickCount = 0;
			VerletStick[] sticks = new VerletStick[2];
			foreach (VerletPoint p in points)
			{
				if (!p.locked)
				{
					Vector2 positionBeforeUpdate = p.position;
					p.position += p.position - p.prevPosition;
					p.position += down * gravity * delta * delta;
					p.prevPosition = positionBeforeUpdate;
				}
				
				if (p.connections == null)
                {
					continue;
                }
				Main.NewText(p.connections[0]);
				foreach (VerletPoint p2 in p.connections)
				{
					sticks[stickCount] = new VerletStick(p, p2);
					stickCount++;
				}

			}
			/*	var size = sticks.Length;
				for (int i = 0; i < size; i++)
				{
					for (int j = i + 1; j < size; j++)
					{
						if (sticks[i] == sticks[j])
						{
							for (int k = j; k < size - 1; k++)
							{
								sticks[k] = sticks[k + 1];
							}
							j--;
							size--;
						}
					}
				}
			*/
			//var distinctArray = sticks[0..size];
			//sticks = distinctArray;
			
			for (int i = 0; i < depth; i++)
			{
				foreach (VerletStick stick in sticks)
				{
					Main.NewText(stick.point1.position);
					Vector2 stickCentre = (stick.point1.position + stick.point2.position) / 2;
					Vector2 stickDir = Vector2.Normalize(stick.point1.position - stick.point2.position);
					if (!stick.point1.locked)
					{
						stick.point1.position = stickCentre + stickDir * stick.length / 2;
					}
					if (!stick.point2.locked)
					{
						stick.point2.position = stickCentre - stickDir * stick.length / 2;
					}

				}
			}
			return points;

		}

	}
}