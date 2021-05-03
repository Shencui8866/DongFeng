using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Josing
{
    public class LogPools
    {
        public List<string> Pools { get; private set; } = new List<string>();
        public int MaxCount { get; private set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="max">log最大容量</param>
        public LogPools(int max) { MaxCount = max; }
        public string Add(string log)
        {
            Pools.Add(log);
            if (Pools.Count > MaxCount)
                Pools.RemoveAt(0);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Pools.Count; i++)
                builder.AppendLine(Pools[i]);
            return builder.ToString();
        }
    }
}

