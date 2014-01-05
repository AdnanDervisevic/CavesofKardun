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
using System.Collections.ObjectModel;
#endregion End of Using Statements

namespace The_Caves_of_Kardun
{
    public class Level
    {
        #region Fields

        private static int levelCount = 1;

        private int? randomSeed;

        private int maxAmountOfMonstersPerRoom;
        private int amountOfRooms;
        private int maxFails;

        private int[,] mapData;
        private Item[,] itemsData;

        private Point mapDimensions;

        private Texture2D floorTexture;
        private Texture2D wallTexture;

        private MonsterData boss;
        private List<MonsterData> monsterData = new List<MonsterData>();

        private Item ladder;
        private List<Item> goldItems = new List<Item>();
        private List<Item> swordItems = new List<Item>();
        private List<Item> shieldItems = new List<Item>();
        private List<Item> helmItems = new List<Item>();
        private List<Item> bootsItems = new List<Item>();
        private List<Item> potionItems = new List<Item>();

        private List<Room> rooms = new List<Room>();
        private List<Monster> monsters = new List<Monster>();

        private Random random;

        private ContentManager contentManager;

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the level count.
        /// </summary>
        public static int LevelCount { get { return Level.levelCount; } }

        /// <summary>
        /// Gets the map data.
        /// </summary>
        public int[,] MapData
        {
            get { return this.mapData; }
        }

