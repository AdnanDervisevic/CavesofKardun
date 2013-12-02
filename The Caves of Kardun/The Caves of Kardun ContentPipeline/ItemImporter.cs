#region File Description
    //////////////////////////////////////////////////////////////////////////
   // ItemImporter                                                         //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
#endregion

namespace The_Caves_of_Kardun_ContentPipeline
{
    [ContentImporter(".item", DisplayName = "Item Importer", DefaultProcessor = "Item Processor")]
    public class ItemImporter : ContentImporter<XmlDocument>
    {
        public override XmlDocument Import(string filename, ContentImporterContext context)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            return doc;
        }
    }
}
