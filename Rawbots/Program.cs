﻿/**
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
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Tao.OpenGl;

using QuickFont;

namespace Rawbots
{
	class Game : GameWindow
	{
		Map map;
		int mapWidth;
		int mapHeight;
		bool gameStats = true;
		bool gameOver = false;
		bool useFonts = false;
        bool cameraEnabled = true;
		string baseTitle = "Rawbots";

		QFont font;
		QFont monoFont;

		Config config;
		public static string resourcePath;

		Robot activeRobot;

		Point nullDelta;
		Point mouseDelta;
		Point prevMouseDelta;

		Point mousePosition;
		Point prevMousePosition;

		Camera camera;
		Camera rmcUnitCamera = new Camera(43.0f, 10.0f, 25.0f);
		GlobalCamera globalCamera = new GlobalCamera(0.0f, 10.0f, 25.0f);

		RobotCamera robotCamera = new RobotCamera(0.0f, 0.0f, 0.0f);

        Camera lightCamera1, lightCamera2, lightCamera3, lightCamera4;

		bool cameraHelp = false;
		
		int renderModeCount;
		int shadingModeCount;

		int fovyscale = 4;

		float[] ambientLight = {1.0f, 1.0f, 1.0f}; // white bright light
		bool ambientLights = true;
		
		bool bottomLeftCornerLight = true;
		bool bottomRightCornerLight = true;
		bool topRightCornerLight = true;
		bool topLeftCornerLight = true;
		bool allPostLights = true;

		string gameStatsText;

		string gameStatsFormat =
			"Game Stats:\r\n" +
			"Robots Remaining: {0}\r\n" +
			"Active Robot Life: {1}\r\n";

		string gameOverText =
			"/!\\ GAME OVER /!\\\r\n" +
			"~ ALL OF YOUR RAWBOTS ARE BELONG TO US ~\r\n";

		string cameraHelpText =
			"W: Move Up\r\n" +
			"A: Move Left\r\n" +
			"S: Move Down\r\n" +
			"D: Move Right\r\n" +
			"Q: Roll Left\r\n" +
			"E: Roll Right\r\n" +
			"Left: Rotate Left\r\n" +
			"Right: Rotate Right\r\n" +
			"Up: Rotate Down\r\n" +
			"Down: Rotate Down\r\n" +
			"Tab: Change Camera\r\n" +
			"Escape: Exit Game\r\n";

		Robot someRobot;

		Light l;

		public Game() : base(800, 600, GraphicsMode.Default, "Rawbots")
		{
			VSync = VSyncMode.On;

			renderModeCount = 0;
			shadingModeCount = 0;

			if (IsWindows())
				useFonts = true;

			config = Config.Load();

			this.Width = config.ScreenWidth;
			this.Height = config.ScreenHeight;

			if (config.Fullscreen)
				this.WindowState = WindowState.Fullscreen;

			resourcePath = DetectResourcePath();

			Mouse.Move += new EventHandler<MouseMoveEventArgs>(OnMouseMove);
			Keyboard.KeyDown += new EventHandler<KeyboardKeyEventArgs>(OnKeyDown);
			Keyboard.KeyUp += new EventHandler<KeyboardKeyEventArgs>(OnKeyUp);

			Console.WriteLine("{0}", resourcePath);

			if (useFonts)
			{
				font = new QFont(resourcePath + "/Fonts/Ubuntu-R.ttf", 16);
				font.Options.Colour = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
				font.Options.DropShadowActive = false;

				monoFont = new QFont(resourcePath + "/Fonts/UbuntuMono-R.ttf", 16);
				monoFont.Options.Colour = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
				monoFont.Options.DropShadowActive = false;
			}

			GL.Disable(EnableCap.Texture2D);
		}

		public void LoadMap(string filename)
		{
			int x = 0;
			int y = 0;
			Light light;
		
			map = MapFile.Load(resourcePath + "/Maps/" + filename);

			mapWidth = map.GetWidth();
			mapHeight = map.GetHeight();

			activeRobot = map.GetActiveRobot();
			robotCamera.Attach(activeRobot);
			map.AddRobotToRemoteControlUnitList(activeRobot);

			light = new Light(LightName.Light7);
			activeRobot.AddLight(light);

			someRobot = activeRobot;

			LightPost lightpost = new LightPost(4);
			map.SetTile(lightpost, x, y);

            light = new Light(LightName.Light0);
            light.setCutOff(45.0f);
            light.lookAt(0.0f, 6.0f, 0.0f,
                         2.0f * (float) Math.Sqrt(2.0f), 0.0f, -2.0f * (float) Math.Sqrt(2.0f),
						 1.0f, 1.0f, 1.0f);
            
            lightpost.AddLight(light);

			l = light;
			
            lightCamera1 = new Camera(0.0f, 6.0f, 0.0f,
                         2.0f * (float) Math.Sqrt(2.0f), 0.0f, -2.0f * (float) Math.Sqrt(2.0f),
                         0.0f, 1.0f, 0.0f);

            lightpost = new LightPost(3);
			map.SetTile(lightpost, x + mapWidth - 1, y);

            light = new Light(LightName.Light1);
            light.setCutOff(45.0f);
            light.lookAt(0.0f, 6.0f, 0.0f,
				-2.0f * (float) Math.Sqrt(2.0f), 0.0f, -2.0f * (float) Math.Sqrt(2.0f),
				0.0f, 0.0f, 1.0f);
            lightpost.AddLight(light);

            lightCamera2 = new Camera(mapWidth - 1, 6.0f, 0.0f,
				(mapWidth - 1) - 2.0f * (float) Math.Sqrt(2.0f), 0.0f, -2.0f * (float) Math.Sqrt(2.0f),
				0.0f, 1.0f, 0.0f);

			lightpost = new LightPost(2);
			map.SetTile(lightpost, x + mapWidth - 1, y + mapHeight -1);

            light = new Light(LightName.Light2);
            light.setCutOff(45.0f);
            light.lookAt(0.0f, 6.0f, 0.0f,
				-2.0f * (float) Math.Sqrt(2.0f), 0.0f, 2.0f * (float) Math.Sqrt(2.0f),
				1.0f, 1.0f, 1.0f);
            lightpost.AddLight(light);

            lightCamera3 = new Camera(mapWidth, 6.0f, -(mapHeight - 1),
				(mapWidth - 1) - 2.0f * (float) Math.Sqrt(2.0f), 0.0f, -(mapHeight - 1) + 2.0f * (float) Math.Sqrt(2.0f),
				0.0f, 1.0f, 0.0f);

			lightpost = new LightPost(1);
			map.SetTile(lightpost, x, y + (mapHeight - 1));

            light = new Light(LightName.Light3);
            light.setCutOff(45.0f);
            light.lookAt(0.0f, 6.0f, 0.0f,
				2.0f * (float) Math.Sqrt(2.0f), 0.0f, 2.0f * (float) Math.Sqrt(2.0f),
				1.0f, 1.0f, 1.0f);
            lightpost.AddLight(light);

            lightCamera4 = new Camera(0.0f, 6.0f, -(mapHeight - 1),
				2.0f * (float) Math.Sqrt(2.0f), 0.0f, -(mapHeight - 1) + 2.0f * (float) Math.Sqrt(2.0f),
				0.0f, 1.0f, 0.0f);

            this.Title = this.baseTitle;

			PrintHelp();

			prevMousePosition = this.WindowCenter;
			prevMouseDelta = new Point(0, 0);
			nullDelta = new Point(0, 0);

			globalCamera.SetMap(map);
			robotCamera.SetMap(map);

			camera = globalCamera;

			System.Windows.Forms.Cursor.Hide();

			GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Texture2D);

			/* To Avoid Having Z-Fighting for projecting surfaces onto another surface (Shadows) */
			GL.PolygonOffset(-10.0f, -25.0f);
		}

		public static bool IsWindows()
		{
			int platform = (int) Environment.OSVersion.Platform;

			if ((platform == 4) || (platform == 6) || (platform == 128))
				return false;

			return true;
		}

		public static string GetPathSeparator()
		{
			if (IsWindows())
				return "\\";
			else
				return "/";
		}

		private string DetectResourcePath()
		{
			string path;
			string parent;
			string separator;
			string cwd = Directory.GetCurrentDirectory();

			separator = GetPathSeparator();

			path = cwd + separator + "Resources";

			if (Directory.Exists(path))
				return path;

			parent = Directory.GetParent(cwd).FullName;
			path = parent + separator + "Resources";

			if (Directory.Exists(path))
				return path;

			parent = Directory.GetParent(parent).FullName;
			path = parent + separator + "Resources";

			if (Directory.Exists(path))
				return path;

			return cwd;
		}

		Point WindowCenter
		{
			get { return new Point((Bounds.Left + Bounds.Right) / 2, (Bounds.Top + Bounds.Bottom) / 2); }
		}

		public void PrintHelp()
		{
            Console.WriteLine("Press ESC to Quit Program.");
            Console.WriteLine("R to toggle between Wire/Solid, Solid and Wire Render Modes");
			Console.WriteLine("T to toggle between Flat and Smooth Shading Modes");
			Console.WriteLine("Y to turn the ambient lights on and off");
			Console.WriteLine("U to turn the bottom left lightpost on and off");
			Console.WriteLine("I to turn the bottom right lightpost on and off");
			Console.WriteLine("O to turn the top right lightpost on and off");
			Console.WriteLine("P to turn the top left lightpost on and off");
			Console.WriteLine("L to turn the all lightposts on and off");
			Console.WriteLine("B to switch between Sky Box and Sky Sphere");
            Console.WriteLine("F4 (Show XYZ Plane), F5 (Show XZ Plane), F6 (Show XY Plane), F7 (Show Nothing)");
            Console.WriteLine("F11 (Enable Camera), F12 (Disable Camera)");
			Console.WriteLine("Camera Controls, W/S (Up/Down), A/D (Left/Right), Q/E (Roll L/R)");
			Console.WriteLine("I/K (Adv Forward), J/L (Strafe L/R)");
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
			GL.Enable(EnableCap.DepthTest);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / fovyscale, Width / (float)Height, 1.0f, 120.0f);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);
		}

		public void OnMouseMove(object sender, MouseMoveEventArgs args)
		{
			mouseDelta.X += args.XDelta;
			mouseDelta.Y += args.YDelta;
		}

		public void OnKeyDown(object sender, KeyboardKeyEventArgs args)
		{
			switch (args.Key)
			{
				case Key.H:
					cameraHelp = (cameraHelp) ? false : true;
					break;

				case Key.Tab:

					if (camera == globalCamera)
					{
						camera = robotCamera;
						robotCamera.Attach(someRobot);
					}
					else if (camera == robotCamera)
						camera = lightCamera1;
					else if (camera == lightCamera1)
						camera = lightCamera2;
					else if (camera == lightCamera2)
						camera = lightCamera3;
					else if (camera == lightCamera3)
						camera = lightCamera4;
					else if (camera == lightCamera4)
						camera = rmcUnitCamera;
					else if (camera == rmcUnitCamera)
						camera = globalCamera;
					break;

				case Key.F:
					if (Keyboard[Key.ControlLeft])
					{
						/* toggle fullscreen */

						if (this.WindowState == WindowState.Fullscreen)
							this.WindowState = WindowState.Normal;
						else
							this.WindowState = WindowState.Fullscreen;
					}
					else
					{
						/* toggle light */
						camera.PerformActions(Camera.Action.TOGGLE_LIGHT);
					}
					break;

				case Key.Number5:
					if (ambientLights)
					{
						ambientLights = false;
						setGlobalAmbientLight(0.0f, 0.0f, 0.0f, 1.0f);
					}
					else
					{
						ambientLights = true;
						setGlobalAmbientLight(0.2f, 0.2f, 0.2f, 1.0f);
					}
					break;

				case Key.Number6:
					if (bottomLeftCornerLight)
					{
						bottomLeftCornerLight = true;
						GL.Disable(EnableCap.Light0);
					}
					else
					{
						bottomLeftCornerLight = true;
						GL.Enable(EnableCap.Light0);
					}
					break;

				case Key.Number7:
					if (bottomRightCornerLight)
					{
						bottomRightCornerLight = true;
						GL.Disable(EnableCap.Light1);
					}
					else
					{
						bottomRightCornerLight = true;
						GL.Enable(EnableCap.Light1);
					}
					break;

				case Key.Number8:
					if (topRightCornerLight)
					{
						topRightCornerLight = true;
						GL.Disable(EnableCap.Light2);
					}
					else
					{
						topRightCornerLight = true;
						GL.Enable(EnableCap.Light2);
					}
					break;

				case Key.Number9:
					if (topLeftCornerLight)
					{
						topLeftCornerLight = true;
						GL.Disable(EnableCap.Light3);
					}
					else
					{
						topLeftCornerLight = true;
						GL.Enable(EnableCap.Light3);
					}
					break;

				case Key.Number0:
					if (allPostLights)
					{
						allPostLights = false;
						GL.Disable(EnableCap.Light3);
						GL.Disable(EnableCap.Light2);
						GL.Disable(EnableCap.Light1);
						GL.Disable(EnableCap.Light0);
					}
					else
					{
						allPostLights = true;
						GL.Enable(EnableCap.Light3);
						GL.Enable(EnableCap.Light2);
						GL.Enable(EnableCap.Light1);
						GL.Enable(EnableCap.Light0);
					}
					break;

				case Key.J:
					camera.PerformActions(Camera.Action.TILT_LEFT | Camera.Action.ACTIVE);
					break;

				case Key.L:
					camera.PerformActions(Camera.Action.TILT_RIGHT | Camera.Action.ACTIVE);
					break;

				case Key.I:
					camera.PerformActions(Camera.Action.TILT_UP | Camera.Action.ACTIVE);
					break;

				case Key.K:
					camera.PerformActions(Camera.Action.TILT_DOWN | Camera.Action.ACTIVE);
					break;
				case Key.B:
					SkyBoxSphere.ChangeEnvironment();
					break;
				case Key.Minus:
					if (fovyscale > 2)
					{
						fovyscale--; // Zoom out.

						Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / fovyscale, Width / (float)Height, 1.0f, /*64.0f*/120.0f);
						GL.MatrixMode(MatrixMode.Projection);
						GL.LoadMatrix(ref projection);
					}
					break;
				case Key.Plus:
					if (fovyscale < 10)
					{
						fovyscale++; // Zoom in.

						Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / fovyscale, Width / (float)Height, 1.0f, /*64.0f*/120.0f);
						GL.MatrixMode(MatrixMode.Projection);
						GL.LoadMatrix(ref projection);
					}
					break;
			}
		}
		
		public void OnKeyUp(object sender, KeyboardKeyEventArgs args)
		{
				switch (args.Key)
				{
					case Key.R:
						renderModeCount = ++renderModeCount % 3;

						switch (renderModeCount)
						{
							case 0:
								map.SetRenderMode(RenderMode.SOLID_WIRE);
								break;
						
							case 1:
								map.SetRenderMode(RenderMode.SOLID);
								break;

							case 2:
								map.SetRenderMode(RenderMode.WIRE);
								break;
						}
						break;
				
					case Key.T:
						shadingModeCount = ++shadingModeCount % 2;
					
						switch (shadingModeCount)
						{
							case 0:
								GL.ShadeModel(ShadingModel.Flat);
								break;

							case 1:
								GL.ShadeModel(ShadingModel.Smooth);
								break;
						}
						break;

					case Key.J:
						camera.PerformActions(Camera.Action.TILT_LEFT);
						break;

					case Key.L:
						camera.PerformActions(Camera.Action.TILT_RIGHT);
						break;

					case Key.I:
						camera.PerformActions(Camera.Action.TILT_UP);
						break;

					case Key.K:
						camera.PerformActions(Camera.Action.TILT_DOWN);
						break;

					case Key.M:
						camera.PerformActions(Camera.Action.TOGGLE_MOUSE);
						break;

					case Key.G:
						gameStats = (gameStats) ? false : true;
						break;

					case Key.Z:
						map.HideTextures();
						break;

					case Key.X:
						map.ShowTextures();
						break;
				}
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			//float[] lightPos = l.getPosition();
			//if (Keyboard[Key.Plus])
			//{
			//    l.setPosition(lightPos[0], lightPos[1]+0.1f, lightPos[2], lightPos[3]);
			//}
			//else if (Keyboard[Key.Minus])
			//{
			//    l.setPosition(lightPos[0], lightPos[1] - 0.1f, lightPos[2], lightPos[3]);
			//}

			if (map.isEnemyDefeated() || map.isFriendlyDefeated())
			{
				string statement = map.isEnemyDefeated() ? "YOU WON! Replay a new game? (Y/N)" : "YOU LOST! Replay a new game?";

				if (this.WindowState == WindowState.Fullscreen)
				{
					Console.WriteLine(statement);
					string answer = "";

					while (!(string.Compare(answer, "Y") == 0)
						|| !(string.Compare(answer, "N") == 0)
						|| !(string.Compare(answer, "y") == 0)
						|| !(string.Compare(answer, "n") == 0))
					{
						answer = Console.ReadLine();

						if (string.Compare(answer, "Y") == 0 || string.Compare(answer, "y") == 0)
						{
							instance.LoadMap(mapFile);
							break;
						}
						else if (string.Compare(answer, "N") == 0 || string.Compare(answer, "n") == 0)
						{
							Exit();
							Console.WriteLine("GAME OVER!");
							break;
						}
					}
				}
				else
				{
					gameOver = true;
				}
		   }

			if (Keyboard[Key.Escape])
				Exit();
			if (Keyboard[Key.F4])
				ReferencePlane.setVisibleAxis(ReferencePlane.XYZ);
			else if (Keyboard[Key.F5])
				ReferencePlane.setVisibleAxis(ReferencePlane.XZ);
			else if (Keyboard[Key.F6])
				ReferencePlane.setVisibleAxis(ReferencePlane.XY);
			else if (Keyboard[Key.F7])
				ReferencePlane.setVisibleAxis(ReferencePlane.NONE);
			else if (Keyboard[Key.F11])
				cameraEnabled = false;
			else if (Keyboard[Key.F12])
				cameraEnabled = true;

			mousePosition = new Point(Mouse.X, Mouse.Y);

			mouseDelta = new Point(mousePosition.X - PointToClient(WindowCenter).X,
				mousePosition.Y - PointToClient(WindowCenter).Y);

			if (!mouseDelta.Equals(nullDelta))
			{
				if (camera.MouseDeltaMotion(mouseDelta.X, mouseDelta.Y))
				{
					mouseDelta.X = mouseDelta.Y = 0;
					System.Windows.Forms.Cursor.Position = WindowCenter;
				}
			}

			if (cameraEnabled && (camera != rmcUnitCamera))
			{
				Camera.Action action = Camera.Action.NONE;

				if (Keyboard[Key.Up])
					action |= Camera.Action.MOVE_UP;
				if (Keyboard[Key.Down])
					action |= Camera.Action.MOVE_DOWN;
				if (Keyboard[Key.Left])
					action |= Camera.Action.MOVE_LEFT;
				if (Keyboard[Key.Right])
					action |= Camera.Action.MOVE_RIGHT;
				if (Keyboard[Key.W])
					action |= Camera.Action.ROTATE_UP;
				if (Keyboard[Key.S])
					action |= Camera.Action.ROTATE_DOWN;
				if (Keyboard[Key.D])
					action |= Camera.Action.ROTATE_RIGHT;
				if (Keyboard[Key.A])
					action |= Camera.Action.ROTATE_LEFT;
				if (Keyboard[Key.Q])
					action |= Camera.Action.ROLL_LEFT;
				if (Keyboard[Key.E])
					action |= Camera.Action.ROLL_RIGHT;

				if (action != Camera.Action.NONE)
					camera.PerformActions(action);
				else
					camera.IdleAction();
			}
			else if (camera == rmcUnitCamera)
			{
				if (Keyboard[Key.Space])
					map.HoverRemoteControlUnit();
				if (Keyboard[Key.Left])
					map.MoveRemoteControlUnitLeft();
				if (Keyboard[Key.Right])
					map.MoveRemoteControlUnitRight();
				if (Keyboard[Key.Up])
					map.MoveRemoteControlUnitUp();
				if (Keyboard[Key.Down])
					map.MoveRemoteControlUnitDown();
				if (Keyboard[Key.ShiftLeft])
					map.GrabRobot();
				if (Keyboard[Key.AltLeft])
					map.FireProjectileFromRobot();

				float[] rmcUnitPos = map.GetRemoteControlUnitPosition();

				rmcUnitCamera.LookAt(rmcUnitPos[0], 6.0f, 8.0f - rmcUnitPos[1],
					rmcUnitPos[0], 3.0f, 4.0f - rmcUnitPos[1], 0.0f, 1.0f, 0.0f);
			}
		}

		float[] globLight = { 1.0f, 1.0f, 1.0f, 1.0f };

        public void setGlobalAmbientLight(float r, float g, float b, float a)
        {
            globLight[0] = r; globLight[1] = g; globLight[2] = b; globLight[3] = a;
        }

		protected override void OnRenderFrame(FrameEventArgs e)
		{
            int startTime = Environment.TickCount & Int32.MaxValue;

			base.OnRenderFrame(e);

            GL.LightModel(LightModelParameter.LightModelAmbient, globLight);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.MatrixMode(MatrixMode.Modelview);

			camera.SetView();
		
			SkyBoxSphere.Render(camera);
            
			ReferencePlane.setDimensions(50, 50);
            ReferencePlane.render();

            map.Render();

			//shadowTest();
			
			int totalTime = (Environment.TickCount & Int32.MaxValue) - startTime;

			int fps = 0;

			if (totalTime > 0)
				fps = 1000 / totalTime;

			Title = this.baseTitle + " FPS: " + fps;
			
			if (useFonts)
			{
				QFont.Begin();
				
				GL.PushMatrix();
				GL.Translate(0.0, 0.0, 0.0);
				font.Print(Title, QFontAlignment.Left);
				GL.PopMatrix();
	
				if (cameraHelp)
				{
					GL.PushMatrix();
					GL.Translate(config.ScreenWidth * 0.75, 0.0, 0.0);
					monoFont.Print(cameraHelpText, QFontAlignment.Left);
					GL.PopMatrix();
				}

				if (gameStats)
				{
					GL.PushMatrix();
					GL.Translate(0.0, config.ScreenHeight * 0.10f, 0.0);
					gameStatsText = String.Format(gameStatsFormat,
						map.GetRobots().Count, activeRobot.GetALife());
					monoFont.Print(gameStatsText, QFontAlignment.Left);
					GL.PopMatrix();
				}

				if (gameOver)
				{
					GL.PushMatrix();
					GL.Translate(config.ScreenWidth * 0.50, 0.0, 0.0);
					monoFont.Print(gameOverText, QFontAlignment.Centre);
					GL.PopMatrix();
				}

				QFont.End();
				GL.Disable(EnableCap.Texture2D);
			}

			GL.Flush();

			SwapBuffers();
        }

	    void shadowTest()
		{
			someRobot.RenderAll();

			GL.PushMatrix();
			float[] lPos = l.getPosition();
			GL.Translate(lPos[0], lPos[1], lPos[2]);
			GL.PointSize(50.0f);
			GL.Begin(BeginMode.Points);
			GL.Vertex3(0.0f, 0.0f, 0.0f);
			GL.End();
			GL.PointSize(1.0f);
			GL.PopMatrix();

			float[] mat = l.getShadowMatrix(new float[] { 0.0f, 1.0f, 0.0f, 0.0f });

			GL.Enable(EnableCap.PolygonOffsetFill);
			GL.MultMatrix(mat);
			//GL.Enable(EnableCap.Blend);
			GL.Color4(0.0f, 0.0f, 0.0f, 1.0f);
			someRobot.HideTextures();
			someRobot.RenderAll();
			someRobot.ShowTextures();
			//GL.Disable(EnableCap.Blend);

			GL.Disable(EnableCap.PolygonOffsetFill);
		}

		static string mapFile;
		static Game instance;

		[STAThread]
		static void Main(string[] args)
		{
			string mapFileName = "default.xml";
			mapFile = mapFileName;

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].Equals("-m"))
				{
					mapFileName = args[++i];
				}
			}

			using (Game game = new Game())
			{
				instance = game;
				game.LoadMap(mapFileName);
				game.Run();
			}
		}
	}
}
