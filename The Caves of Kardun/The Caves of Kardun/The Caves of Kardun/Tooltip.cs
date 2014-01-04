#region File Description
    //////////////////////////////////////////////////////////////////////////
   // Tooltip                                                              //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion End of Using Statements

namespace The_Caves_of_Kardun
{
    public static class Tooltip
    {
        #region Fields

        private static Texture2D blackTexture;
        private static Texture2D borderTexture;
        private static SpriteFont titleFont;
        private static SpriteFont textFont;
        private static StringBuilder stringBuilder;
        private static bool initialized;
        private static bool draw;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public static Item SelectedItem { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the tooltip class.
        /// </summary>
        /// <param name="titleFont">The font used for titles.</param>
        /// <param name="textFont">The font used for text.</param>
        public static void Initialize(GraphicsDevice graphicsDevice, SpriteFont titleFont, SpriteFont textFont)
        {
            Tooltip.titleFont = titleFont;
            Tooltip.textFont = textFont;
            
            blackTexture = new Texture2D(graphicsDevice, 5, 5, false, graphicsDevice.DisplayMode.Format);
            int size = blackTexture.Width * blackTexture.Height;
            Color[] color = new Color[size];
            for (int i = 0; i < size; i++)
                color[i] = Color.Black;
            blackTexture.SetData<Color>(color);

            borderTexture = new Texture2D(graphicsDevice, 5, 5, false, graphicsDevice.DisplayMode.Format);
            size = borderTexture.Width * borderTexture.Height;
            color = new Color[size];
            for (int i = 0; i < size; i++)
                color[i] = Color.White;
            borderTexture.SetData<Color>(color);

            Tooltip.initialized = true;
        }

        /// <summary>
        /// Show a tooltip for a specific item.
        /// </summary>
        /// <param name="item">The item to show the tooltip for.</param>
        public static void Show(Item item)
        {
            if (!initialized)
                throw new Exception("Error, tooltip is not initialized.");

            if (draw)
                return;

            Tooltip.SelectedItem = item;
            Tooltip.stringBuilder = new StringBuilder(item.Name + "|");

            /*
                Health:
                Block:
                Damage: 10-11
                Dot Damage: 10 over 2
                Miss chance: 50%
                Speed: 2x
                Special: 
            */

            if (item.Health > 0)
            {
                stringBuilder.Append("Health: ");
                stringBuilder.Append(item.Health);
                stringBuilder.Append("\n");
            }

            if (item.Block > 0)
            {
                stringBuilder.Append("Block: ");
                stringBuilder.Append(item.Block);
                stringBuilder.Append("\n");
            }

            if (item.MinDamage > 0 && item.MaxDamage > 0)
            {
                if (item.MinDamage == item.MaxDamage)
                {
                    stringBuilder.Append("Damage: ");
                    stringBuilder.Append(item.MinDamage);
                    stringBuilder.Append("\n");
                }
                else
                {
                    stringBuilder.Append("Damage: ");
                    stringBuilder.Append(item.MinDamage);
                    stringBuilder.Append("-");
                    stringBuilder.Append(item.MaxDamage);
                    stringBuilder.Append("\n");
                }
            }

            if (item.DotDamage > 0 && item.DotDuration > 0)
            {
                stringBuilder.Append("Dot Damage: ");
                stringBuilder.Append(item.DotDamage);
                stringBuilder.Append("/");
                stringBuilder.Append(item.DotDuration);
                stringBuilder.Append("\n");
            }

            if (item.MissChance > 0)
            {
                stringBuilder.Append("Miss Chance: ");
                stringBuilder.Append(item.MissChance);
                stringBuilder.Append("%");
                stringBuilder.Append("\n");
            }

            if (item.Speed > 0)
            {
                stringBuilder.Append("Speed: ");
                stringBuilder.Append(item.Speed);
                stringBuilder.Append("x");
                stringBuilder.Append("\n");
            }
            
            if (item.Special > ItemSpecials.None)
            {
                stringBuilder.Append("Special: ");
                stringBuilder.Append(item.Special);
                stringBuilder.Append("\n");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            draw = true;
        }

        /// <summary>
        /// Show a tooltip for a specific item.
        /// </summary>
        /// <param name="item">The item to show the tooltip for.</param>
        public static void Show(int gold)
        {
            if (!initialized)
                throw new Exception("Error, tooltip is not initialized.");

            if (draw)
                return;

            Tooltip.SelectedItem = null;
            Tooltip.stringBuilder = new StringBuilder("Gold|Amount: " + gold);

            draw = true;
        }

        /// <summary>
        /// Shows a tooltip for the traits.
        /// </summary>
        /// <param name="positiveTraits">The positive traits.</param>
        /// <param name="negativeTraits">The negative traits.</param>
        public static void Show(PositiveTraits positiveTraits, NegativeTraits negativeTraits)
        {
            if (!initialized)
                throw new Exception("Error, tooltip is not initialized.");

            if (draw)
                return;

            Tooltip.stringBuilder = new StringBuilder("Positive Traits|");

            /*
                SuperStrength       = 1,
                SuperLife           = 2,
                ElvenSpeed          = 4,
                TwentyTwentyVision  = 8,
                Ambidextrous        = 16,
            */

            if ((positiveTraits & PositiveTraits.SuperStrength) == PositiveTraits.SuperStrength)
            {
                stringBuilder.Append("- Super Strength");
                stringBuilder.Append("\n");
            }

            if ((positiveTraits & PositiveTraits.SuperLife) == PositiveTraits.SuperLife)
            {
                stringBuilder.Append("- Super Life");
                stringBuilder.Append("\n");
            }

            if ((positiveTraits & PositiveTraits.ElvenSpeed) == PositiveTraits.ElvenSpeed)
            {
                stringBuilder.Append("- Elven Speed");
                stringBuilder.Append("\n");
            }

            if ((positiveTraits & PositiveTraits.TwentyTwentyVision) == PositiveTraits.TwentyTwentyVision)
            {
                stringBuilder.Append("- Twenty Twenty Vision");
                stringBuilder.Append("\n");
            }

            if ((positiveTraits & PositiveTraits.Ambidextrous) == PositiveTraits.Ambidextrous)
            {
                stringBuilder.Append("- Ambidextrous");
                stringBuilder.Append("\n");
            }

            stringBuilder.Append("|Negative Traits|");

            if ((negativeTraits & NegativeTraits.ColourBlind) == NegativeTraits.ColourBlind)
            {
                stringBuilder.Append("- Colour blind");
                stringBuilder.Append("\n");
            }

            if ((negativeTraits & NegativeTraits.MissingAnArm) == NegativeTraits.MissingAnArm)
            {
                stringBuilder.Append("- Missing an Arm");
                stringBuilder.Append("\n");
            }

            if ((negativeTraits & NegativeTraits.MissingAnEye) == NegativeTraits.MissingAnEye)
            {
                stringBuilder.Append("- Missing an Eye");
                stringBuilder.Append("\n");
            }

            if ((negativeTraits & NegativeTraits.NearSighted) == NegativeTraits.NearSighted)
            {
                stringBuilder.Append("- Near Sighted");
                stringBuilder.Append("\n");
            }

            if ((negativeTraits & NegativeTraits.SenseOfDirection) == NegativeTraits.SenseOfDirection)
            {
                stringBuilder.Append("- Sense of Direction");
                stringBuilder.Append("\n");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            draw = true;
        }

        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        public static void Hide()
        {
            if (!initialized)
                throw new Exception("Error, tooltip is not initialized.");

            draw = false;
        }

        /// <summary>
        /// Draws the tooltip.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used to draw.</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (!initialized)
                throw new Exception("Error, tooltip is not initialized.");

            float margin = 3;

            if (draw)
            {
                string[] strings = stringBuilder.ToString().Split('|');
                int[] heights = new int[strings.Length];

                float height = 0;
                float width = 0;

                for (int i = 0; i < strings.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        height += Tooltip.titleFont.MeasureString(strings[i]).Y;
                        
                        float w = Tooltip.titleFont.MeasureString(strings[i]).X;
                        if (w > width)
                            width = w;

                        heights[i] = (int)Tooltip.titleFont.MeasureString(strings[i]).Y + 1;
                    }
                    else
                    {
                        height += Tooltip.textFont.MeasureString(strings[i]).Y;

                        float w = Tooltip.textFont.MeasureString(strings[i]).X;
                        if (w > width)
                            width = w;

                        heights[i] = (int)Tooltip.textFont.MeasureString(strings[i]).Y + 1;
                    }
                }

                Vector2 position = new Vector2(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y);

                if (TheCavesOfKardun.CurrentMouseState.X < TheCavesOfKardun.SCREENWIDTH / 2)
                {
                    // Vänster sida.
                    position.Y -= height + 10;
                    position.X += 10;
                }
                else
                {
                    // Höger sida.
                    position.Y -= height + 10;
                    position.X -= width + 10;
                }

                height += margin * 2;
                width += margin * 2 + 10;

                spriteBatch.Draw(Tooltip.borderTexture, new Rectangle((int)position.X - 3, (int)position.Y - 3, 3, (int)height + 6), Color.White);
                spriteBatch.Draw(Tooltip.borderTexture, new Rectangle((int)(position.X + width), (int)position.Y - 3, 3, (int)height + 6), Color.White);
                spriteBatch.Draw(Tooltip.borderTexture, new Rectangle((int)(position.X), (int)(position.Y - 3), (int)width, 3), Color.White);
                spriteBatch.Draw(Tooltip.borderTexture, new Rectangle((int)(position.X), (int)(position.Y + height), (int)width, 3), Color.White);
                spriteBatch.Draw(Tooltip.blackTexture, new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height), Color.White * 0.4f);

                position += new Vector2(margin);
                for (int i = 0; i < strings.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        // Rubrik
                        spriteBatch.DrawString(titleFont, strings[i], position, Color.White);
                        position.Y += heights[i];
                    }
                    else
                    {
                        // Text
                        spriteBatch.DrawString(textFont, strings[i], position, Color.White);
                        position.Y += heights[i];
                    }
                }
            }
        }

        #endregion
    }
}