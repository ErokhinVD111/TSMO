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

        /// <summary>
        /// Число каналов в системе
        /// </summary>
        public readonly int CountChannels;


        /// <summary>
        /// Число каналов, которые могут обслуживать одновременно 
        /// </summary>
        public readonly int CountParalelChannels;

        #endregion

        /// <summary>
        /// Список каналов
        /// </summary>
        public List<Channel> Channels;


        private static ServiceSystem systemInstance;


        private ServiceSystem(N n, int paralelChannels)
        {
            CountChannels = (int)n;
            CountParalelChannels = paralelChannels;
            Channels = new(CountChannels);
        }

        public static ServiceSystem GetServiceSystem(N n, int paralelChannels)
        {
            systemInstance = new ServiceSystem(n, paralelChannels);
            return systemInstance;
        }


        /// <summary>
        /// Метод для установки начальных значений для системы
        /// </summary>
        public void SetDefaultChannels()
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


        /// <summary>
        /// Метод для проверки состояния каналов
        /// </summary>
        /// <returns></returns>
        public int CheckCountFreeChannels()
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
