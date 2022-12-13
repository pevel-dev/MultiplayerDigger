using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Digger
{
    public class GameState
    {
        public const int ElementSize = 32;
        public List<CreatureAnimation> Animations = new List<CreatureAnimation>();
        private Game gameSession;

        public GameState(Game game)
        {
            gameSession = game;
        }
        
        public void BeginAct()
        {
            Animations.Clear();
            for (var x = 0; x < gameSession.MapWidth; x++)
            for (var y = 0; y < gameSession.MapHeight; y++)
            {
                var creature = gameSession.Map[x, y];
                if (creature == null) continue;
                var command = creature.Act(x, y, gameSession);

                if (x + command.DeltaX < 0 || x + command.DeltaX >= gameSession.MapWidth || y + command.DeltaY < 0 ||
                    y + command.DeltaY >= gameSession.MapHeight)
                    throw new Exception($"The object {creature.GetType()} falls out of the game field");

                Animations.Add(
                    new CreatureAnimation
                    {
                        Command = command,
                        Creature = creature,
                        Location = new Point(x * ElementSize, y * ElementSize),
                        TargetLogicalLocation = new Point(x + command.DeltaX, y + command.DeltaY)
                    });
            }

            Animations = Animations.OrderByDescending(z => z.Creature.GetDrawingPriority()).ToList();
        }

        public void EndAct()
        {
            var creaturesPerLocation = GetCandidatesPerLocation();
            for (var x = 0; x < gameSession.MapWidth; x++)
            for (var y = 0; y < gameSession.MapHeight; y++)
                gameSession.Map[x, y] = SelectWinnerCandidatePerLocation(creaturesPerLocation, x, y, gameSession);
        }

        private static ICreature SelectWinnerCandidatePerLocation(List<ICreature>[,] creatures, int x, int y, Game gameSession)
        {
            var candidates = creatures[x, y];
            var aliveCandidates = candidates.ToList();
            foreach (var candidate in candidates)
            foreach (var rival in candidates)
                if (rival != candidate && candidate.DeadInConflict(rival, gameSession))
                    aliveCandidates.Remove(candidate);
            if (aliveCandidates.Count > 1)
                throw new Exception(
                    $"Creatures {aliveCandidates[0].GetType().Name} and {aliveCandidates[1].GetType().Name} claimed the same map cell");

            return aliveCandidates.FirstOrDefault();
        }

        private List<ICreature>[,] GetCandidatesPerLocation()
        {
            var creatures = new List<ICreature>[gameSession.MapWidth, gameSession.MapHeight];
            for (var x = 0; x < gameSession.MapWidth; x++)
            for (var y = 0; y < gameSession.MapHeight; y++)
                creatures[x, y] = new List<ICreature>();
            foreach (var e in Animations)
            {
                var x = e.TargetLogicalLocation.X;
                var y = e.TargetLogicalLocation.Y;
                var nextCreature = e.Command.TransformTo ?? e.Creature;
                creatures[x, y].Add(nextCreature);
            }

            return creatures;
        }
    }
}