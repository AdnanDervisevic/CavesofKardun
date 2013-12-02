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
#endregion

namespace The_Caves_of_Kardun_ContentPipeline
{
    [ContentProcessor(DisplayName = "Item Processor")]
    public class ItemProcessor : ContentProcessor<XmlDocument, Item>
    {
        public override Item Process(XmlDocument input, ContentProcessorContext context)
        {
            Item item = new Item();

            /*
            XmlNodeList mapNodeList = input.GetElementsByTagName("Map");

            foreach (XmlNode mapNode in mapNodeList)
            {
                XmlElement mapElement = (XmlElement)mapNode;

                XmlNodeList tilesetNodeList = mapElement.GetElementsByTagName("Tileset");
                foreach (XmlNode tilesetNode in tilesetNodeList)
                {
                    Tileset tileset = new Tileset();
                    tileset.Name = tilesetNode.Attributes["Name"].InnerText;
                    tileset.TextureName = tilesetNode.Attributes["TextureName"].InnerText;
                    tileset.Dimensions = Size.FromString(tilesetNode.Attributes["Dimensions"].InnerText);
                    tileset.TileSize = Size.FromString(tilesetNode.Attributes["TileSize"].InnerText);
                    tileset.Margin = Size.FromString(tilesetNode.Attributes["Margin"].InnerText);
                    tileset.Spacing = Size.FromString(tilesetNode.Attributes["Spacing"].InnerText);

                    map.AddTileset(tileset);
                }

                XmlNodeList collisionLayerNodeList = mapElement.GetElementsByTagName("CollisionLayer");
                foreach (XmlNode collisionLayerNode in collisionLayerNodeList)
                {
                    XmlNodeList childNodes = collisionLayerNode.ChildNodes;

                }
            }


            XmlNode mapNode = mapNodeList.Item(0);
            Map map = new Map(mapNode.Attributes["Name"].InnerText, Size.FromString(mapNode.Attributes["Dimensions"].InnerText), Size.FromString(mapNode.Attributes["TileSize"].InnerText));

            XmlNodeList tilesetsNodeList = ((XmlElement)mapNode).GetElementsByTagName("Tilesets");
            foreach (XmlNode tilesetNode in tilesetsNodeList)
            {
                Tileset tileset = map.AddTileset("test", "test", Size.Zero, Size.Zero);
                tileset.Margin = new Size(10, 10);
            }
             * */

            return item;
        }
    }
}