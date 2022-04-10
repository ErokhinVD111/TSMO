using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    //большое количетво полей (проще хранить в структуре)
    //Нужно больше классов 
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


        /// <summary>
        /// Число выполненных заявок
        /// </summary>
        public int CountRequestComplete { get; set; }


        /// <summary>
        /// Число заявок
        /// </summary>
        public int CountRequest { get; set; }


        /// <summary>
        /// Число отклоненных заявок
        /// </summary>
        public int CountRequestDenied { get; set; }



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
            CountRequestComplete = 0;
            CountRequest = 0;
            CountRequestDenied = 0;
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
                    IsActive = false,
                    Number = i + 1,
                    IndexRequest = 0
                });
            }
        }


        /// <summary>
        /// Метод начала обработки свободными каналами пришедшей заявки
        /// </summary>
        /// <param name="timeComingRequest"></param>
        /// <param name="timeBusyChannel"></param>
        /// <param name="indexRequest"></param>
        public void StartOfChannelsProcessing(double timeComingRequest, double timeBusyChannel, int indexRequest)
        {
            //Получаем количество свободных каналов в зависимости от условия (k>=l, k<l, k=0), где k кол-во свободных каналов
            int countFreeChannels = GetCountFreeChannels();

            //Ставим заявку на обслуживание в свободные каналы
            Channels.Where(channel => !channel.IsActive).ToList().Take(countFreeChannels).
                ToList().ForEach(channel =>
                {
                    channel.IsActive = true;
                    channel.IndexRequest = indexRequest;
                    channel.timeComingRequest.Add(timeComingRequest);
                    channel.timeBusyChannel.Add(timeBusyChannel);

                });

            if (countFreeChannels > 0)
            {
                CountRequestComplete++;
            }
            else
            {
                CountRequestDenied++;
            }
            CountRequest++;
        }




        /// <summary>
        /// Метод для поиска свободных каналов
        /// </summary>
        /// <returns></returns>
        public int GetCountFreeChannels()
        {
            if (Channels.Where(channel => !channel.IsActive).Count() >= CountParalelChannels)
            {
                return CountParalelChannels;
            }
            else if (Channels.Where(channel => !channel.IsActive).Count() > 0)
            {
                return Channels.Where(channel => !channel.IsActive).Count();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Метод для освобождения каналов 
        /// </summary>
        /// <param name="timeComingRequest"></param>
        public void FreeChannels(double timeComingRequest)
        {
            //Освобождаем каналы, у которых вышло время обслуживания
            Channels.Where(channel => channel.IsActive).ToList().
                Where(channel => timeComingRequest >= channel.timeComingRequest.Last() + channel.timeBusyChannel.Last()).
                ToList().ForEach(i => i.IsActive = false);
            
            //Делаем перераспределение при условии, что оно нужно
            StartSupport(timeComingRequest);
            
        }


        /// <summary>
        /// Метод для перераспределения каналов (оказания взаимопомощи)
        /// </summary>
        public void StartSupport(double timeComingRequest)
        {

        }

    }
}
