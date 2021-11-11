using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class user
    {
        public string id;
        public string password;

        public user()
        {

        }

        public user(string id, string password)
        {
            this.id = id;
            this.password = password;
        }
    }
    class stage
    {
        public string stagecode;
        public string stagename;
        public string stagetema;

        public stage()
        {

        }
        public stage(string stagecode, string stagename, string stagetema)
        {
            this.stagecode = stagecode;
            this.stagename = stagename;
            this.stagetema = stagetema;
        }
    }
    class ranking
    {
        public int rank;
        public string stagecode;
        public string userid;
        public string cleartime;

        public ranking()
        {

        }

        public ranking(int rank, string stagecode, string userid, string cleartime)
        {
            this.rank = rank;
            this.stagecode = stagecode;
            this.userid = userid;
            this.cleartime = cleartime;
        }

    }
}
