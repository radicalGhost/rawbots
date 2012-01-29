/**
 * RawBots: an awesome robot game
 * 
 * Copyright 2012 Marc-Andre Moreau <marcandre.moreau@gmail.com>
 * Copyright 2012 Mark Foo Bonasoro <foo_mark@q8ismobile.com>
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this file,
 * You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Rawbots
{
	public class PhasersWeapon : Weapon
	{
		private double halfCylinderRadius;
		private double halfCylinderHeight;
		private HalfCylinderModel halfCylinder;
		
		public PhasersWeapon()
		{
			halfCylinderRadius = 0.5f;
			halfCylinderHeight = 0.5f;
			halfCylinder = new HalfCylinderModel(halfCylinderRadius, halfCylinderHeight);
			halfCylinder.setColor(0.7f, 0.6f, 0.75f);
		}
		
		public override int getCost()
		{
			return 4;
		}
		
		public override int getRange()
		{
			return 10;
		}
		
		public override int getLethality()
		{
			return 4;
		}

		public override void Render()
		{
			GL.PushMatrix();
			
			halfCylinder.render();
			
			GL.PopMatrix();
		}
	}
}

