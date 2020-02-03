using System;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2020FLOB
{
    //Author: Zachary Kidd-Smith
    /*
     * This was made very last minute, as such it's derived from the Button class for quick and easy functionality.
     */
    public class Hammer : Button
    {
        //Fields
        private String state;
        private Texture2D hammerUp;
        private Texture2D hammerDown;
        
        //Constructor
        public Hammer(Texture2D inHammerUp, Texture2D inHammerDown, int inX, int inY, SpriteFont inFont) : base(inHammerUp, 1f, inX, inY, inHammerUp.Width, inHammerUp.Height, inFont, "")
        {
            state = "inactive";
            hammerUp = inHammerUp;
            hammerDown = inHammerDown;
        }
        
        //Update
        public void Update()
        {
            base.Update();
        }
        
        //Draw
        public void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
        
        //Mutators
        public void MoveX(int change)
        {
            base.SetBoxX(base.GetBoxX() + change);
        }
        
        public void MoveY(int change)
        {
            base.SetBoxY(base.GetBoxY() + change);
        }
    }
}