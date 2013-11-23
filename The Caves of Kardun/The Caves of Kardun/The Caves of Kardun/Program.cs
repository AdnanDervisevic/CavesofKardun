﻿#region File Description
//----------------------------------------------------------------------------
// Program.cs
//
// Copyright (C) . All rights reserved.
//----------------------------------------------------------------------------
#endregion End of File Description

#region Using Statements
// System
using System;
#endregion End of Using Statements

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// This is the main class for the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TheCavesOfKardun game = new TheCavesOfKardun())
            {
                game.Run();
            }
        }
    }
}