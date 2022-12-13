using System;
using System.Windows.Forms;

namespace Digger
{
    public class Game
    {
        public ICreature[,] Map;
        private int _scores = 0;
        public int Scores
        {
            get => _scores;
            set
            {
                if (_scores < 0)
                {
                    throw new ArgumentException("Очки должны быть неотрицательные");
                }

                _scores = value;
            }
        }

        public bool IsOver;

        public Keys KeyPressed;
        public int MapWidth => Map.GetLength(0);
        public int MapHeight => Map.GetLength(1);

        public void CreateMap()
        {
            var generator = new MapGenerator(16, 15, 1,24,10,1,100,0);
            var map = generator.Map;
            Map = CreatureMapCreator.CreateMap(map);
        }
    }
}