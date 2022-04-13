using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Framework;
using Framework.Camera;
using Framework.ECS;
using Framework.ECS.Components;
using Framework.DebugUI;

using ProjectQuad.Framework.Components;

namespace ProjectQuad
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public const uint CELLSIZE_X = 30;
        public const uint CELLSIZE_Y = 30;

        Level currentLvl = null;
        Camera camera;


        uint player;


        private Vector2 MouseWorldPos { get; set; }
        private Point MouseWorldGridPos { get; set; }

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
            //SetFrameLimit(0);
            SetFrameLimit(60);
            SetResolution(800, 600);

            Manager.IncludeComponentNamespace("ProjectQuad.Framework.Components");

            currentLvl = new Level("Levels/map1.tmx", "defaultLevel");    
            camera.MovementSpeed = 10;

            // Constructing player entity according to the instruction file
            player = Manager.LoadEntity("Entities/Player.ent");

            Manager.GetComponent<GoToComponent>(player).Locked = true;
            Manager.GetComponent<TravelComponent>(player).TravelTo(new Point(10, 14), currentLvl, true);
            Manager.GetComponent<TravelComponent>(player).TravelTo(new Point(4, 2), currentLvl, true);

            #if DEBUG
            #else
            #endif

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load font for debug ui
            DebugUI.Font = Content.Load<SpriteFont>("Font1");

            // Load and set level textures
            currentLvl.stamp_t =  Content.Load<Texture2D>("cell");
            currentLvl.font = Content.Load<SpriteFont>("Font1");

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
            MouseWorldPos = Vector2.Transform(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y),
                                                        Matrix.Invert(camera.GetTransform(GraphicsDevice.Viewport)));

            MouseWorldGridPos = new Point((int)(MouseWorldPos.X / CELLSIZE_X), (int)(MouseWorldPos.Y / CELLSIZE_Y));

            PlayerControl();

            InputManager.Update(Keyboard.GetState(), Mouse.GetState());
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
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.GetTransform(GraphicsDevice.Viewport));
            Manager.GetComponent<DrawableComponent>(player).Draw(_spriteBatch, new Vector2(CELLSIZE_X, CELLSIZE_Y));
            _spriteBatch.End();

            DebugUI.DrawDebug<float>(_spriteBatch, "Init dist: ", Manager.GetComponent<GoToComponent>(player).InitDistance, 0);
            DebugUI.DrawDebug<float>(_spriteBatch, "Traveled: ", Manager.GetComponent<GoToComponent>(player).Traveled, 1);
            DebugUI.DrawDebug<TimeSpan>(_spriteBatch, "Game time: ", gameTime.ElapsedGameTime.Duration(), 2);
            base.Draw(gameTime);
        }

        private void PlayerControl()
        {
            TravelComponent trav_c = Manager.GetComponent<TravelComponent>(player);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                trav_c.TravelOneStep(TravelComponent.Direction.N, currentLvl);
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                trav_c.TravelOneStep(TravelComponent.Direction.W, currentLvl);
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                trav_c.TravelOneStep(TravelComponent.Direction.S, currentLvl);
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                trav_c.TravelOneStep(TravelComponent.Direction.E, currentLvl);

            if (InputManager.GetState(MouseButtons.Left) == InputState.JustPressed)
                trav_c.TravelTo(MouseWorldGridPos, currentLvl, false);
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
