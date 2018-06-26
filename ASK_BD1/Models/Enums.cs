using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASK_BD1.Models
{
    public class Enums
    {
        public enum VOTE_STATES
        {
            NONE = 0,
            UP = 1,
            DOWN = 2,
            NOT_SIGNED_IN = 4
        };
    }
}