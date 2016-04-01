﻿using Match3.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.World
{
    public class Swap
    {
        public static List<Swap> FindSwaps(BlockField field, int matchLength)
        {
            var swaps = new List<Swap>();

            for (int y = 0; y < field.Height; ++y)
            {
                for (int x = 0; x < field.Width; ++x)
                {
                    if (!field[y, x].Usable())
                        continue;

                    if (x < field.Width - 1 && field[y, x + 1].Usable())
                    {
                        field.Swap(x, y, x + 1, y);

                        if (Chain.FindMaxChainLength(field, field[y, x]) >= matchLength ||
                            Chain.FindMaxChainLength(field, field[y, x + 1]) >= matchLength)
                        {
                            swaps.Add(new Swap(field[y, x], field[y, x + 1]));
                        }

                        field.Swap(x, y, x + 1, y);
                    }

                    if (y < field.Height - 1 && field[y + 1, x].Usable())
                    {
                        field.Swap(x, y, x, y + 1);

                        if (Chain.FindMaxChainLength(field, field[y, x]) >= matchLength ||
                            Chain.FindMaxChainLength(field, field[y + 1, x]) >= matchLength)
                        {
                            swaps.Add(new Swap(field[y, x], field[y + 1, x]));
                        }

                        field.Swap(x, y, x, y + 1);
                    }
                }
            }

            return swaps;
        }

        public Block From
        { get; private set; }
        public Block To
        { get; private set; }

        public bool CanSwap
        { get; private set; }

        public Swap(Block fromBlock, Block toBlock)
        {
            #region Debug
#if DEBUG
            if (fromBlock == null)
                throw new ArgumentNullException(nameof(fromBlock));
            if (toBlock == null)
                throw new ArgumentNullException(nameof(toBlock));
#endif
            #endregion

            From = fromBlock;
            To = toBlock;

            CanSwap = CheckSwap();
        }

        public Swap()
        {
        }

        public void Make(Action<Swap> swappedCallback = null)
        {
            if (!CanSwap)
                throw new InvalidOperationException("Can not swap this blocks.");

            Action<Block, Point, Point> onMoved = (block, originPos, newPos) =>
            {
                if (From.IsMoving || To.IsMoving)
                    return;

                if (swappedCallback != null)
                    swappedCallback(this);
            };

            From.MoveTo(To, movedCallback: onMoved);
            To.MoveTo(From, movedCallback: onMoved);
        }

        private bool CheckSwap()
        {
            if (From.X == To.X)
                return Math.Abs(From.Y - To.Y) == 1;

            if (From.Y == To.Y)
                return Math.Abs(From.X - To.X) == 1;

            return false;
        }
    }
}