#region File Description
    //////////////////////////////////////////////////////////////////////////
   // ItemWriter                                                           //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.Collections.Generic;
using The_Caves_of_Kardun;
#endregion

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class ItemWriter : ContentTypeWriter<Items>
    {
        protected override void Write(ContentWriter output, Items value)
        {
            output.Write(value.Values.Count);

            foreach (Item item in value.Values)
            {
                output.Write(item.Name);
                output.Write(item.OverworldTextureName);
                output.Write(item.TextureName);
                output.Write((int)item.Type);
                output.Write(item.Value);
                output.Write(item.MinGold);
                output.Write(item.MaxGold);
                output.Write(item.MinDamage);
                output.Write(item.MaxDamage);
                output.Write(item.DotDamage);
                output.Write(item.DotDuration);
                output.Write(item.MissChance);
                output.Write(item.Speed);
                output.Write(item.Health);
                output.Write(item.Block);
                output.Write((int)item.Special);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "The_Caves_of_Kardun.ItemReader, The Caves of Kardun";
        }
    }
}
