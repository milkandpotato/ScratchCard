using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchCard.Model
{
    public class AwardPosition
    {
        //奖品x轴位置
        private int position_x;
        //奖品y轴位置
        private int position_y;

        public int PositionX { get { return position_x; } set { position_x = value; } }
        public int PositionY { get { return position_y; } set { position_y = value; } }

        public AwardPosition(int max_x, int max_y)
        {
            Random random = new Random();

            this.position_x = random.Next(max_x);
            this.position_y = random.Next(max_y);
        }
    }
}
