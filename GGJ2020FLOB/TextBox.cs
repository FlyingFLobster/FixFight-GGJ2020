using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Author: Zachary Kidd-Smith
namespace GGJ2020FLOB
{
    public class TextBox : UIBox //Inherits from UIBox to handle all the box-y stuff while TextBox handles putting the text in the box.
    {
        //Fields.
        private SpriteFont font;

        //This is the text the box is currently displaying.
        private String boxText;

        //This is the scaling applied to the text.
        private const float textScale = 1.0F;

        //Constructors.
        public TextBox(Texture2D interiorTexture, float inTransparency, int inX, int inY, int inWidth, int inHeight, SpriteFont inFont) : base(interiorTexture, inTransparency, inX, inY, inWidth, inHeight)
        {
            font = inFont;

            boxText = String.Empty;
        }

        //Update
        public void Update()
        {

        }

        //Draw
        public void Draw(SpriteBatch sb, Color textColor)
        {
            base.Draw(sb);

            //Draws text at the parent box's location while applying the text scale.
            sb.DrawString(spriteFont: font, text:boxText, position: new Vector2(base.GetBoxX(), base.GetBoxY()), origin:new Vector2(), color: textColor, rotation:0f, effects:SpriteEffects.None, layerDepth:0f, scale: textScale);
        }
        
        //Version of Draw that Tints the box, white text for readability.
        public void Draw(SpriteBatch sb, Color tint, Color textColor)
        {
            base.Draw(sb, tint);

            //Draws text at the parent box's location while applying the text scale.
            sb.DrawString(spriteFont: font, text:boxText, position: new Vector2(base.GetBoxX(), base.GetBoxY()), origin:new Vector2(), color: textColor, rotation:0f, effects:SpriteEffects.None, layerDepth:0f, scale: textScale);
        }

        //This is used to add text to the box.
        public void AddText(String inText)
        {
            String parsedText = ParseText('|' + inText); //Line character '|' represents a new string of text, like the * in Undertale.
            boxText = ScrollText(boxText + parsedText);
        }

        //Checks if the next word will go over the box's width, and inserts a newline if it does.
        private String ParseText(String inText)
        {
            String line = String.Empty;
            String outText = String.Empty;
            String[] wordRA = inText.Split(' ');

            foreach (String word in wordRA)
            {
                if ((int)(font.MeasureString(line + word).Length() * textScale) > base.GetBoxWidth())
                {
                    outText = outText + line + '\n';
                    line = String.Empty;
                }

                line = line + word + ' ';
            }

            return outText + line + '\n'; //This ensures the line always ends, so that newlines don't have to manually be added to text.
        }

        //Checks if the new block of text goes over the bottom of the box, and if it does it removes previous lines until it fits.
        private String ScrollText(String inText)
        {
            String outText = inText;

            //The text's height minus 20 of the box height is used to prevent the text going a bit over the box's bottom.
            while ((int)(font.MeasureString(outText).Y * textScale) - GetBoxHeight() * 0.2f > base.GetBoxHeight())
            {
                outText = outText.Substring(outText.IndexOf('\n') + 1);
            }

            return outText;
        }
    }
}