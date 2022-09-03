using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asset
{
    public class BuildTimer 
    {
        public long StartTime;
        public long EndTime;
        public TimeSpan Duration
        {
            get 
            { 
                if(EndTime> StartTime)
                {
                    return TimeSpan.FromTicks(EndTime - StartTime);
                }
                else
                {
                    return TimeSpan.FromTicks(DateTime.Now.Ticks - StartTime);
                }
            }
        }
    }
}
