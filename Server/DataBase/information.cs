using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class User
    {
        public string id;
        public string password;

        public User()
        {

        }

        public User(string id, string password)
        {
            this.id = id;
            this.password = password;
        }
    }
    class Stage
    {
        public string stagecode;
        public string stagename;
        public string stagetema;

        public Stage()
        {

        }
        public Stage(string stagecode, string stagename, string stagetema)
        {
            this.stagecode = stagecode;
            this.stagename = stagename;
            this.stagetema = stagetema;
        }
    }
    class Ranking
    {
        public int rank;
        public string stagecode;
        public string userid;
        public string cleartime;

        public Ranking()
        {

        }

        public Ranking(int rank, string stagecode, string userid, string cleartime)
        {
            this.rank = rank;
            this.stagecode = stagecode;
            this.userid = userid;
            this.cleartime = cleartime;
        }

    }
}
