using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Author: Zachary Kidd-Smith
namespace GGJ2020FLOB
{
    /*
     * The Main Class for the enemy Machines that must be fixed.
     */
    public class Machine
    {
        //Fields
        private List<Texture2D> spriteList;
        private Texture2D sprite;
        private Vector2 position;
        private Vector2 ogPosition;
        private float rotation;

        private int hp; //How much hp the Machine has (0->100), restore it to fully heal the machine.
        
        //States and timers
        private String state;
        private double stateTimer;
        private bool flashing;
        private bool spriteOn;
        private double flashTimer;
        private Color tint; //Tint should change a little based on status, used in drawing.
        private bool watered;
        private float lerpAmount;

        //Constructors
        public Machine(List<Texture2D> inSprites, Vector2 inPosition)
        {
            spriteList = inSprites;
            position = inPosition;
            ogPosition = inPosition;
            rotation = 0f;

            hp = 0;
            state = "Normal";
            stateTimer = 0;
            flashing = false;
            spriteOn = true;
            flashTimer = 0;
            tint = Color.White;
            watered = false;

            lerpAmount = 0f;
            
            ChooseSprite();

            //hpBox = new TextBox();
        }
        
        //Draw
        public void Draw(SpriteBatch sb)
        {
            //sb.Draw(sprite, position);
            ChooseSprite(); //Choose which sprite the machine should have

            Vector2 origin = new Vector2(sprite.Width / 2, sprite.Height / 2);

            if (flashing)
            {
                spriteOn = !spriteOn;
                Shake();
            }

            if (spriteOn)
            {
                sb.Draw(
                    sprite, 
                    position,
                    null,
                    tint,
                    rotation,
                    origin,
                    Vector2.One, 
                    SpriteEffects.None,
                    0f
                );
            }

            //sb.DrawString(font, name + ": hp:" + hp + "%", (position - origin), Color.Black);
        }
        
        //Update
        public void Update(GameTime gameTime)
        {
            //Spawn lerping
            if (state.Equals("Spawning"))
            {
                lerpAmount += 0.01f;
                position.Y = position.Y + (ogPosition.Y - position.Y) * lerpAmount; //Lerp from pos.y to ogpos.y

                if (lerpAmount >= 0.85)
                {
                    position = ogPosition;
                    state = "Normal";
                }
            }
            else
            {
                //Fixed state, machine is repaired, let's fling it off the screen!
                if (state.Equals("Fixed"))
                {
                    rotation += 0.1f;
                    if (position.Y < -300) 
                        position.Y -= 5;
                    if (position.X < 1200)
                        position.X += 5;
                    else
                        state = "Finished";
                }

                //Hit Flash
                if (flashing)
                {
                    flashTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (flashTimer >= 200)
                    {
                        flashing = false;
                        flashTimer = 0;
                        spriteOn = true;
                        ResetPosition();

                        if (watered) //reset color if the flash was triggered by water.
                        {
                            tint = Color.White;
                            watered = false;
                        }
                    }
                }
                
                //Status effects
                stateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (state.Equals("Normal"))
                {
                    
                    if (stateTimer >= 2000) //Every 2000 milliseconds there is a chance of the machine experiencing a status effect.
                    {
                        if (Game1.Random.Next(0, 8) == 3) //1 in 7 chance of the machine getting warm, just to add slight variation in timing.
                        {
                            state = "Warm";
                            Game1.TextBox.AddText("The Machine is getting warm!");
                            tint = Color.DarkSalmon;
                        }
                        else if (hp >= 50 && Game1.Random.Next(0, 9) == 3) //1 in 10 chance of machine needing a reboot when above 50% hp
                        {
                            state = "NeedReboot";
                            Game1.TextBox.AddText("The Machine isn't cooperating, Reboot it!");
                        }
                        stateTimer = 0;
                    }
                }
                else if (state.Equals("Warm"))
                {
                    if (stateTimer >= 3000) //Warm for too long and the machine catches fire!
                    {
                        state = "Fire";
                        Game1.TextBox.AddText("The Machine has caught !FIRE!");
                        stateTimer = 0;

                        tint = Color.OrangeRed;
                    }
                }
                else if (state.Equals("Fire"))
                {
                    if (stateTimer >= 1000) //While on fire the machine takes damage over time
                    {
                        if (hp > 0)
                        {
                            int damage = Game1.Random.Next(2, 5);
                            this.SetHP(this.GetHP() - damage);
                            Game1.TextBox.AddText("The Machine has lost -" + damage + "% from the !FIRE!");
                            stateTimer = 0;
                            Game1.SoundEffects[2].Play();
                            this.Shake();
                            
                            if (hp <= 0)
                                Game1.TextBox.AddText("You should probably put the fire out. . .");
                        }
                    }
                }
            }
        }
        
