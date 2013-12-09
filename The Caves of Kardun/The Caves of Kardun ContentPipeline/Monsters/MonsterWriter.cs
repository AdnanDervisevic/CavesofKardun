#region File Description
    //////////////////////////////////////////////////////////////////////////
   // MonsterWriter                                                        //
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
    public class MonsterWriter : ContentTypeWriter<MonstersData>
    {
        protected override void Write(ContentWriter output, MonstersData value)
        {
            output.Write(value.Values.Count);

            foreach (MonsterData monsterData in value.Values)
            {
                output.Write(monsterData.Name);
                output.Write(monsterData.TextureName);
                output.Write(monsterData.Value);
                output.Write(monsterData.Health);
                output.Write(monsterData.MinDamage);
                output.Write(monsterData.MaxDamage);
                output.Write(monsterData.Boss);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "The_Caves_of_Kardun.MonsterReader, The Caves of Kardun";
        }
    }
}
