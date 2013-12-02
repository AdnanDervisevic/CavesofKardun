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
using The_Caves_of_Kardun;
#endregion

namespace The_Caves_of_Kardun_ContentPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class ItemWriter : ContentTypeWriter<Item>
    {
        protected override void Write(ContentWriter output, Item value)
        {
            output.Write(value.OverworldTextureName);
            output.Write(value.TextureName);
            output.Write((int)value.Type);
            output.Write(value.Value);
            output.Write(value.MinDamage);
            output.Write(value.MaxDamage);
            output.Write(value.DotDamage);
            output.Write(value.DotDuration);
            output.Write(value.MissChance);
            output.Write(value.Speed);
            output.Write(value.Health);
            output.Write(value.Block);
            output.Write((int)value.Special);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "The_Caves_of_Kardun.ItemReader, The Caves of Kardun";
        }
    }
}
