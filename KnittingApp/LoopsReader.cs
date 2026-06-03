using System.Text;

namespace KnittingApp
{
    public class LoopsReader
    {
        public Guid UserId { get; set; }
        public Guid LoopReaderId { get; } 
        int currentIndex;

        Dictionary<string, string> colors;
        public Guid LoopMapId { get; }

        public LoopMap loopMap { get; set; }

        TimeSpan spentTime;

        public LoopsReader(Guid loopReaderId, int currentIndex, Guid loopMapId,  TimeSpan spentTime)
        {
            LoopReaderId = loopReaderId;
            this.currentIndex = currentIndex;
            LoopMapId = loopMapId;
            this.spentTime = spentTime;
            colors = new Dictionary<string, string>();
        }

        public LoopsReader(int startIndex, LoopMap loopMap)
        {
            LoopReaderId = Guid.NewGuid();
            LoopMapId = loopMap.LoopMapId;
            currentIndex = startIndex;
            this.loopMap = loopMap;
            colors = new Dictionary<string, string>();
            GetLoopMapColors();
        }

        public (List<string>, List<string>) GetCurrentString()
        {
            if (currentIndex < 0)
                return (new List<string> { "end" }, new List<string>());
           var currentString=(new List<string>(), new List<string>());
            var line = loopMap.loopMap[currentIndex];
            for(int i=0; i < line.Length; ++i)
            {
                if (line[i].type == SharedConstants.LoopType.none)
                    continue;
                int loopCount = 1;
                var type=line[i].type;
                var side=line[i].side;
                var color = line[i].Color;
                string stringType;
                switch (type)
                {
                    case SharedConstants.LoopType.loop:
                        stringType="петля";
                        break;
                    case SharedConstants.LoopType.increase:
                        stringType = "прибавка";
                        break;
                    case SharedConstants.LoopType.decrease:
                        stringType = "убавка";
                        break;
                    default:
                        stringType = "петля";
                        break;
                }

                string stringSide;
                switch (side)
                {
                    case SharedConstants.LoopSide.front:
                        stringSide = "лицевая";
                        break;
                    case SharedConstants.LoopSide.back:
                        stringSide = "изнаночная";
                        break;
                    default:
                        stringSide = "лицевая";
                        break;
                }

                while (line[i].type == line[i + 1].type &&
                    line[i].side == line[i + 1].side &&
                    line[i].Color == line[i + 1].Color)
                {
                    ++loopCount;
                    ++i;
                }
                currentString.Item1.Add(loopCount +" "+ stringSide+" " + stringType +" "+ colors[color]);
                currentString.Item2.Add(color);
                --currentIndex;
            }
            return currentString;
        }

        public int GetCurrentIndex()
        {
            return currentIndex;
        }
        public int GetProgress()
        {
            return loopMap.loopMap.Length/currentIndex;
        }

        public void SetSpentTime(TimeSpan time)
        {
            spentTime = time;
        }

        public TimeSpan GetSpentTime()
        {
            return spentTime;
        }

        void GetLoopMapColors()
        {
            int colorCount = 1;
            for (int i = 0; i < loopMap.loopMap.Length; ++i)
            {
                for (int j = 0; j < loopMap.loopMap[i].Length; ++j)
                {
                    if(loopMap.loopMap[i][j]!=null&& loopMap.loopMap[i][j].GetType()!=SharedConstants.LoopType.none)
                    {
                        var currentColor = loopMap.loopMap[i][j].Color;
                        if (!colors.ContainsKey(currentColor))
                        {
                            colors.Add(currentColor, "Цвет " + colorCount);
                            ++colorCount;
                        }
                    }

                }
            }
        }

        string GetEnding(int number)
        {
            string ending;
            if(number==1)
                    ending = "ля";
            else if(number>1&&number<5)
                ending = "ли";
            else 
                ending = "ель";
            return ending;
        }
    }
}
