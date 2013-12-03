#region File Description
    //////////////////////////////////////////////////////////////////////////
   // Level                                                                //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion End of Using Statements

namespace The_Caves_of_Kardun
{
    public enum Objects
    {
        None,
        Gold,
        Sword,
        Shield,
        Helmet,
        Boots
    }

    public class Level
    {
        #region Fields

        private int? randomSeed;
        private int amountOfRooms;
        private int maxFails;

        private int[,] mapData;
        private Objects[,] objectData;

        private List<Monster> monsters = new List<Monster>();
        private List<Room> rooms = new List<Room>();
        private Point mapDimensions;

        private Texture2D doorTexture;
        private Texture2D floorTexture;
        private Texture2D wallTexture;

        private Texture2D goldTexture;

        private Random random;

        private ContentManager contentManager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the map data.
        /// </summary>
        public int[,] MapData
        {
            get { return this.mapData; }
        }

        public List<Room> Rooms 
        { 
            get { return this.rooms; }
        }

        public int RoomSpawnIndex { get; private set; }

        public int BossRoomIndex { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="mapDimensions"></param>
        /// <param name="randomSeed"></param>
        /// <param name="amountOfRooms"></param>
        /// <param name="maxFails"></param>
        public Level(ContentManager contentManager, Point mapDimensions, int? randomSeed, int amountOfRooms, int maxFails)
        {
            this.contentManager = contentManager;
            this.mapDimensions = mapDimensions;
            this.mapData = new int[mapDimensions.X, mapDimensions.Y];
            this.objectData = new Objects[mapDimensions.X, mapDimensions.Y];
            for (int x = 0; x < mapDimensions.X; x++)
                for (int y = 0; y < mapDimensions.Y; y++)
                    this.objectData[x, y] = Objects.None;

            this.randomSeed = randomSeed;
            this.amountOfRooms = amountOfRooms;
            this.maxFails = maxFails;

            if (randomSeed.HasValue)
                this.random = new Random(randomSeed.Value);
            else
                this.random = new Random();

            MakeLevel(amountOfRooms, maxFails);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads all the content.
        /// </summary>
        /// <param name="content">The contentManager</param>
        public void LoadContent()
        {
            this.floorTexture = this.contentManager.Load<Texture2D>("Tiles/floor");
            this.wallTexture = this.contentManager.Load<Texture2D>("Tiles/wall");
            this.doorTexture = this.contentManager.Load<Texture2D>("Tiles/door");

            this.goldTexture = this.contentManager.Load<Texture2D>("Objects/gold");
        }

        /// <summary>
        /// Recreates the level.
        /// </summary>
        public void RecreateLevel()
        {
            MakeLevel(this.amountOfRooms, this.maxFails);
        }

        /// <summary>
        /// Called every time monsters should either move or attack.
        /// </summary>
        /// <param name="player">The player.</param>
        public void MonsterAI(Player player)
        {
            Point playerTile = TheCavesOfKardun.ConvertPositionToCell(player.Center);

            foreach (Monster monster in this.monsters)
            {
                if (monster.Health <= 0)
                    continue;

                Point monsterTile = TheCavesOfKardun.ConvertPositionToCell(monster.Center);

                // If the player is to the left of the monster, attack.
                if (monsterTile.X + 1 == playerTile.X && monsterTile.Y == playerTile.Y)
                    monster.Attack(player);
                else if (monsterTile.X - 1 == playerTile.X && monsterTile.Y == playerTile.Y) // If the player is to the right of the monster, attack.
                    monster.Attack(player);
                else if (monsterTile.X == playerTile.X && monsterTile.Y + 1 == playerTile.Y) // If the player is below the monster, attack.
                    monster.Attack(player);
                else if (monsterTile.X == playerTile.X && monsterTile.Y - 1 == playerTile.Y) // If the player is abopve the monster, attack.
                    monster.Attack(player);
            }
        }

        /// <summary>
        /// Checks whether someone can walk in the given direction.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="motion">The direction as a motion vector.</param>
        /// <returns>Returns true if the position can walk in a specific direction; otherwise false.</returns>
        public bool CanWalk(Character character, Vector2 motion, int amountOfTiles, out Vector2 targetPosition)
        {
            Point playerCoords = TheCavesOfKardun.ConvertPositionToCell(character.Center);
            targetPosition = Vector2.Zero;

            if (motion.X == 1 && motion.Y == 0)
            {
                int i = 0;
                for (i = 1; i <= amountOfTiles; i++)
                {
                    if (this.mapData[playerCoords.X + i, playerCoords.Y] != 1 || this.objectData[playerCoords.X + i, playerCoords.Y] != Objects.None)
                        break;

                    bool shouldBreak = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                        if (TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center) == new Point(playerCoords.X + i, playerCoords.Y) && this.monsters[j].Health > 0)
                            shouldBreak = true;

                    if (shouldBreak)
                        break;
                }

                if (i - 1 != 0)
                {
                    targetPosition.X = character.Position.X + TheCavesOfKardun.TileWidth * (i - 1);
                    return true;        
                }
            }
            else if (motion.X == -1 && motion.Y == 0)
            {
                int i = 0;
                for (i = 1; i <= amountOfTiles; i++)
                {
                    if (this.mapData[playerCoords.X - i, playerCoords.Y] != 1 || this.objectData[playerCoords.X - i, playerCoords.Y] != Objects.None)
                        break;

                    bool shouldBreak = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                        if (TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center) == new Point(playerCoords.X - i, playerCoords.Y) && this.monsters[j].Health > 0)
                            shouldBreak = true;

                    if (shouldBreak)
                        break;
                }

                if (i - 1 != 0)
                {
                    targetPosition.X = character.Position.X - TheCavesOfKardun.TileWidth * (i - 1);
                    return true;
                }
            }
            else if (motion.X == 0 && motion.Y == 1)
            {
                int i = 0;
                for (i = 1; i <= amountOfTiles; i++)
                {
                    if (this.mapData[playerCoords.X, playerCoords.Y + i] != 1 || this.objectData[playerCoords.X, playerCoords.Y + i] != Objects.None)
                        break;

                    bool shouldBreak = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                        if (TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center) == new Point(playerCoords.X, playerCoords.Y + i) && this.monsters[j].Health > 0)
                            shouldBreak = true;

                    if (shouldBreak)
                        break;
                }

                if (i - 1 != 0)
                {
                    targetPosition.Y = character.Position.Y + TheCavesOfKardun.TileHeight * (i - 1);
                    return true;
                }
            }
            else if (motion.X == 0 && motion.Y == -1)
            {
                int i = 0;
                for (i = 1; i <= amountOfTiles; i++)
                {
                    if (this.mapData[playerCoords.X, playerCoords.Y - i] != 1 || this.objectData[playerCoords.X, playerCoords.Y - i] != Objects.None)
                        break;

                    bool shouldBreak = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                        if (TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center) == new Point(playerCoords.X, playerCoords.Y - i) && this.monsters[j].Health > 0)
                            shouldBreak = true;

                    if (shouldBreak)
                        break;
                }