        //Accessors
        public int GetHP()
        {
            return hp;
        }

        public bool IsFinished()
        {
            return state.Equals("Finished");
        }
        
        //Mutators
        public void SetHP(int inHP)
        {
            if (inHP >= 100)
            {
                hp = 100;
                state = "Fixed";
                Game1.TextBox.AddText("The Machine has been repaired!");
                tint = Color.White; //Set as white here so it looks nice while it flies off the screen.
                sprite = spriteList[Game1.Random.Next(3, 5)]; //Choose one of the 'fixed' sprites at random
            }
            else if (inHP < 0)
                hp = 0;
            else
            {
                hp = inHP;
            }
        }

        public void Repair(int damage)
        {
            if (state.Equals("NeedReboot")) //Take damage if affected while needing a reboot.
            {
                hp -= damage;
                Game1.TextBox.AddText("It's bugged! HP lowered by -" + damage + "%. . .");
                Game1.SoundEffects[3].Play();
                state = "Normal";
            }
            else
            {
                SetHP(GetHP() + damage);
                
                //Report results to TextBox
                Game1.TextBox.AddText("You repaired the Machine for " + damage + "%!");
                Game1.SoundEffects[0].Play();
            }
            
            this.Flash();
        }

        public void CoolDown()
        {
            if (state.Equals("Warm"))
            {
                state = "Normal";
                Game1.TextBox.AddText("The Machine cools down.");
                stateTimer = 0;
            }
            else if (state.Equals("Fire"))
            {
                state = "Normal";
                Game1.TextBox.AddText("The Machine's fire is put out!");
                stateTimer = 0;
            }
            else
            {
                Game1.TextBox.AddText("You splashed the machine with water. . .");
            }

            Game1.SoundEffects[1].Play();
            
            tint = Color.MediumBlue;
            watered = true;
            this.Flash();
        }

        public void Reboot()
        {
            int damage = Game1.Random.Next(2, 4);
            
            if (state.Equals("NeedReboot"))
            {
                state = "Normal";
                SetHP(GetHP() + damage);
                Game1.TextBox.AddText("Nice reboot! " + damage + "% restored.");
                Game1.SoundEffects[4].Play();
                stateTimer = 0;
            }
            else
            {
                SetHP(GetHP() - damage);
                Game1.TextBox.AddText("Pointless reboot, " + damage + "% lost. . .");
                Game1.SoundEffects[5].Play();
                this.Flash();
            }
        }

        public void Flash()
        {
            flashing = true;
            flashTimer = 0;
        }

        private void Shake()
        {
            position.X = position.X + Game1.Random.Next(-10, 10);
            position.Y = position.Y + Game1.Random.Next(-10, 10);
        }

        private void ResetPosition()
        {
            position = ogPosition;
        }

        private void ChooseSprite()
        {
            if (!state.Equals("Fixed"))
            {
                if (state.Equals("NeedReboot"))
                    sprite = spriteList[6]; //sprite 6 should be machinestopbig.
                else if (hp >= 50)
                    sprite = spriteList[2];
                else
                    sprite = spriteList[1];
            }
        }

        /*
         * Resets all the machine's fields to how they were at construction,
         * basically treating the machine as if it were brand new so gameplay can continue.
         *
         * This code is getting so smelly. . . But hey it's almost 4am!
         */
        public void Reset()
        {
            rotation = 0f;
            ResetPosition();
            
            hp = 0;
            state = "Normal";
            stateTimer = 0;
            flashing = false;
            spriteOn = true;
            flashTimer = 0;
            tint = Color.White;
            watered = false;
            
            ChooseSprite();
        }

        public void SpawnLerp() //Slightly different version of the SpawnLerp in the MainCHaracter class.
        {
            state = "Spawning"; //Use the Machine classes state instead of using another flag.
            position.Y = -sprite.Height; //Comes from off screen on the top.
            lerpAmount = 0f;
        }
    }
}