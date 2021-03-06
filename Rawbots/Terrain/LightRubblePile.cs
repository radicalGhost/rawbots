﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Rawbots
{
    class LightRubblePile : Tile
    {
        public LightRubblePile()
        {
			PosX = PosY = 0;
			model = new OBJModel(Game.resourcePath + "/Floor/floor_debris_1.obj");
        }

		public LightRubblePile(int x, int y)
		{
			PosX = x;
			PosY = y;
			model = new OBJModel(Game.resourcePath + "/Floor/floor_debris_1.obj");
		}

        public override void SetRenderMode(RenderMode renderMode)
        {
            base.SetRenderMode(renderMode);
        }

        public override void Render()
        {
            GL.PushMatrix();
			model.Render();
            GL.PopMatrix();
        }
    }
}
