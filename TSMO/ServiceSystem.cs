using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    //большое количетво полей (проще хранить в структуре)
    internal class ServiceSystem
    {
        #region ChannelsCharacteristics

        /// <summary>
        /// Число каналов в системе
        /// </summary>
        private int numberOfChannels;


        /// <summary>
        /// Число каналов, которые могут обслуживать одновременно 
        /// </summary>
        private int numberOfParalelChannels;

        #endregion

        #region ServiceSystemCharacteristics

        /// <summary>
        /// Коллекция, содержащая информацию о том, сколько каналов обслуживают одну заявку
        /// </summary>
        private Dictionary<int, int> numOfChReqPairs;

        /// <summary>
        /// Коллекция каналов, обрабатывающих заявку
        /// </summary>
        private List<Channel> channels;


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


        /// <summary>
        /// Число загруженных каналов на каждом цикле
        /// </summary>
        public List<int> CountBusyChPerIter { get; set; }

        #endregion

        public ServiceSystem(NumberOfChannels numberOfChannels, int numberOfParalelChannels)
        {
            this.numberOfChannels = (int)numberOfChannels;

            this.numberOfParalelChannels = numberOfParalelChannels;

            channels = new(this.numberOfChannels);

            numOfChReqPairs = new();

            SetDefaultChannels();

            CountBusyChPerIter = new();

            CountRequest = 0;

            CountRequestComplete = 0;

            CountRequestDenied = 0;
        }


        /// <summary>
        /// Метод для установки начальных значений каналам
        /// </summary>
        private void SetDefaultChannels()
        {

            for (int i = 0; i < numberOfChannels; i++)
            {
                channels.Add(new Channel()
                {
                    IsActive = false,
                    IndexRequest = 0
                });
            }
        }


        /// <summary>
        /// Метод начала обработки свободными каналами пришедшей заявки
        /// </summary>
        /// <param name="timeComingRequest">Время прихода заявки</param>
        /// <param name="timeBusyChannel">Время загруженности канала</param>
        /// <param name="indexRequest">Индекс пришедшеий заявки</param>
        public void AddRequest(double timeComingRequest, double timeBusyChannel, int indexRequest)
        {
            UpdateChannels(timeComingRequest);

            //Получаем количество свободных каналов в зависимости от условия (k>=l, k<l, k=0), где k кол-во свободных каналов
            int countFreeChannels = GetCountFreeChannels();


            //Ставим заявку на обслуживание в свободные каналы
            channels.Where(channel => !channel.IsActive).ToList().Take(countFreeChannels).
                ToList().ForEach(channel =>
                {
                    channel.IsActive = true;
                    channel.IndexRequest = indexRequest;
                    channel.TimeComingRequest = timeComingRequest;
                    channel.TimeBusyChannel = timeBusyChannel;

                });

            if (countFreeChannels > 0)
            {
                CountRequestComplete++;

                numOfChReqPairs.Add(indexRequest, countFreeChannels);
            }
            else
            {
                CountRequestDenied++;
            }
            CountRequest++;
        }




        /// <summary>
        /// Метод для поиска кол-ва свободных каналов
        /// </summary>
        /// <returns></returns>
        public int GetCountFreeChannels()
        {
            int countFreeChannels = channels.Where(channel => !channel.IsActive).Count();

            if (countFreeChannels >= numberOfParalelChannels)
            {
                return numberOfParalelChannels;
            }
            else if (countFreeChannels > 0)
            {
                return countFreeChannels;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Метод для поиска кол-ва занятых каналов
        /// </summary>
        /// <returns></returns>
        public int GetCountBusyChannels()
        {
            return channels.Where(channel => channel.IsActive).Count();
        }


        /// <summary>
        /// Метод для освобождения каналов 
        /// </summary>
        /// <param name="timeComingRequest">Время прихода новой заявки</param>
        public void UpdateChannels(double timeComingRequest)
        {
            //Освобождаем каналы, у которых вышло время обслуживания
            channels.Where(channel => channel.IsActive).ToList().
                Where(channel => timeComingRequest >= channel.TimeComingRequest + channel.TimeBusyChannel).
                ToList().ForEach(channel =>
                {
                    channel.IsActive = false;
                    numOfChReqPairs.Remove(channel.IndexRequest);

                });

            //Делаем перераспределение при условии, что оно нужно
            Supporting(timeComingRequest);

        }


        private void Supporting(double timeComingRequest)
        {
            //Каналы, которым нужна помощь (проверям через индексы заявок)
            var channelsNeedHelp = channels.Where(channel => channel.IsActive).ToList().
                    Where(channel => numOfChReqPairs.ContainsKey(channel.IndexRequest) &&
                    numOfChReqPairs.GetValueOrDefault(channel.IndexRequest) < numberOfParalelChannels).ToList();

            //Через цикл проходимся по каналам, которым нужна помощь и добавляем в помощь свободные каналы
            foreach (var channel in channelsNeedHelp)
            {
                //Каналы, обрабатывающие одну заявку в кол-ве меньше l
                var tempBusyChannels = channelsNeedHelp.Where(ch => ch.IndexRequest == channel.IndexRequest).ToList();

                //Пускаем в помощь свободные каналы и пересчитываем время
                channels.Where(channel => !channel.IsActive).ToList().
                    Take(numberOfParalelChannels - tempBusyChannels.Count).ToList().
                    ForEach(supportChannel =>
                    {
                        supportChannel.IsActive = true;
                        supportChannel.IndexRequest = channel.IndexRequest;
                        supportChannel.TimeComingRequest = channel.TimeComingRequest;
                        supportChannel.TimeBusyChannel =
                            channel.TimeBusyChannel - (timeComingRequest - channel.TimeComingRequest + channel.TimeBusyChannel) 
                            * (tempBusyChannels.Count / numberOfParalelChannels);
                    });

                tempBusyChannels.ForEach(ch => ch.TimeBusyChannel -= (timeComingRequest - ch.TimeComingRequest + ch.TimeBusyChannel)
                            * (tempBusyChannels.Count / numberOfParalelChannels));
            }


        }












































        ///// <summary>
        ///// Метод для перераспределения каналов (оказания взаимопомощи)
        ///// </summary>
        //public void StartSupport(double timeComingRequest)
        //{
        //    //Поиск каналов, которым нужна помощь
        //    var channelsNeedHelp = channels.Where(channel => channel.IsActive).ToList();

        //    //Если каналов, которым нужна помощь меньше, чем l и больше 0, то начинаем оказывать взаимопомощь
        //    if (channelsNeedHelp.Count < numberOfParalelChannels && channelsNeedHelp.Count > 0)
        //    {
        //        //выберем канал, нуждающий в помощи для взятия параметров
        //        var channelNeedHelp = channelsNeedHelp.First();

        //        double newTimeBusy = 0;
        //        double timeComing = 0;

        //        //ищем каналы, которые могут помочь и мениям их состояние и время
        //        channels.Where(channel => !channel.IsActive).ToList().
        //            Take(numberOfParalelChannels - channelsNeedHelp.Count).ToList().
        //            ForEach(support =>
        //            {
        //                support.IsActive = true;

        //                support.IndexRequest = channelNeedHelp.IndexRequest;

        //                //время сколько прошло с момента обработки каналами, кол-во которых меньше l
        //                timeComing = support.timeComingRequest.Last() + support.timeBusyChannel.Last();

        //                //время, сколько осталось обрабатывать заявку l каналами
        //                newTimeBusy = (channelNeedHelp.timeBusyChannel.Last() -
        //                    support.timeComingRequest.Last() + support.timeBusyChannel.Last()) / numberOfParalelChannels;

        //                support.timeBusyChannel.Add(newTimeBusy);

        //                support.timeComingRequest.Add(timeComing);
        //            });

        //        //изменяем время у каналов, кол-во которым нужна была помощь
        //        channelsNeedHelp.ForEach(channel => channel.
        //                        timeBusyChannel[channel.timeBusyChannel.Count() - 1] = timeComing + newTimeBusy);
        //    }
        //}

    }
}
