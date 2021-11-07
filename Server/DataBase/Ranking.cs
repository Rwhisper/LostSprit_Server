using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Ranking
    {
        public int Rank { get; set; }
        public string StageCode { get; set; }
        public string Id { get; set; }
        public string ClearTime { get; set; }
        public Ranking()
        {

        }
        public Ranking(int rank, string stageCode, string id, string clearTime)
        {
            this.Rank = rank;
            this.StageCode = stageCode;
            this.Id = id;
            this.ClearTime = clearTime;
        }

        

    }
}