                if (i - 1 != 0)
                {
                    targetPosition.Y = character.Position.Y - TheCavesOfKardun.TileHeight * (i - 1);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the player encounters a monster.
        /// </summary>
        /// <param name="targetTile">The tile the player clicked on.</param>
        /// <param name="monster">The monster that the player has encountered.</param>
        /// <returns>Returns true if there are a monster at the targetTile; otherwise false.</returns>
        public bool EncounterMonster(Point targetTile, out Monster monster)
        {
            monster = null;

            for (int i = 0; i < this.monsters.Count; i++)
                if (targetTile == TheCavesOfKardun.ConvertPositionToCell(this.monsters[i].Center) && this.monsters[i].Health > 0)
                    monster = this.monsters[i];

            if (monster != null)
                return true;

            return false;
        }

        /// <summary>
        /// Check if the player encounters an object.
        /// </summary>
        /// <param name="targetTile">The tile the player clicked on.</param>
        /// <param name="typeOfObject">The type of object the player clicked on.</param>
        /// <returns>Returns true if the player clicked on an item; otherwise false.</returns>
        public bool EncounterObject(Point targetTile, out Objects typeOfObject)
        {
            typeOfObject = this.objectData[targetTile.X, targetTile.Y];

            if (typeOfObject != Objects.None)
            {
                this.objectData[targetTile.X, targetTile.Y] = Objects.None;
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Draws the level.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraPosition"></param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 cameraPosition, Vector2 playerPosition, Point min, Point max)
        {
            min.X = Math.Max(min.X, 0);
            min.Y = Math.Max(min.Y, 0);

            max.X = Math.Min(max.X, mapDimensions.X);
            max.Y = Math.Min(max.Y, mapDimensions.Y);

            spriteBatch.Begin();
            for (int x = min.X; x < max.X; x++)
            {
                for (int y = min.Y; y < max.Y; y++)
                {
                    if (mapData[x, y] == 1)
                        spriteBatch.Draw(floorTexture,
                            new Rectangle(
                                (int)(x * TheCavesOfKardun.TileWidth - cameraPosition.X),
                                (int)(y * TheCavesOfKardun.TileHeight - cameraPosition.Y),
                                TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight),
                            Color.White);
                    else if (mapData[x, y] == 2)
                        spriteBatch.Draw(wallTexture,
                            new Rectangle(
                                (int)(x * TheCavesOfKardun.TileWidth - cameraPosition.X),
                                (int)(y * TheCavesOfKardun.TileHeight - cameraPosition.Y),
                                TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight),
                            Color.White);
                    else if (mapData[x, y] == 3)
                        spriteBatch.Draw(doorTexture,
                            new Rectangle(
                                (int)(x * TheCavesOfKardun.TileWidth - cameraPosition.X),
                                (int)(y * TheCavesOfKardun.TileHeight - cameraPosition.Y),
                                TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight),
                            Color.White);

                    if (objectData[x, y] == Objects.Gold)
                        spriteBatch.Draw(this.goldTexture,
                            new Rectangle(
                                (int)(x * TheCavesOfKardun.TileWidth - cameraPosition.X),
                                (int)(y * TheCavesOfKardun.TileHeight - cameraPosition.Y),
                                TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight),
                            Color.White);
                }
            }

            spriteBatch.End();

            for (int i = 0; i < this.monsters.Count; i++)
                this.monsters[i].Draw(gameTime, spriteBatch, cameraPosition);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Creates the entire level.
        /// </summary>
        /// <param name="randomSeed"></param>
        /// <param name="amountOfRooms"></param>
        /// <param name="maxFails"></param>
        private void MakeLevel(int amountOfRooms, int maxFails)
        {
            this.monsters.Clear();
            this.rooms.Clear();
            this.mapData = new int[mapDimensions.X, mapDimensions.Y];
            this.objectData = new Objects[mapDimensions.X, mapDimensions.Y];

            while (this.rooms.Count < amountOfRooms)
            {
                int width = random.Next(8, 15);
                int height = random.Next(8, 15);

                Room room = new Room(
                    random.Next(0, mapDimensions.X - width),
                    random.Next(0, mapDimensions.Y - height),
                    width, height);

                if (!RoomCollides(room))
                    rooms.Add(room);
                else
                {
                    maxFails--;
                    if (maxFails <= 0)
                        break;
                }
            }

            foreach (Room room in this.rooms)
                MakeRoom(room);

            this.RoomSpawnIndex = random.Next(0, this.rooms.Count - 1);
            OrderRoomsByDistanceToSpawn();

            this.BossRoomIndex = random.Next(this.rooms.Count - 4, this.rooms.Count - 1);

            for (int i = 0; i < this.rooms.Count - 1; i++)
            {
                MakeCorridor(this.rooms[i], this.rooms[i + 1]);
            }

            MakeWalls();
            SpawnObjects();
            SpawnMonsters();
        }

        /// <summary>
        /// Spawns monsters on the map.
        /// </summary>
        private void SpawnMonsters()
        {
            for (int i = 0; i < 20; i++)
            {
                int r = this.BossRoomIndex;
                do
                {
                    r = random.Next(1, this.rooms.Count);
                } while (r == this.BossRoomIndex);

                Room room = this.rooms[r];

                Point floorTile = Point.Zero;
                int maxFailes = 10;
                int tries = 0;

                while (true)
                {
                    if (tries == maxFailes)
                        break;

                    floorTile = room.RandomFloorTile;
                    tries++;

                    if (this.objectData[floorTile.X, floorTile.Y] != Objects.None)
                        continue;

                    bool positionAlreadyOccupied = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                    {
                        if (floorTile == TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center))
                        {
                            positionAlreadyOccupied = true;
                            break;
                        }
                    }

                    if (positionAlreadyOccupied)
                        continue;

                    break;
                }

                if (floorTile != Point.Zero)
                {
                    // Skapa random(?) monster
                    this.monsters.Add(new Monster(contentManager.Load<Texture2D>("Sprites/spider"), TheCavesOfKardun.ConvertCellToPosition(floorTile), 500, 90, 
                        contentManager.Load<SpriteFont>("combatFont")));
                }
            }
        }

        /// <summary>
        /// Spawns object on the map.
        /// </summary>
        private void SpawnObjects()
        {
            for (int i = 0; i < 100; i++)
            {
                int r = this.BossRoomIndex;
                do
                {
                    r = random.Next(1, this.rooms.Count);
                } while (r == this.BossRoomIndex);
                              
                Room room = this.rooms[r];

                Point floorTile = Point.Zero;
                int maxFailes = 10;
                int tries = 0;
                do
                {
                    if (tries == maxFailes)
                        break;

                    floorTile = room.RandomFloorTile;
                    tries++;
                } while (floorTile == Point.Zero || this.objectData[floorTile.X, floorTile.Y] != Objects.None);

                if (floorTile != Point.Zero)
                    this.objectData[floorTile.X, floorTile.Y] = Objects.Gold;
            }
        }

        /// <summary>
        /// Creates a room.
        /// </summary>
        /// <param name="room"></param>
        private void MakeRoom(Room room)
        {
            for (int x = 0; x < room.Width; x++)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    if (x == 0 || x == room.Width - 1 || y == 0 || y == room.Height - 1)
                    {
                        mapData[room.Left + x, room.Top + y] = 2;
                        room.MapData[x, y] = 2;
                    }
                    else
                    {
                        mapData[room.Left + x, room.Top + y] = 1;
                        room.MapData[x, y] = 1;
                    }
                }
            }
        }

        /// <summary>
        /// Order the rooms by distance to the spawn room.
        /// </summary>
        private void OrderRoomsByDistanceToSpawn()
        {
            Room tmpRoom = this.rooms[0];
            this.rooms[0] = this.rooms[this.RoomSpawnIndex];
            this.rooms[this.RoomSpawnIndex] = tmpRoom;
            this.RoomSpawnIndex = 0;

            for (int i = 1; i < this.rooms.Count; i++)
                this.rooms[i].DistanceToSpawn = Math.Sqrt(Math.Pow(Math.Abs(this.rooms[i].Left - this.rooms[0].Left), 2) + Math.Pow(Math.Abs(this.rooms[i].Top - this.rooms[0].Top), 2));

            this.rooms = this.rooms.OrderBy(o => o.DistanceToSpawn).ToList();
        }

        /// <summary>
        /// Creates a corridor
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        private void MakeCorridor(Room r1, Room r2)
        {
            int x = r1.Center.X;
            int y = r1.Center.Y;

            if (x == 0 && y == 0)
                return;

            int x2 = r2.Center.X;
            int y2 = r2.Center.Y;

            if (x2 == 0 && y2 == 0)
                return;

            while (x != x2)
            {
                this.mapData[x, y] = 1;

                x += (x < x2) ? 1 : -1;
            }

            while (y != y2)
            {
                this.mapData[x, y] = 1;

                y += (y < y2) ? 1 : -1;
            }

            r1.IsConnect = true;
            r2.IsConnect = true;
        }

        /// <summary>
        /// Creates walls
        /// </summary>
        private void MakeWalls()
        {
            for (int x = 0; x < this.mapDimensions.X; x++)
                for (int y = 0; y < this.mapDimensions.Y; y++)
                    if (this.mapData[x, y] == 0 && HasAdjacentFloor(x, y))
                        this.mapData[x, y] = 2;
        }

        /// <summary>
        /// Helper for checking if a tile has adjacent floor.
        /// </summary>
        /// <param name="x">The X-coordinate of the tile.</param>
        /// <param name="y">The Y-coordinate of the tile.</param>
        /// <returns>Returns true if the tile has adjacent floor; otherwise false.</returns>
        private bool HasAdjacentFloor(int x, int y)
        {
            if (x > 0 && this.mapData[x - 1, y] == 1)
                return true;
            if (x < this.mapDimensions.X - 1 && this.mapData[x + 1, y] == 1)
                return true;

            if (y > 0 && this.mapData[x, y - 1] == 1)
                return true;
            if (y < this.mapDimensions.Y - 1 && this.mapData[x, y + 1] == 1)
                return true;

            if (x > 0 && y > 0 && this.mapData[x - 1, y - 1] == 1)
                return true;
            if (x < mapDimensions.X - 1 && y > 0 && this.mapData[x + 1, y - 1] == 1)
                return true;

            if (x > 0 && y < mapDimensions.Y - 1 && this.mapData[x - 1, y + 1] == 1)
                return true;
            if (x < mapDimensions.X - 1 && y < mapDimensions.Y - 1 && this.mapData[x + 1, y + 1] == 1)
                return true;

            return false;
        }

        /// <summary>
        /// Does the room collide with another room?
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private bool RoomCollides(Room room)
        {
            foreach (Room r in this.rooms)
                if (room.CollidesWith(r))
                    return true;

            return false;
        }

        #endregion
    }
}