using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NES.Components;
using System;

namespace NES
{
    struct Log
    {
        static bool isDebug = Environment.GetEnvironmentVariable("BUILD") == "Debug";

        public static void Clear()
        {
            Console.Clear();
        }

        public static void Print(string message)
        {
            if (isDebug)
            {
                Console.Write(message);
            }
        }
    }

    class GFX : Game
    {

        SpriteFont spriteFont;
        Vector2 spriteFontPositionZeroPage = new Vector2(10, 10);
        Vector2 spriteFontPositionRom = new Vector2(10, 300);
        Vector2 spriteFontPositionCpuStatus = new Vector2(450, 10);
        Vector2 spriteFontPositionCurrentInstruction = new Vector2(450, 300);


        Bus nesBus;
        Texture2D canvas;
        Rectangle tracedSize;
        UInt32[] pixels;

        Random rnd = new Random();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public GFX(int x, int y, bool debugInformation = false)
        {
            nesBus = new Bus();
            graphics = new GraphicsDeviceManager(this);

            if (debugInformation)
            {
                x = x * 3;
                y = y * 3;
            }

            LogInfo($"x res set to {x}");
            LogInfo($"y res set to {y}");

            // Window
            graphics.PreferredBackBufferWidth = x;
            graphics.PreferredBackBufferHeight = y;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;



            Log.Clear();
        }

        private void LogInfo(string v)
        {
            System.Console.WriteLine(v);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            tracedSize = GraphicsDevice.PresentationParameters.Bounds;
            canvas = new Texture2D(GraphicsDevice, tracedSize.Width, tracedSize.Height, false, SurfaceFormat.Color);
            pixels = new UInt32[tracedSize.Width * tracedSize.Height];



            spriteBatch = new SpriteBatch(GraphicsDevice);

            Bus.SetTestPrg(0x8000, new byte[28] {
                0xA2, 0x0A, 0x8E, 0x00,0x00,0xA2, 0x03, 0x8E, 0x01, 0x00, 0xAC, 0x00, 0x00, 0xA9, 0x00, 0x18,
                0x6D, 0x01, 0x00, 0x88, 0xD0, 0xFA, 0x8D, 0x02, 0x00, 0xEA, 0xEA, 0xEA
            });


            spriteFont = Content.Load<SpriteFont>("Arial12pt");


            base.Initialize();
            Log.Print("Terminal Active\n");
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        static bool spacePressed = false;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyUp(Keys.Space))
                spacePressed = false;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !spacePressed){
                spacePressed = true;
                do{
                nesBus.Tick();
                }while (!Bus.cpu.Complete());
            }


             
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);

            spriteBatch.Begin();

            var zp = Debug.ArrayToDebugView(nesBus.GetAddressRange(0, 0xFF), 0, 0xFF);
            var rom = Debug.ArrayToDebugView(nesBus.GetAddressRange(0x8000, 0x80FF), 0x8000, 0x80FF);
            var status = Bus.cpu.status;


            spriteBatch.DrawString(spriteFont, zp, spriteFontPositionZeroPage, Color.White);
            spriteBatch.DrawString(spriteFont, rom, spriteFontPositionRom, Color.White);
            spriteBatch.DrawString(spriteFont, Debug.GetCpuStatus(), spriteFontPositionCpuStatus, Color.White);
            spriteBatch.DrawString(spriteFont, Debug.Decompile(), spriteFontPositionCurrentInstruction, Color.White);




            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}