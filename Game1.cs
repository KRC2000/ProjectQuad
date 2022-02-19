using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Framework;

namespace MonogameProj1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public const uint CELLSIZE_X = 20;
        public const uint CELLSIZE_Y = 20;

        Level currentLvl = null;
        Camera camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            camera = new Camera();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SetFrameLimit(60);
            SetResolution(800, 600);

            currentLvl = new Level("Levels/Mothership.lvl");    

            #if DEBUG
                Level.PrintDrawCalls = true;
            #else
                Level.PrintDrawCalls = false;
            #endif

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            currentLvl.stamp_t =  Content.Load<Texture2D>("cell");
            currentLvl.font =  Content.Load<SpriteFont>("Font1");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CameraInputUpdate(camera);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Viewport viewport = new Viewport(0, 0, 200, 200);
            //GraphicsDevice.Viewport = viewport;

            // TODO: Add your drawing code here
            currentLvl.Draw(_spriteBatch, camera, GraphicsDevice, camera.GetViewArea(GraphicsDevice.Viewport));

            base.Draw(gameTime);
        }

        private void CameraInputUpdate(Camera cam)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) cam.Move(-CELLSIZE_X, 0);
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) cam.Move(CELLSIZE_X, 0);
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) cam.Move(0, -CELLSIZE_Y);
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) cam.Move(0, CELLSIZE_Y);
        }

        protected void SetFrameLimit(int targetFps)
        {
            if (targetFps != 0)
            {
                _graphics.SynchronizeWithVerticalRetrace = true;
                IsFixedTimeStep = true;
                TargetElapsedTime = TimeSpan.FromSeconds(1d / (double)targetFps);
            }
            else
            {
                _graphics.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }
            _graphics.ApplyChanges();
        }

        protected void SetFullScreen(bool value)
        {
            _graphics.IsFullScreen = value;
            _graphics.ApplyChanges();
        }

        protected void SetResolution(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
        }
    
    }
}
