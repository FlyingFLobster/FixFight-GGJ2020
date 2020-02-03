using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GGJ2020FLOB
{
    //Author: Zachary Kidd-Smith
    public class Button : TextBox //A Textbox than can be clicked, triggering another method.
    {
        //Fields
        private bool clicked;
        private bool hovered;

        //Constructors
        public Button(Texture2D interiorTexture, float inTransparency, int inX, int inY, int inWidth, int inHeight, SpriteFont inFont, String inLabel) : base(interiorTexture, inTransparency, inX, inY, inWidth, inHeight, inFont)
        {
            clicked = false;
            hovered = false;
            base.AddText(inLabel);
        }
        
        //Draw
        public void Draw(SpriteBatch sb)
        {
            if (hovered)
            {
                base.Draw(sb, Color.Gray, Color.White);
            }
            else
            {
                base.Draw(sb, Color.DarkSlateBlue);
            }
        }
        
        //Update
        public void Update(MouseState mouse)
        {
            Point mousePos = new Point(x: mouse.X, y: mouse.Y);
            if (GetBox().Contains(mousePos))
            {
                hovered = true;
            }
            else
            {
                hovered = false;
            }

            if (hovered && mouse.LeftButton == ButtonState.Pressed)
            {
                clicked = true;
            }
            else
            {
                clicked = false;
            }
        }
        
        //Accessors
        public bool getClicked()
        {
            return clicked;
        }
    }
}