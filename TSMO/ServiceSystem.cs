using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    internal class ServiceSystem
    {
        #region ChannelsCharacteristics

        private readonly int CountChannels;

        public readonly int CountParalelChannels;

        #endregion

        public List<Channel> Channels;

        private static ServiceSystem systemInstance;

        private ServiceSystem(N n, int paralelChannels)
        {
            CountChannels = (int)n;
            CountParalelChannels = paralelChannels;
            Channels = new List<Channel>();
        }

        public static ServiceSystem GetServiceSystem(N n, int paralelChannels)
        {
            systemInstance = new ServiceSystem(n, paralelChannels);
            return systemInstance;
        }

        public void SetChannels()
        {
            for (int i = 0; i < CountChannels; i++)
            {
                Channels.Add(new Channel() 
                { 
                    state = false, 
                    number = i + 1, 
                    requestComplete = 0 
                });
            }
        }

        public int CheckStateChannels()
        {
            int countFreeChannel = 0;
            foreach (Channel channel in Channels)
            {
                if (channel.state == false)
                {
                    countFreeChannel++;
                }
            }
            return countFreeChannel;
        }
        


    }
}
