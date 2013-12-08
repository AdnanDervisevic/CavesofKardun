#region File Description
    //////////////////////////////////////////////////////////////////////////
   // MonsterImporter                                                      //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
#endregion

namespace The_Caves_of_Kardun
{
    [ContentImporter(".tcokm", DisplayName = "TCOK Monster Importer", DefaultProcessor = "TCOK Monster Processor")]
    public class MonsterImporter : ContentImporter<XmlDocument>
    {
        public override XmlDocument Import(string filename, ContentImporterContext context)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            return doc;
        }
    }
}