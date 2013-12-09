#region File Description
    //////////////////////////////////////////////////////////////////////////
   // MonsterProcessor                                                     //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
using The_Caves_of_Kardun;
using System;
using System.Collections.Generic;
#endregion

namespace The_Caves_of_Kardun
{
    [ContentProcessor(DisplayName = "TCOK Monsters Processor")]
    public class MonsterProcessor : ContentProcessor<XmlDocument, MonstersData>
    {
        public override MonstersData Process(XmlDocument input, ContentProcessorContext context)
        {
            MonstersData monstersData = new MonstersData();
            
            XmlNodeList monsterNodeList = input.GetElementsByTagName("monster");

            foreach (XmlNode monsterNode in monsterNodeList)
            {
                MonsterData monsterData = new MonsterData();

                XmlElement itemElement = (XmlElement)monsterNode;

                XmlNodeList nameNodeList = itemElement.GetElementsByTagName("name");
                foreach (XmlNode nameNode in nameNodeList)
                    monsterData.Name = nameNode.InnerText;

                XmlNodeList textureNameNodeList = itemElement.GetElementsByTagName("textureName");
                foreach (XmlNode textureNameNode in textureNameNodeList)
                    monsterData.TextureName = textureNameNode.InnerText;

                XmlNodeList valueNodeList = itemElement.GetElementsByTagName("value");
                foreach (XmlNode valueNode in valueNodeList)
                    monsterData.Value += int.Parse(valueNode.InnerText);

                XmlNodeList healthNodeList = itemElement.GetElementsByTagName("hp");
                foreach (XmlNode healthNode in healthNodeList)
                    monsterData.Health += int.Parse(healthNode.InnerText);

                XmlNodeList minDamageNodeList = itemElement.GetElementsByTagName("minDmg");
                foreach (XmlNode minDamageNode in minDamageNodeList)
                    monsterData.MinDamage += int.Parse(minDamageNode.InnerText);

                XmlNodeList maxDamageNodeList = itemElement.GetElementsByTagName("maxDmg");
                foreach (XmlNode maxDamageNode in maxDamageNodeList)
                    monsterData.MaxDamage += int.Parse(maxDamageNode.InnerText);

                XmlNodeList bossNodeList = itemElement.GetElementsByTagName("boss");
                foreach (XmlNode bossNode in bossNodeList)
                    monsterData.Boss = bool.Parse(bossNode.InnerText);

                monstersData.Values.Add(monsterData);
            }
            
            return monstersData;
        }
    }
}