using System;
using System.Windows.Forms;

namespace Digger
{
    public class Game
    {
        public ICreature[,] Map;
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

        public bool IsOver
        {
            get => _isOver;
            set
            {
                if (_isOver)
                    throw new Exception("Игра уже закончена. Создайте новую игру чтобы играть дальше");
                _isOver = value;
            }
        }

        public Keys KeyPressed;

        public int MapWidth
        {
            get => Map.GetLength(0);
        }

        public int MapHeight
        {
            get => Map.GetLength(1);
        }
        
        private int _scores = 0;
        private bool _isOver = false;

        public Game()
        {
            var generator = new MapGenerator(16, 15, 1,24,10,0,100,0);
            var map = generator.Map;
            Map = CreatureMapCreator.CreateMap(map);
        }
    }
}