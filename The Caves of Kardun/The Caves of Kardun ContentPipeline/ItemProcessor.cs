#region File Description
    //////////////////////////////////////////////////////////////////////////
   // ItemProcessor                                                        //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
using The_Caves_of_Kardun;
using System;
#endregion

namespace The_Caves_of_Kardun_ContentPipeline
{
    [ContentProcessor(DisplayName = "Item Processor")]
    public class ItemProcessor : ContentProcessor<XmlDocument, Item>
    {
        public override Item Process(XmlDocument input, ContentProcessorContext context)
        {
            Item item = new Item();

            XmlNodeList itemNodeList = input.GetElementsByTagName("item");

            foreach (XmlNode itemNode in itemNodeList)
            {
                XmlElement itemElement = (XmlElement)itemNode;

                XmlNodeList nameNodeList = itemElement.GetElementsByTagName("name");
                foreach (XmlNode nameNode in nameNodeList)
                    item.Name = nameNode.InnerText;

                XmlNodeList overworldTextureNameNodeList = itemElement.GetElementsByTagName("overworldTextureName");
                foreach (XmlNode overworldTextureNameNode in overworldTextureNameNodeList)
                    item.OverworldTextureName = overworldTextureNameNode.InnerText;

                XmlNodeList textureNameNodeList = itemElement.GetElementsByTagName("textureName");
                foreach (XmlNode textureNameNode in textureNameNodeList)
                    item.TextureName = textureNameNode.InnerText;

                XmlNodeList typeNodeList = itemElement.GetElementsByTagName("type");
                foreach (XmlNode typeNode in typeNodeList)
                    item.Type = (ItemTypes)Enum.Parse(typeof(ItemTypes), typeNode.InnerText);

                XmlNodeList valueNodeList = itemElement.GetElementsByTagName("value");
                foreach (XmlNode valueNode in valueNodeList)
                    item.Value += int.Parse(valueNode.InnerText);

                XmlNodeList minGoldNodeList = itemElement.GetElementsByTagName("minGold");
                foreach (XmlNode minGoldNode in minGoldNodeList)
                    item.MinGold += int.Parse(minGoldNode.InnerText);

                XmlNodeList maxGoldNodeList = itemElement.GetElementsByTagName("maxGold");
                foreach (XmlNode maxGoldNode in maxGoldNodeList)
                    item.MaxGold += int.Parse(maxGoldNode.InnerText);

                XmlNodeList minDamageNodeList = itemElement.GetElementsByTagName("minDmg");
                foreach (XmlNode minDamageNode in minDamageNodeList)
                    item.MinDamage += int.Parse(minDamageNode.InnerText);

                XmlNodeList maxDamageNodeList = itemElement.GetElementsByTagName("maxDmg");
                foreach (XmlNode maxDamageNode in maxDamageNodeList)
                    item.MaxDamage += int.Parse(maxDamageNode.InnerText);

                XmlNodeList dotDamageNodeList = itemElement.GetElementsByTagName("dotDmg");
                foreach (XmlNode dotDamageNode in dotDamageNodeList)
                    item.DotDamage += int.Parse(dotDamageNode.InnerText);

                XmlNodeList dotDurationNodeList = itemElement.GetElementsByTagName("dotDuration");
                foreach (XmlNode dotDurationNode in dotDurationNodeList)
                    item.DotDuration += int.Parse(dotDurationNode.InnerText);

                XmlNodeList healthNodeList = itemElement.GetElementsByTagName("hp");
                foreach (XmlNode healthNode in healthNodeList)
                    item.Health += int.Parse(healthNode.InnerText);

                XmlNodeList blockNodeList = itemElement.GetElementsByTagName("block");
                foreach (XmlNode blockNode in blockNodeList)
                    item.Block += int.Parse(blockNode.InnerText);

                XmlNodeList missNodeList = itemElement.GetElementsByTagName("miss");
                foreach (XmlNode missNode in missNodeList)
                    item.MissChance += int.Parse(missNode.InnerText);

                XmlNodeList speedNodeList = itemElement.GetElementsByTagName("speed");
                foreach (XmlNode speedNode in speedNodeList)
                    item.Speed += int.Parse(speedNode.InnerText);

                XmlNodeList specialNodeList = itemElement.GetElementsByTagName("special");
                foreach (XmlNode specialNode in specialNodeList)
                    item.Special = (ItemSpecials)Enum.Parse(typeof(ItemSpecials), specialNode.InnerText);
            }

            return item;
        }
    }
}