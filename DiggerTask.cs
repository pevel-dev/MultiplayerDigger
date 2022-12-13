using System.Windows;
using System.Windows.Forms;

namespace Digger
{
    class Terrain : ICreature
    {
        public string GetImageFileName() => "Terrain.png";

        public int GetDrawingPriority() => 0;

        public CreatureCommand Act(int x, int y) => new CreatureCommand();

        public bool DeadInConflict(ICreature conflictedObject) =>
            conflictedObject is Player || conflictedObject is Sack;
    }

    class Player : ICreature
    {
        public string GetImageFileName() => "Digger.png";

        public int GetDrawingPriority() => 1;

        public CreatureCommand Act(int x, int y)
        {
            switch (Game.KeyPressed)
            {
                case Keys.Left when CanPlayerGoToPoint(x - 1, y):
                    return new CreatureCommand { DeltaX = -1 };
                case Keys.Right when CanPlayerGoToPoint(x + 1, y):
                    return new CreatureCommand { DeltaX = 1 };
                case Keys.Up when CanPlayerGoToPoint(x, y - 1):
                    return new CreatureCommand { DeltaY = -1 };
                case Keys.Down when CanPlayerGoToPoint(x, y + 1):
                    return new CreatureCommand { DeltaY = 1 };
                default:
                    return new CreatureCommand();
            }
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            switch (conflictedObject)
            {
                case Terrain _:
                    conflictedObject.DeadInConflict((ICreature)this);
                    break;
                case Sack _:
                    Game.IsOver = true;
                    return true;
            }

            return false;
        }

        private bool CanPlayerGoToPoint(int x, int y)
        {
            return x >= 0 && y >= 0
                          && x < Game.MapWidth && y < Game.MapHeight
                          && !(Game.Map[x, y] is Sack);
        }
    }

    internal class Sack : ICreature
    {
        private int _dropTime = 0;

        public string GetImageFileName() => "Sack.png";

        public int GetDrawingPriority() => 2;

        public CreatureCommand Act(int x, int y)
        {
            if (CanDown(x, y))
            {
                _dropTime++;
                return new CreatureCommand { DeltaY = 1 };
            }

            if (_dropTime > 1)
                return new CreatureCommand { TransformTo = new Gold() };
            _dropTime = 0;
            return new CreatureCommand();
        }

        public bool CanDown(int x, int y) =>
            y + 1 < Game.MapHeight && (Game.Map[x, y + 1] is null || (_dropTime > 1 && Game.Map[x, y + 1] is Player));


        public bool DeadInConflict(ICreature conflictedObject) => false;
    }

    class Gold : ICreature
    {
        public string GetImageFileName() => "Gold.png";

        public int GetDrawingPriority() => 3;

        public CreatureCommand Act(int x, int y) => new CreatureCommand();

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Player)
            {
                Game.Scores += 10;
                return true;
            }

            return false;
        }
    }
}