using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Author: Zachary Kidd-Smith
namespace GGJ2020FLOB
{
    /*
     * The Main Class for the Player Character.
     */
    public class MainCharacter
    {
        //Fields
        private Texture2D sprite;
        private Vector2 position;
        private Vector2 ogPosition;

        //Stats
        private int actionPoints; //Action Points, when they are full (0->100) the player can perform an action.
        private double apTimer; //Amount of time since last action point update in milliseconds.
        private int atk; //Attack Power, used in Attack calculations.
        private int apGrowth; //How much the Action Points fill every time they update.
        
        //State
        private bool spawning;
        private float lerpAmount;

        //Constructors
        public MainCharacter(Texture2D inSprite, Vector2 inPosition)
        {
            sprite = inSprite;
            position = inPosition;
            ogPosition = inPosition;

            actionPoints = 0;
            apTimer = 0;
            atk = 10;
            apGrowth = 3;

            spawning = false;
            lerpAmount = 0f;
        }
        
        //Draw
        public void Draw(SpriteBatch sb /*, SpriteFont font*/)
        {
            Vector2 origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            
            sb.Draw(
                sprite, 
                position,
                null,
                Color.White,
                0f,
                origin,
                Vector2.One, 
                SpriteEffects.None,
                0f
            );
            
            //sb.DrawString(font, "AP:" + actionPoints + "%", (position - origin), Color.Black);
        }
        
        //Update
        public void Update(GameTime gameTime)
        {
            if (spawning) //Lerp sprite onto screen
            {
                lerpAmount += 0.01f;
                position.X = position.X + (ogPosition.X - position.X) * lerpAmount; //Lerp from pos.x to ogpos.x

                if (lerpAmount >= 0.85)
                {
                    position = ogPosition;
                    spawning = false;
                }
            }
            else //regular update logic
            {
                apTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (apTimer >= 110)
                {
                    actionPoints += apGrowth;
                    if (actionPoints > 100)
                        actionPoints = 100;

                    apTimer = 0;
                }
            }
        }
        
        //Accessors
        public int GetAP()
        {
            return actionPoints;
        }
        
        //Player Actions
        //The main Attack used by the player, Restores a small amount of hp to the enemy Machine.
        public void Attack(Machine machine)
        {
            int damage = 0;
            
            if (actionPoints == 100)
            {
                //Damage calc is simply a range between a bit below attack to a bit above attack.
                //The range below is smaller because weak attacks aren't fun :p.
                damage = Game1.Random.Next( atk - (atk / 4), atk + (atk / 2));
                machine.Repair(damage);
                actionPoints = 0;
            }
        }

        public void Water(Machine machine)
        {
            if (actionPoints == 100)
            {
                machine.CoolDown();
                actionPoints = 0;
            }
        }

        public void RebootMachine(Machine machine)
        {
            if (actionPoints == 100)
            {
                machine.Reboot();
                actionPoints = 0;
            }
        }

        public void SpawnLerp() //Set spawning flag and place the sprite off-screen to prepare for lerping back onto screen.
        {
            spawning = true;
            position.X = -sprite.Width; //placing the sprite off-screen to the left
            lerpAmount = 0f;
        }
    }
}