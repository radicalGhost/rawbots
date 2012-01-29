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

using Tao.FreeGlut;

namespace Rawbots
{
	public class NuclearWeapon : Weapon
	{
		private double radius;
		private double height;
		
        private CylinderModel[] mcComponent;
        private const int OUTER_CHASSIS = 0;
        private const int INNER_CHASSIS = 1;
        private const int TOTAL_COMPONENTS = 2;

		public NuclearWeapon()
		{
			radius = 1.0f;
			height = 1.0f;
			
			/* slices = 8, stacks = 1 */
			
            mcComponent = new CylinderModel[TOTAL_COMPONENTS];
			
            for (int i = 0; i < mcComponent.Length; i++)
                mcComponent[i] = new CylinderModel(radius, height);
		}
		
		public override int getCost()
		{
			return 20;
		}
		
		public override int getRange()
		{
			return 8;
		}
		
		public override int getLethality()
		{
			/*
			 * Nuclear weapons destroy all robots and factories within an 8 mile
			 * radius of the robot carrying the device - this includes the carrying robot.
			 * This is currently the only method we have to destroy factories and war bases!
			 */
			
			return 0;
		}

        public override void setRenderMode(RenderMode renderMode)
        {
            for (int i = 0; i < mcComponent.Length; i++)
                mcComponent[i].setRenderMode(renderMode);
        }

		public override void Render()
		{
            /* Rotate the object on the X-axis about 90 degrees */
            GL.Rotate(90.0f, 1.0f, 0.0f, 0.0f);

            /************************************************************************/
            /* Drawing the Outer chassis                                            */
            /************************************************************************/

            /* Set the gray color */
            mcComponent[OUTER_CHASSIS].render();

            /* Translate the object -.21 to be above the outer chassis cylinder */
            GL.Translate(0.0f, 0.0f, -0.21f);

            /************************************************************************/
            /* Drawing the Inner chassis                                            */
            /************************************************************************/

            mcComponent[INNER_CHASSIS].render();
		}
	}
}

