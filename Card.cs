using System;
using System.Collections.Generic;

namespace ScratchCard
{
    public class Card
    {
        public Card()
        {

        }

        //长度
        private int length;
        //宽度
        private int width;
        //奖项
        private Dictionary<string,int> awards = new Dictionary<string,int>();


        public int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public Dictionary<string, int> Awards
        {
            get { return this.awards; }
        }


        public override string ToString()
        {

            return $"刮刮卡的长度：{this.length},宽度:{this.width}";
        }
    }
}
