using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Digger
{
    enum Items
    {
        Player,
        Sack,
        Gold,
        Monster,
        MadMonster,
        Terrain,
        Bomb,
        Empty,
    }
    public class MapGenerator
    {
        private static Dictionary<Items, char> ItemsDict = new Dictionary<Items, char>()
        {
            {Items.Player, 'P'},
            {Items.Sack, 'S'},
            {Items.Gold, 'G'},
            {Items.Monster, 'M'},
            {Items.MadMonster, 'N'},
            {Items.Terrain, 'T'},
            {Items.Bomb, 'B'},
            {Items.Empty, ' '}
        };

        private Random rnd = new Random();
        private int _height;
        private int _width;
        private Items[,] _map;
        public string Map
        {
            get => ConvertMapToString(_map);
        }

        public MapGenerator(int height, int width, int monsterCount, int sackCount, int goldCount, int madMonsterCount, int terrainCount, int bombCount)
        {
            _height = height;
            _width = width;
            Generate(monsterCount, sackCount, goldCount, madMonsterCount, terrainCount, bombCount);
        }
        
        public void Generate(int monsterLimit, int sackLimit, int goldLimit, int madMonsterLimit, int terrainLimit, int bombLimit)
        {
            CreateMap();
            PutNPointsOnMap(monsterLimit, Items.Monster);
            PutNPointsOnMap(sackLimit, Items.Sack);
            PutNPointsOnMap(goldLimit, Items.Gold);
            PutNPointsOnMap(terrainLimit, Items.Terrain);
            PutNPointsOnMap(bombLimit, Items.Bomb);
            PutNPointsOnMap(madMonsterLimit, Items.MadMonster);
            PutNPointsOnMap(1, Items.Player);
        }

        private void CreateMap()
        {
            _map = new Items[_height, _width];
            for (var i = 0; i < _height; i++)
            {
                for (var j = 0; j < _width; j++)
                {
                    _map[i, j] = Items.Empty;
                }
            }
        }
        private void PutNPointsOnMap(int count, Items item)
        {
            for (var i = 0; i < count; i++)
            {
                var randomPoint = GenerateRandomPoint();
                _map[randomPoint.X, randomPoint.Y] = item;
            }
        }

        private Point GenerateRandomPoint() => 
            new Point(rnd.Next(0, _height), rnd.Next(0, _width ));

        private string ConvertMapToString(Items[,] map)
        {
            var result = new StringBuilder();
            var width = map.GetLength(0);
            var heigth = map.GetLength(1);
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < heigth; j++)
                {
                    result.Append(ItemsDict[_map[i, j]]);
                }
                if(i != width - 1)
                    result.Append("\r\n");
            }

            return result.ToString();
        }

    }
}