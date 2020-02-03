using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Author: Zachary Kidd-Smith
namespace GGJ2020FLOB
{
    public class UIBox //This is a simple class intended to draw rectangles to the screen. It will be used as the parent for most UI elements.
    {
        //Fields.
        private Rectangle box;
        private Texture2D boxInteriorTexture;
        private float transparency; //How transparent the boxInteriorTexture is, 1.0f is no transparency, 0.5f is half transparent, etc.

        //Constructors.
        public UIBox(Texture2D interiorTexture, float inTransparency, int inX, int inY, int inWidth, int inHeight)
        {
            box = new Rectangle(x: inX, y: inY, width: inWidth, height: inHeight);
            boxInteriorTexture = interiorTexture;
            transparency = inTransparency;
        }

        //Update
        public void Update()
        {

        }

        //Draw
        public void Draw(SpriteBatch sb)
        {
            //Draw box interior.
            sb.Draw(boxInteriorTexture, box, new Color(Color.White, transparency));
        }
        
        //Version of Draw that tints the box
        public void Draw(SpriteBatch sb, Color tint)
        {
            sb.Draw(boxInteriorTexture, box, new Color(tint, transparency));
        }

        //Accessors, mainly used in child classes to access details about the box.
        public int GetBoxX()
        {
            return box.X;
        }

        public int GetBoxY()
        {
            return box.Y;
        }

        public int GetBoxWidth()
        {
            return box.Width;
        }

        public int GetBoxHeight()
        {
            return box.Height;
        }

        public Rectangle GetBox()
        {
            return box;
        }
        
        //Mutators
        protected void SetBoxX(int inX)
        {
            box.X = inX;
        }

        protected void SetBoxY(int inY)
        {
            box.Y = inY;
        }
    }
}