        /// <summary>
        /// Gets the list of rooms.
        /// </summary>
        public ReadOnlyCollection<Room> Rooms 
        { 
            get { return this.rooms.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the player's spawn room index.
        /// </summary>
        public int RoomSpawnIndex { get; private set; }

        /// <summary>
        /// Gets the boss room index.
        /// </summary>
        public int BossRoomIndex { get; private set; }

        /// <summary>
        /// Gets the list of monster as a readonly collection.
        /// </summary>
        public ReadOnlyCollection<Monster> Monsters
        {
            get { return this.monsters.AsReadOnly(); }
        }

        /// <summary>
        /// Gets or sets the player on this level.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets the list of gold items as read only.
        /// </summary>
        public ReadOnlyCollection<Item> GoldItems
        {
            get { return this.goldItems.AsReadOnly(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="contentManager">The content manager used to load content.</param>
        /// <param name="mapDimensions">The dimensions of the map, in tiles.</param>
        /// <param name="randomSeed">The random seed used when randomizing stuff.</param>
        /// <param name="maxAmountOfRooms">The max amount of rooms created.</param>
        /// <param name="maxAmountOfMonstersPerRoom">The max amount of monsters per room.</param>
        /// <param name="maxFails">The max tries before we give up trying to create more rooms.</param>
        public Level(ContentManager contentManager, Point mapDimensions, int? randomSeed, int maxAmountOfRooms, int maxAmountOfMonstersPerRoom, int maxFails)
        {
            this.maxAmountOfMonstersPerRoom = maxAmountOfMonstersPerRoom;
            this.contentManager = contentManager;
            this.mapDimensions = mapDimensions;
            this.mapData = new int[mapDimensions.X, mapDimensions.Y];
            this.itemsData = new Item[mapDimensions.X, mapDimensions.Y];
            for (int x = 0; x < mapDimensions.X; x++)
                for (int y = 0; y < mapDimensions.Y; y++)
                    this.itemsData[x, y] = null;

            this.randomSeed = randomSeed;
            this.amountOfRooms = maxAmountOfRooms;
            this.maxFails = maxFails;

            if (randomSeed.HasValue)
                this.random = new Random(randomSeed.Value);
            else
                this.random = new Random();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads all the content.
        /// </summary>
        /// <param name="content">The contentManager</param>
        public void LoadContent()
        {
            this.floorTexture = this.contentManager.Load<Texture2D>("Textures/Tiles/floor");
            this.wallTexture = this.contentManager.Load<Texture2D>("Textures/Tiles/wall");
            
            //Load Items
            List<Item> items = this.contentManager.Load<List<Item>>("items");

            for (int i = 0; i < items.Count; i++)
            {
                switch (items[i].Type)
                {
                    case ItemTypes.Gold:
                        this.goldItems.Add(items[i]);
                        break;

                    case ItemTypes.Sword:
                        this.swordItems.Add(items[i]);
                        break;

                    case ItemTypes.Shield:
                        this.shieldItems.Add(items[i]);
                        break;

                    case ItemTypes.Boots:
                        this.bootsItems.Add(items[i]);
                        break;

                    case ItemTypes.Helmet:
                        this.helmItems.Add(items[i]);
                        break;

                    case ItemTypes.Potion:
                        this.potionItems.Add(items[i]);
                        break;

                    case ItemTypes.Ladder:
                        this.ladder = items[i];
                        break;
                }
            }

            List<MonsterData> monsterData = this.contentManager.Load<List<MonsterData>>("monsters");
            for (int i = 0; i < monsterData.Count; i++)
            {
                if (monsterData[i].Boss)
                    this.boss = monsterData[i];
                else
                    this.monsterData.Add(monsterData[i]);
            }

            MakeLevel(this.amountOfRooms, this.maxFails);
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void ResetGame()
        {
            Level.levelCount = 1;
            Tooltip.Hide();
            MakeLevel(this.amountOfRooms, this.maxFails);
        }

        /// <summary>
        /// Recreates the level.
        /// </summary>
        public void NextLevel()
        {
            Level.levelCount++;
            this.Player.DamageTaken = 0;
            Tooltip.Hide();
            MakeLevel(this.amountOfRooms, this.maxFails);
        }

        /// <summary>
        /// Called when we should drop boss loot and spawn the ladder to the next level.
        /// </summary>
        public void DropBossLoot()
        {
            Point centerPos = this.rooms[this.BossRoomIndex].Center;
            Point playerPos = TheCavesOfKardun.ConvertPositionToCell(this.Player.Center);

            // 15% chance to spawn a helmet.
            int itemSpawnPosIndex = -1;
            if (random.Next(0, 100) >= 65)
                itemSpawnPosIndex = random.Next(0, 8);

            Point[] lootSpawns = new Point[8];
            lootSpawns[0] = new Point(centerPos.X, centerPos.Y - 2);
            lootSpawns[1] = new Point(centerPos.X + 1, centerPos.Y - 1);
            lootSpawns[2] = new Point(centerPos.X + 2, centerPos.Y);
            lootSpawns[3] = new Point(centerPos.X + 1, centerPos.Y + 1);
            lootSpawns[4] = new Point(centerPos.X, centerPos.Y + 2);
            lootSpawns[5] = new Point(centerPos.X - 1, centerPos.Y + 1);
            lootSpawns[6] = new Point(centerPos.X - 2, centerPos.Y);
            lootSpawns[7] = new Point(centerPos.X - 1, centerPos.Y - 1);

            for (int i = 0; i < lootSpawns.Length; i++)
            {
                if (i == itemSpawnPosIndex)
                    this.itemsData[lootSpawns[i].X, lootSpawns[i].Y] = this.helmItems[random.Next(0, this.helmItems.Count)];
                else
                    this.itemsData[lootSpawns[i].X, lootSpawns[i].Y] = this.goldItems[random.Next(0, this.goldItems.Count)];
            }

            this.itemsData[centerPos.X, centerPos.Y] = this.ladder;
        }

        /// <summary>
        /// Called every time monsters should either move or attack.
        /// </summary>
        /// <param name="player">The player.</param>
        public void UpdateMonstersAI(GameTime gameTime, Player player)
        {
            // The total damage to inflict on the player.
            int totalDamage = 0;

            foreach (Monster monster in this.monsters)
            {
                if (this.Player.MissChance == 0 || random.Next(0, 100) >= this.Player.MissChance)
                    totalDamage += monster.UpdateAI(gameTime, this, player);
            }

            // When we've moved/calculated damage for all monsters then inflict the combined damage to the player.
            if (totalDamage > 0)
                player.InflictDamage(totalDamage - player.Block);
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

            if (character is Player)
            {
                if ((this.Player.NegativeTraits & NegativeTraits.SenseOfDirection) == NegativeTraits.SenseOfDirection)
                {
                    if (motion.X == 1)
                        motion.X = -1;
                    else if (motion.X == -1)
                        motion.X = 1;
                    else if (motion.Y == 1)
                        motion.Y = -1;
                    else if (motion.Y == -1)
                        motion.Y = 1;
                }
            }

            if (motion.X == 1 && motion.Y == 0)
            {
                int i = 0;
                for (i = 1; i <= amountOfTiles; i++)
                {
                    if (this.mapData[playerCoords.X + i, playerCoords.Y] != 1 || this.itemsData[playerCoords.X + i, playerCoords.Y] != null)
                        break;

                    bool shouldBreak = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                        if (TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center) == new Point(playerCoords.X + i, playerCoords.Y) && this.monsters[j].Alive)
                            shouldBreak = true;

                    if (shouldBreak)
                        break;
                }

                if (i - 1 != 0)
                {
                    targetPosition.X = character.Position.X + TheCavesOfKardun.TileWidth * (i - 1);

                    if (character != this.Player && (this.PredictPositions(character) || this.PredictPlayerPosition(character, true)))
                    {
                        targetPosition = character.Position;
                        return true;
                    }

                    if (this.PredictMonsterPosition(character) > 1)
                    {
                        targetPosition = Vector2.Zero;
                        return false;
                    }

                    return true;
                }
            }
            else if (motion.X == -1 && motion.Y == 0)
            {
                int i = 0;
                for (i = 1; i <= amountOfTiles; i++)
                {
                    if (this.mapData[playerCoords.X - i, playerCoords.Y] != 1 || this.itemsData[playerCoords.X - i, playerCoords.Y] != null)
                        break;

                    bool shouldBreak = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                        if (TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center) == new Point(playerCoords.X - i, playerCoords.Y) && this.monsters[j].Alive)
                            shouldBreak = true;

                    if (shouldBreak)
                        break;
                }

                if (i - 1 != 0)
                {
                    targetPosition.X = character.Position.X - TheCavesOfKardun.TileWidth * (i - 1);

                    if (character != this.Player && (this.PredictPositions(character) || this.PredictPlayerPosition(character, true)))
                    {
                        targetPosition = character.Position;
                        return true;
                    }

                    if (this.PredictMonsterPosition(character) > 1)
                    {
                        targetPosition = Vector2.Zero;
                        return false;
                    }

                    return true;
                }
            }
            else if (motion.X == 0 && motion.Y == 1)
            {
                int i = 0;
                for (i = 1; i <= amountOfTiles; i++)
                {
                    if (this.mapData[playerCoords.X, playerCoords.Y + i] != 1 || this.itemsData[playerCoords.X, playerCoords.Y + i] != null)
                        break;

                    bool shouldBreak = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                        if (TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center) == new Point(playerCoords.X, playerCoords.Y + i) && this.monsters[j].Alive)
                            shouldBreak = true;

                    if (shouldBreak)
                        break;
                }

                if (i - 1 != 0)
                {
                    targetPosition.Y = character.Position.Y + TheCavesOfKardun.TileHeight * (i - 1);

                    if (character != this.Player && (this.PredictPositions(character) || this.PredictPlayerPosition(character, false)))
                    {
                        targetPosition = character.Position;
                        return true;
                    }

                    if (this.PredictMonsterPosition(character) > 1)
                    {
                        targetPosition = Vector2.Zero;
                        return false;
                    }

                    return true;
                }
            }
            else if (motion.X == 0 && motion.Y == -1)
            {
                int i = 0;
                for (i = 1; i <= amountOfTiles; i++)
                {
                    if (this.mapData[playerCoords.X, playerCoords.Y - i] != 1 || this.itemsData[playerCoords.X, playerCoords.Y - i] != null)
                        break;

                    bool shouldBreak = false;
                    for (int j = 0; j < this.monsters.Count; j++)
                        if (TheCavesOfKardun.ConvertPositionToCell(this.monsters[j].Center) == new Point(playerCoords.X, playerCoords.Y - i) && this.monsters[j].Alive)
                            shouldBreak = true;

                    if (shouldBreak)
                        break;
                }

                if (i - 1 != 0)
                {
                    targetPosition.Y = character.Position.Y - TheCavesOfKardun.TileHeight * (i - 1);

                    if (character != this.Player && (this.PredictPositions(character) || this.PredictPlayerPosition(character, false)))
                    {
                        targetPosition = character.Position;
                        return true;
                    }

                    if (this.PredictMonsterPosition(character) > 1)
                    {
                        targetPosition = Vector2.Zero;
                        return false;
                    }

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
            {
                if (targetTile == TheCavesOfKardun.ConvertPositionToCell(this.monsters[i].Center) && this.monsters[i].Alive)
                {
                    monster = this.monsters[i];
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the player encounters an object.
        /// </summary>
        /// <param name="targetTile">The tile the player clicked on.</param>
        /// <param name="typeOfObject">The type of object the player clicked on.</param>
        /// <returns>Returns true if the player clicked on an item; otherwise false.</returns>
        public bool EncounterItem(Point targetTile, out Item item)
        {
            item = this.itemsData[targetTile.X, targetTile.Y];
            
            if (item != null)
                return true;
            
            return false;
        }

        /// <summary>
        /// Removes an item from a tile.
        /// </summary>
        /// <param name="targetTile">The tile to remove from.</param>
        public void RemoveItemFromTile(Point targetTile)
        {
            this.itemsData[targetTile.X, targetTile.Y] = null;
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

                    if (itemsData[x, y] != null && itemsData[x, y].OverworldTexture != null)
                        spriteBatch.Draw(this.itemsData[x, y].OverworldTexture,
                            new Rectangle(
                                (int)(x * TheCavesOfKardun.TileWidth - cameraPosition.X),
                                (int)(y * TheCavesOfKardun.TileHeight - cameraPosition.Y),
                                TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight),
                            Color.White);
                }
            }
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
            this.itemsData = new Item[mapDimensions.X, mapDimensions.Y];

            while (this.rooms.Count < amountOfRooms)
            {
                int width = random.Next(8, 16);
                int height = random.Next(8, 16);

                Room room = new Room(randomSeed,
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

            this.RoomSpawnIndex = random.Next(0, this.rooms.Count);
            OrderRoomsByDistanceToSpawn();

            this.BossRoomIndex = random.Next(this.rooms.Count - 3, this.rooms.Count);

            for (int i = 0; i < this.rooms.Count - 1; i++)
                MakeCorridor(this.rooms[i], this.rooms[i + 1]);

            MakeWalls();
            SpawnMonsters(20);
            SpawnItems();
            SpawnPotions(3);
            SpawnGold(80);

            SpawnBoss();
        }

        /// <summary>
        /// Spawns the boss on the map.
        /// </summary>
        private void SpawnBoss()
        {
            Room bossRoom = this.rooms[this.BossRoomIndex];
            Point centerTile = bossRoom.Center;

            this.monsters.Add(new Monster(this.boss.Name, this.boss.Texture, centerTile, 0, this.boss.Health, this.boss.MinDamage, this.boss.MaxDamage, 
                contentManager.Load<SpriteFont>("Fonts/combatFont")));
            this.monsters[this.monsters.Count - 1].Boss = true;
        }

        /// <summary>
        /// Spawns monsters on the map.
        /// </summary>
        private void SpawnMonsters(int maxAmount)
        {
            for (int i = 0; i < maxAmount; i++)
            {
                Room room;
                do
                {
                    room = RandomRoom();
                } while (room.MonsterInRoom > maxAmountOfMonstersPerRoom);

                Point floorTile = FreeRandomFloorTile(room, 30);

                if (floorTile != Point.Zero)
                {
                    int maxValue = Math.Min((int)Math.Floor(Math.Sqrt(Level.levelCount * Math.Pow(5, 2)) / 2), this.monsterData.Count);

                    int index = random.Next(0, maxValue);

                    this.monsters.Add(new Monster(this.monsterData[index].Name,
                        this.monsterData[index].Texture, floorTile, 500, this.monsterData[index].Health, 
                        this.monsterData[index].MinDamage, this.monsterData[index].MaxDamage, contentManager.Load<SpriteFont>("Fonts/combatFont")));

                    room.MonsterInRoom++;
                }
            }
        }

        /// <summary>
        /// Spawns object on the map.
        /// </summary>
        private void SpawnGold(int maxAmount)
        {
            for (int i = 0; i < maxAmount; i++)
            {
                Point floorTile = FreeRandomFloorTile(RandomRoom(), 30);

                if (floorTile != Point.Zero)
                    this.itemsData[floorTile.X, floorTile.Y] = this.goldItems[random.Next(0, this.goldItems.Count)];
            }
        }

        /// <summary>
        /// Spawn items on the map.
        /// </summary>
        private void SpawnItems()
        {
            Item item = null;
            int range = ((int)((this.rooms.Count - 3) - 1) / 2) + 1;

#region Random Sword

            if (Level.levelCount < this.swordItems.Count)
            {
                Item[] availableSwords = new Item[Level.levelCount];
                for (int i = 0; i < Level.levelCount; i++)
                    availableSwords[i] = this.swordItems[i];

                double rand = random.NextDouble() * 100;
                if (rand >= 75)
                {
                    // Spawn availabileSwords[Level.levelCount - 1]
                    item = availableSwords[Level.levelCount - 1];
                }
                else
                {
                    double percentage = 75.0 / Math.Max(Level.levelCount - 1, 1);

                    double totalPercentage = percentage;
                    int index = 0;
                    while (totalPercentage <= 75 && index < Level.levelCount)
                    {
                        if (rand < Math.Floor(totalPercentage))
                        {
                            // Spawn availabileSwords[index]
                            item = availableSwords[index];
                            break;
                        }

                        index++;
                        totalPercentage += percentage;
                    }
                }
            }
            else
            {
                double rand = random.NextDouble() * 100;

                int maxValue = Level.levelCount - 1;
                if (maxValue >= this.swordItems.Count)
                    maxValue = this.swordItems.Count - 1;

                double p = ((100 / Level.levelCount) * maxValue);

                if (rand >= p)
                {
                    // Spawn this.swordItems[maxValue]
                    item = this.swordItems[maxValue];
                }
                else
                {
                    double percentage = p / Math.Max(maxValue - 1, 1);

                    double totalPercentage = percentage;
                    int index = 0;
                    while (totalPercentage <= p && index < this.swordItems.Count)
                    {
                        if (rand < Math.Floor(totalPercentage))
                        {
                            // Spawn availabileSwords[index]
                            item = this.swordItems[index];
                            break;
                        }

                        index++;
                        totalPercentage += percentage;
                    }
                }
            }

            Point floorTile = FreeRandomFloorTile(this.rooms[random.Next(1, range)], 15);

            if (floorTile != Point.Zero)
                this.itemsData[floorTile.X, floorTile.Y] = item;

#endregion

            item = null;

#region Random Shield or Boots


            if (random.Next(0, 100) >= 70)
            {
                // Sköld
                if (Level.levelCount < this.shieldItems.Count)
                {
                    Item[] availableshields = new Item[Level.levelCount];
                    for (int i = 0; i < Level.levelCount; i++)
                        availableshields[i] = this.shieldItems[i];

                    double rand = random.NextDouble() * 100;
                    if (rand >= 75)
                    {
                        // Spawn availabileshields[Level.levelCount - 1]
                        item = availableshields[Level.levelCount - 1];
                    }
                    else
                    {
                        double percentage = 75.0 / Math.Max(Level.levelCount - 1, 1);

                        double totalPercentage = percentage;
                        int index = 0;
                        while (totalPercentage <= 75 && index < Level.levelCount)
                        {
                            if (rand < Math.Floor(totalPercentage))
                            {
                                // Spawn availabileshields[index]
                                item = availableshields[index];
                                break;
                            }

                            index++;
                            totalPercentage += percentage;
                        }
                    }
                }
                else
                {
                    double rand = random.NextDouble() * 100;

                    int maxValue = Level.levelCount - 1;
                    if (maxValue >= this.shieldItems.Count)
                        maxValue = this.shieldItems.Count - 1;

                    double p = ((100 / Level.levelCount) * maxValue);

                    if (rand >= p)
                    {
                        // Spawn this.shieldItems[maxValue]
                        item = this.shieldItems[maxValue];
                    }
                    else
                    {
                        double percentage = p / Math.Max(maxValue - 1, 1);

                        double totalPercentage = percentage;
                        int index = 0;
                        while (totalPercentage <= p && index < this.shieldItems.Count)
                        {
                            if (rand < Math.Floor(totalPercentage))
                            {
                                // Spawn availabileshields[index]
                                item = this.shieldItems[index];
                                break;
                            }

                            index++;
                            totalPercentage += percentage;
                        }
                    }
                }
            }
            else
            {
                // Boots
                if (Level.levelCount < this.bootsItems.Count)
                {
                    Item[] availableboots = new Item[Level.levelCount];
                    for (int i = 0; i < Level.levelCount; i++)
                        availableboots[i] = this.bootsItems[i];

                    double rand = random.NextDouble() * 100;
                    if (rand >= 75)
                    {
                        // Spawn availabileboots[Level.levelCount - 1]
                        item = availableboots[Level.levelCount - 1];
                    }
                    else
                    {
                        double percentage = 75.0 / Math.Max(Level.levelCount - 1, 1);

                        double totalPercentage = percentage;
                        int index = 0;
                        while (totalPercentage <= 75 && index < Level.levelCount)
                        {
                            if (rand < Math.Floor(totalPercentage))
                            {
                                // Spawn availabileboots[index]
                                item = availableboots[index];
                                break;
                            }

                            index++;
                            totalPercentage += percentage;
                        }
                    }
                }
                else
                {
                    double rand = random.NextDouble() * 100;

                    int maxValue = Level.levelCount - 1;
                    if (maxValue >= this.bootsItems.Count)
                        maxValue = this.bootsItems.Count - 1;

                    double p = ((100 / Level.levelCount) * maxValue);

                    if (rand >= p)
                    {
                        // Spawn this.bootsItems[maxValue]
                        item = this.bootsItems[maxValue];
                    }
                    else
                    {
                        double percentage = p / Math.Max(maxValue - 1, 1);

                        double totalPercentage = percentage;
                        int index = 0;
                        while (totalPercentage <= p && index < this.bootsItems.Count)
                        {
                            if (rand < Math.Floor(totalPercentage))
                            {
                                // Spawn availabileboots[index]
                                item = this.bootsItems[index];
                                break;
                            }

                            index++;
                            totalPercentage += percentage;
                        }
                    }
                }
            }

            floorTile = FreeRandomFloorTile(this.rooms[random.Next(range, this.rooms.Count - 3)], 15);

            if (floorTile != Point.Zero)
                this.itemsData[floorTile.X, floorTile.Y] = item;
#endregion
        }

        /// <summary>
        /// Spawns potions on the map.
        /// </summary>
        /// <param name="maxAmount"></param>
        private void SpawnPotions(int maxAmount)
        {
            for (int i = 0; i < maxAmount; i++)
            {
                Point floorTile = FreeRandomFloorTile(RandomRoom(), 30);

                if (floorTile != Point.Zero)
                    this.itemsData[floorTile.X, floorTile.Y] = this.potionItems[random.Next(0, this.potionItems.Count)];
            }
        }

        /// <summary>
        /// Gets a random room that's not the boss room or the player spawn room.
        /// </summary>
        /// <returns>Returns a random room.</returns>
        private Room RandomRoom()
        {
            int r = 0;
            do
            {
                r = random.Next(1, this.rooms.Count);
            } while (r == this.BossRoomIndex);

            return this.rooms[r];
        }

        /// <summary>
        /// Gets a free random floor tile in a given room.
        /// </summary>
        /// <param name="room">The room to random a floor tile from.</param>
        /// <returns>Returns the free floor tile.</returns>
        private Point FreeRandomFloorTile(Room room, int maxFailes)
        {
            Point floorTile = Point.Zero;
            int tries = 0;
            bool positionAlreadyOccupied = false;
            do
            {
                if (tries == maxFailes)
                    break;

                floorTile = room.RandomFloorTile;
                tries++;

                positionAlreadyOccupied = false;
                for (int i = 0; i < this.monsters.Count; i++)
                {
                    if (floorTile == TheCavesOfKardun.ConvertPositionToCell(this.monsters[i].Center))
                    {
                        positionAlreadyOccupied = true;
                        break;
                    }
                }
            } while (floorTile == Point.Zero || this.itemsData[floorTile.X, floorTile.Y] != null || positionAlreadyOccupied);

            return floorTile;
        }

        /// <summary>
        /// Creates a room.
        /// </summary>
        /// <param name="room">The room to create.</param>
        private void MakeRoom(Room room)
        {
            for (int x = 0; x < room.Width; x++)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    if (x == 0 || x == room.Width - 1 || y == 0 || y == room.Height - 1)
                        mapData[room.Left + x, room.Top + y] = 2;
                    else
                        mapData[room.Left + x, room.Top + y] = 1;
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

        /// <summary>
        /// Predicts how many monsters will have the same target position.
        /// </summary>
        /// <param name="character">The character to compare with.</param>
        /// <returns>Returns the amount of monsters with the same target position.</returns>
        private int PredictMonsterPosition(Character character)
        {
            Vector2 characterTargetPos = character.Position;

            if (character.TargetPosition == Vector2.Zero)
                return 0;

            if (character.TargetPosition.X == 0)
                characterTargetPos.Y = character.TargetPosition.Y;
            else
                characterTargetPos.X = character.TargetPosition.X;

            int counter = 0;
            for (int i = 0; i < this.monsters.Count; i++)
            {
                if (!this.monsters[i].Alive)
                    continue;

                Vector2 monsterTargetPos = monsters[i].Position;

                if (monsters[i].TargetPosition.X == 0)
                    monsterTargetPos.Y = monsters[i].TargetPosition.Y;
                else
                    monsterTargetPos.X = monsters[i].TargetPosition.X;

                if (characterTargetPos == monsterTargetPos)
                    counter++;
            }

            return counter;
        }

        /// <summary>
        /// Predicts the players position and compares it with the given character.
        /// </summary>
        /// <param name="character">The character to compare with.</param>
        /// <returns>Returns true if the player's target position equals to the characters target position.</returns>
        private bool PredictPositions(Character character)
        {
            Vector2 characterTargetPos = character.Position;

            if (character.TargetPosition == Vector2.Zero)
                return false;

            if (character.TargetPosition.X == 0)
                characterTargetPos.Y = character.TargetPosition.Y;
            else
                characterTargetPos.X = character.TargetPosition.X;

            Vector2 playerTargetPos = this.Player.Position;
            if (this.Player.TargetPosition.X == 0)
                playerTargetPos.Y = this.Player.TargetPosition.Y;
            else
                playerTargetPos.X = this.Player.TargetPosition.X;

            return characterTargetPos == playerTargetPos;
        }

        /// <summary>
        /// Predicts the players position and compares it with the given character's current position.
        /// </summary>
        /// <param name="character">The character to compare with.</param>
        /// <returns>Returns true if the players target position is equally to the character's current position.</returns>
        private bool PredictPlayerPosition(Character character, bool xaxis)
        {
            Vector2 playerTargetPos = this.Player.Position;
            if (this.Player.TargetPosition.X == 0)
                playerTargetPos.Y = this.Player.TargetPosition.Y;
            else
                playerTargetPos.X = this.Player.TargetPosition.X;

            if (xaxis && playerTargetPos.X == character.Position.X)
                return true;
            else if (!xaxis && playerTargetPos.Y == character.Position.Y)
                return true;

            return false;
        }

        #endregion
    }
}