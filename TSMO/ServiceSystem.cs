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
        private Dictionary<int, List<Channel>> reqChannelsPairs;

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


        /// <summary>
        /// Базовые характеристики системы
        /// </summary>
        private BaseCharacteristicsSMO baseCharacteristicsSMO;


        /// <summary>
        /// Счетчик времени моделирования, обслуживания и прихода заявки
        /// </summary>
        private TimeCalculator timeCalculator;

        #endregion

        public ServiceSystem(BaseCharacteristicsSMO baseCharacteristicsSMO, TimeCalculator timeCalculator)
        {
            this.baseCharacteristicsSMO = baseCharacteristicsSMO;
            this.timeCalculator = timeCalculator;
            numberOfChannels = (int)baseCharacteristicsSMO.NumOfCh;
            numberOfParalelChannels = baseCharacteristicsSMO.L;
            channels = new(this.numberOfChannels);
            reqChannelsPairs = new();
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
        public void AddRequest(double timeComingRequest, int indexRequest)
        {
            //Освобождаем каналы, если время обслуживания вышло
            UpdateChannels(timeComingRequest);

            //Перераспределение
            Redistribution(timeComingRequest);

            //Получаем количество свободных каналов в зависимости от условия (k>=l, k<l, k=0), где k кол-во свободных каналов
            int countFreeChannels = GetCountFreeChannels();

            //Число занятых каналов
            CountBusyChPerIter.Add(GetCountBusyChannels());

            //Расчитываем время загруженности канала
            double timeBusyChannel = timeCalculator.CalculateTimeService(baseCharacteristicsSMO.Mu, countFreeChannels);

            //Ставим заявку на обслуживание в свободные каналы
            var freeChannels = channels.Where(channel => !channel.IsActive).ToList().Take(countFreeChannels).ToList();
            freeChannels.ForEach(channel =>
            {
                channel.IsActive = true;
                channel.IndexRequest = indexRequest;
                channel.TimeComingRequest = timeComingRequest;
                channel.TimeBusyChannel = timeBusyChannel;

            });
            if (countFreeChannels > 0)
            {
                reqChannelsPairs.Add(freeChannels.First().IndexRequest, freeChannels);
                CountRequestComplete++;
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
                    reqChannelsPairs.Remove(channel.IndexRequest);

                });

            //Делаем перераспределение при условии, что оно нужно
            Supporting(timeComingRequest);

        }

        /// <summary>
        /// Метод для взаимопомощи каналам
        /// </summary>
        /// <param name="timeComingRequest"></param>
        private void Supporting(double timeComingRequest)
        {
            //Каналы, которым нужна помощь (проверям через индексы заявок)
            double newTimeBusy;
            double newTimeComing = 0.0;
            bool isNewTime = false;
            foreach (var channel in reqChannelsPairs.Values)
            {
                foreach (var support in channels)
                {
                    if (channel.Count < numberOfChannels)
                    {
                        if (support.IsActive == false)
                        {
                            support.IsActive = true;
                            support.IndexRequest = channel.First().IndexRequest;

                            newTimeComing = support.TimeComingRequest + support.TimeBusyChannel;
                            channel.Add(support);
                            isNewTime = true;
                        }
                    }
                }
                if (isNewTime)
                {
                    newTimeBusy = timeCalculator.CalculateTimeService(baseCharacteristicsSMO.Mu, channel.Count);
                    channel.ForEach(ch =>
                    {
                        ch.TimeComingRequest = newTimeComing;
                        ch.TimeBusyChannel = newTimeBusy;
                    });
                }
            }
        }

        /// <summary>
        /// Перераспределение
        /// </summary>
        /// <param name="timeComingRequest"></param>
        private void Redistribution(double timeComingRequest)
        {
            int countReqInSystem = reqChannelsPairs.Keys.Count;
            if (countReqInSystem + 1 <= numberOfChannels)
            {
                foreach (var channel in reqChannelsPairs.Values)
                {
                    if (numberOfChannels - GetCountBusyChannels() == 0)
                    {

                        if (channel.Count > 1)
                        {
                            channel.Last().IsActive = false;
                            channel.Remove(channel.Last());
                            double newTimeBusy = timeCalculator.CalculateTimeService(baseCharacteristicsSMO.Mu, channel.Count);
                            channel.ForEach(ch =>
                            {
                                ch.TimeComingRequest = timeComingRequest;
                                ch.TimeBusyChannel = newTimeBusy;
                            });
                        }

                    }
                }
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
