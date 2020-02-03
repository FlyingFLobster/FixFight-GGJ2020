using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//Author: Zachary Kidd-Smith
namespace GGJ2020FLOB
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont font;
        private int resX;
        private int resY;
        private bool titleScreenOn; //whether or not the title screen is active, with all other game elements paused.

        private GameTime gameTime;
        public static readonly Random Random = new Random();

        public static List<SoundEffect> SoundEffects;
        private SoundEffectInstance battleTheme;
        private SoundEffectInstance menuTheme;
        /*
         * The sound effect instance class  is used instead of the song class
         * due to the song class having pauses in the loops when the song ends,
         * This results in more memory used but with only one or two songs it should
         * be okay.
         */

        //UI Objects
        public static TextBox TextBox;
        private TextBox hpDisplay;
        private TextBox apDisplay;
        private TextBox repairDisplay;
        private Button attackButton;
        private Button waterButton;
        private Button rebootButton;
        
        //Game Objects
        private MainCharacter player;
        private Machine machine;
        private int repairCount;

        //private Hammer hammer; //Wasn't enough time to implement :(
        
        //Background
        private Texture2D background;
        private Effect bgWarp;
        private float fxTimer;
        private const float fxSpeed = 0.02f;

        private Texture2D titleBG;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            SoundEffects = new List<SoundEffect>();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameTime = new GameTime();
            fxTimer = 0.0f;
            repairCount = 0;
            titleScreenOn = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            resX = GraphicsDevice.PresentationParameters.BackBufferWidth;
            resY = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // TODO: use this.Content to load your game content here
            
            //Load Sprites
            font = Content.Load<SpriteFont>("Font");
            
            Texture2D textBoxBG = Content.Load<Texture2D>("ggj2020boxbg");
            TextBox = new TextBox(textBoxBG, 0.8f, 0, 0, 256, 128, font);
            repairDisplay = new TextBox(textBoxBG, 0.6f, 0, TextBox.GetBoxHeight(), 128, 35, font);

            Texture2D buttonBG = Content.Load<Texture2D>("ggj2020buttonbg");
            attackButton = new Button(buttonBG, 1.0f, (resX / 3) + 60, resY / 2, 65, 35, font, "REPAIR");
            waterButton = new Button(buttonBG, 1.0f, (resX / 3) + 30, (resY / 2) + 35, 65, 35, font, "WATER");
            rebootButton = new Button(buttonBG, 1.0f, (resX / 3) + 90, (resY / 2) + 35, 65, 35, font, "REBOOT");

            
            Texture2D playerSprite = Content.Load<Texture2D>("ggj2020mechanicbig");
            player = new MainCharacter(playerSprite, new Vector2(200,300));
            apDisplay = new TextBox(textBoxBG, 1.0f, (resX / 3) + 60, (resY / 2) - 35, 65, 35, font);
            
            List<Texture2D> machineSprites =new List<Texture2D>();
            machineSprites.Add(Content.Load<Texture2D>("ggj2020machineblankbig"));
            machineSprites.Add(Content.Load<Texture2D>("ggj2020machinebrokenbig2"));
            machineSprites.Add(Content.Load<Texture2D>("ggj2020machinebrokenbig1"));
            machineSprites.Add(Content.Load<Texture2D>("ggj2020machinefixedbig1"));
            machineSprites.Add(Content.Load<Texture2D>("ggj2020machinefixedbig2"));
            machineSprites.Add(Content.Load<Texture2D>("ggj2020machinefixedbig3"));
            machineSprites.Add(Content.Load<Texture2D>("ggj2020machinestopbig"));

            machine = new Machine(machineSprites, new Vector2(600, 200));
            hpDisplay = new TextBox(textBoxBG, 1.0f, 570 - (machineSprites[0].Width / 2), 100, 65, 35, font);

            Texture2D hammerUpSprite = Content.Load<Texture2D>("ggj2020hammer1");
            Texture2D hammerDownSprite = Content.Load<Texture2D>("ggj2020hammer2");
            //hammer = new Hammer(hammerUpSprite, hammerDownSprite, 600, 200, font);

            background = Content.Load<Texture2D>("ggj2020trippybackground");
            titleBG = Content.Load<Texture2D>("ggj2020titlescreennamenobg");
            
            //Load Effects
            //Effect allWhite = Content.Load<Effect>("AllWhite");
            bgWarp = Content.Load<Effect>("ggj2020backgroundshader");

            //Load Sounds
            SoundEffects.Add(Content.Load<SoundEffect>("ggj2020impactsfx"));
            SoundEffects.Add(Content.Load<SoundEffect>("ggj2020watersfx"));
            SoundEffects.Add(Content.Load<SoundEffect>("ggj2020firesfx"));
            SoundEffects.Add(Content.Load<SoundEffect>("ggj2020rebootimpactsfx"));
            SoundEffects.Add(Content.Load<SoundEffect>("ggj2020rebootcorrectsfx"));
            SoundEffects.Add(Content.Load<SoundEffect>("ggj2020rebootincorrectsfx"));


            battleTheme = Content.Load<SoundEffect>("ggj2020battletheme").CreateInstance();
            battleTheme.IsLooped = true;
            menuTheme = Content.Load<SoundEffect>("ggj2020menutheme").CreateInstance();
            menuTheme.IsLooped = true;
            //battleTheme.Play();
            menuTheme.Play();

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (titleScreenOn)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    titleScreenOn = false;
                    
                    menuTheme.Stop();
                    battleTheme.Play();
                    player.SpawnLerp();
                    machine.SpawnLerp();
                    TextBox.AddText("Repair the Machine!");
                }
            }
            else
            {
                //Control Input
                attackButton.Update(Mouse.GetState());
                if (attackButton.getClicked())
                    player.Attack(machine);
            
                waterButton.Update(Mouse.GetState());
                if (waterButton.getClicked())
                    player.Water(machine);
                
                rebootButton.Update(Mouse.GetState());
                if (rebootButton.getClicked())
                    player.RebootMachine(machine);
                
                player.Update(gameTime);
                machine.Update(gameTime);
            
                apDisplay.AddText("AP:" + player.GetAP() + "%");
                hpDisplay.AddText("HP:" + machine.GetHP() + "%");
                repairDisplay.AddText("Repaired: " + repairCount);

                //Reset the machine once it's finished.
                if (machine.IsFinished())
                {
                    machine.Reset();
                    machine.SpawnLerp();
                    repairCount++;
                }
            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            //Draw background (With cool warping effect!)
            fxTimer += fxSpeed;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: bgWarp);

            bgWarp.Parameters["fTimer"].SetValue(fxTimer);
            spriteBatch.Draw(
                background, 
                Vector2.Zero,
                null,
                Color.White,
                0f,
                Vector2.Zero, 
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            spriteBatch.End();
            
            if (titleScreenOn) //Draw only title screen
            {
                spriteBatch.Begin();
                
                spriteBatch.Draw(
                    titleBG, 
                    Vector2.Zero,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero, 
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
                
                spriteBatch.DrawString(font, "Left Click!", Vector2.Zero, Color.White, 0f, Vector2.Zero, new Vector2(2.0f, 2.0f),  SpriteEffects.None, 0f);
                
                spriteBatch.End();
            }
            else //Draw everything else
            {
                //Draw game objects
                spriteBatch.Begin();
            
                player.Draw(spriteBatch);
                machine.Draw(spriteBatch);
                TextBox.Draw(spriteBatch, Color.White);
                attackButton.Draw(spriteBatch);
                waterButton.Draw(spriteBatch);
                rebootButton.Draw(spriteBatch);
                hpDisplay.Draw(spriteBatch, Color.White);
                apDisplay.Draw(spriteBatch, Color.White);
            
                //Draw Score
                repairDisplay.Draw(spriteBatch, Color.White);
            
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}