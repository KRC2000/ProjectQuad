using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Framework.Camera;
using Framework.ECS;
using Framework.ECS.Components;

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


        uint player;

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

            currentLvl = new Level("Levels/Level1.lvl");    
            camera.MovementSpeed = 10;

            // Constructing player entity according to the instruction file
            player = Manager.LoadEntity("Entities/Player.ent");

            Manager.GetComponent<GoToComponent>(player).GoTo(300, 100);

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

            // Load and set level textures
            currentLvl.stamp_t =  Content.Load<Texture2D>("cell");
            currentLvl.font =  Content.Load<SpriteFont>("Font1");

            // Load and set player's drawable component's texture
            Manager.GetComponent<DrawableComponent>(player).texture = Content.Load<Texture2D>("pl");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            camera.Update(Keyboard.GetState());

            Manager.GetComponent<GoToComponent>(player).Update();

            // Get world mouse position
            Vector2 mouseWorldPos = Vector2.Transform(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y),
                                                        Matrix.Invert(camera.GetTransform(GraphicsDevice.Viewport)));

            // Make player entity move to the mouse position using GoToComponent
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                Manager.GetComponent<GoToComponent>(player).GoTo(mouseWorldPos.X, mouseWorldPos.Y);
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Viewport viewport = new Viewport(0, 0, 200, 200);
            //GraphicsDevice.Viewport = viewport;

            // TODO: Add your drawing code here
            currentLvl.Draw(_spriteBatch, camera, GraphicsDevice, camera.GetViewArea(GraphicsDevice.Viewport));

            // Draw player entity
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, camera.GetTransform(GraphicsDevice.Viewport));
            Manager.GetComponent<DrawableComponent>(player).Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
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
