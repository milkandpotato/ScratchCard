using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchCard.model
{
    public class Award
    {
        //奖品名称
        private string name;
        //奖品数量
        private int number;
        //卡片长度
        private int length;
        //卡片宽度
        private int width;

        //奖品位置
        private List<AwardPosition> awardPositions = new List<AwardPosition>();

        public string Name { get { return name; } set { name = value; } }
        public int Number
        {
            get { return number; }
            set
            {
                number = value;
                GeneratePosition();
            }
        }

        public List<AwardPosition> AwardPositions { get { return awardPositions; } }
        
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public Award() { }

        private void GeneratePosition()
        {
            for (int i = 0; i < this.number; i++)
            {
                AwardPosition position = new AwardPosition(this.Length, this.Width);

                awardPositions.Add(position);
            }
        }
    }
}
