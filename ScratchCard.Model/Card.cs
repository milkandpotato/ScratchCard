using System;
using System.Collections.Generic;

namespace ScratchCard.Model
{
    public class Card
    {
        //长度
        private int length;
        //宽度
        private int width;
        //奖项数量
        private int awardTypes;
        //奖项
        private List<Award> awards = new List<Award>();


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

        public int AwardTypes
        {
            get { return awardTypes; }
            set { awardTypes = value; }
        }

        public List<Award> Awards
        {
            get { return awards; }
            set { awards = value; }
        }

        public Card()
        {

        }
    }
}
