using TSMO;


// Создаем экземпляр модели с начальными характеристиками
ModelCharacteristics modelCharacteristics = ModelCharacteristics.GetModel
(
    g: 2,
    a: 30,
    I: 5,
    p: 0.51,
    muMid: 0.333,
    v: 1600,
    l: 2
);


// Создаем экземпляр для расчета времени поступления и обслуживания заявок
Time time = new Time(modelCharacteristics.lambda, modelCharacteristics.mu, modelCharacteristics._l);


// Создаем экземпляр системы обслуживания
ServiceSystem serviceSystem = ServiceSystem.GetServiceSystem(modelCharacteristics.n, modelCharacteristics._l);


//Устанавливаем в системе начальные значения
serviceSystem.SetDefaultChannels();


//время прихода заявки
double timeComingRequest = 0;


//время обслуживания заявки
double timeBusyChannel = 0;


//счетчик отказов
int requestDenied = 0;



//Основной цикл (генерация заявки происходит на каждой итерации)
for (int t = 0; t < time.GetTimeModeling(); t++)
{
    timeComingRequest = time.CalculateTi(); //Рассчитываем время поступления заявки

    //Проверка, что каналы можно осовбодить (т.к. время обслуживания вышло)
    foreach (var freeChannel in serviceSystem.Channels)
    {
        //Проверяем состояние каждого канала
        if (freeChannel.state == true)
        {
            //Проверка что время обслуживания вышло
            if (timeComingRequest >= freeChannel.timeCommingRequest.Last() + freeChannel.timeBusy.Last())
            {

                freeChannel.state = false;

                freeChannel.requestComplete++;

                //начинаем перераспределение при условии, что оно нужно

                int countBusyChannel = 0; //количество каналов, которые обслуживают определенную заявку 

                foreach (var busyChannel in serviceSystem.Channels)
                {
                    //если индекс заявки освободившегося канала не равен индексу заявки занятого, то проверяем не нужна ли помощь занятому каналу
                    if (freeChannel.IndexRequest != busyChannel.IndexRequest && busyChannel.state == true)
                    {
                        //через цикл ищем количество каналов, которые обслуживают нужный индекс заявки
                        foreach (var channel in serviceSystem.Channels)
                        {
                            if (channel.IndexRequest == busyChannel.IndexRequest)
                            {
                                countBusyChannel++;
                            }
                        }

                    }
                    //если количество облуживающих заявку каналов меньше l
                    if (countBusyChannel < serviceSystem.CountParalelChannels)
                    {
                        freeChannel.state = true;

                        freeChannel.IndexRequest = busyChannel.IndexRequest;

                        int countSupportChannel = 0;

                        foreach (var supportChannel in serviceSystem.Channels)
                        {
                            if (supportChannel.IndexRequest == busyChannel.IndexRequest)
                            {
                                countSupportChannel++;
                            }
                        }

                        //время, сколько канал в одиночку обрабатывал заявку до момента взаимопомощи
                        double timeNewRequest = busyChannel.timeBusy.Last() - freeChannel.timeCommingRequest.Last() + freeChannel.timeBusy.Last();

                        //оставшееся время обработки заявки для l каналов
                        double timeNewBusy = timeNewRequest / (serviceSystem.CountParalelChannels / countSupportChannel);

                        //меняем время загруженности канала
                        busyChannel.timeBusy[busyChannel.timeBusy.Count - 1] = timeNewRequest + timeNewBusy;

                        freeChannel.timeBusy.Add(timeNewBusy);
                        freeChannel.timeCommingRequest.Add(timeNewRequest);

                        break;

                    }
                }

            }
        }
    }


    //Проверям состояние каналов (есть ли среди них свободные и удовлетворяющие условию число_свободных_каналов >= l)
    if (serviceSystem.CheckCountFreeChannels() >= serviceSystem.CountParalelChannels)
    {
        //Рассчитываем время обслуживания заявки
        timeBusyChannel = time.CalculateTService(serviceSystem.CountParalelChannels);
        for (int i = 0, j = 0; i < serviceSystem.CountParalelChannels;)
        {
            //Ищем свободные каналы
            if (serviceSystem.Channels[j].state == false)
            {
                serviceSystem.Channels[j].state = true;
                serviceSystem.Channels[j].timeBusy.Add(timeBusyChannel);
                serviceSystem.Channels[j].timeCommingRequest.Add(timeComingRequest);
                serviceSystem.Channels[j].IndexRequest = t;
                i++;
            }
        }

    }
    //Проверям состояние каналов (есть ли вообще свободные)
    else if (serviceSystem.CheckCountFreeChannels() > 0)
    {
        //Рассчитываем время обслуживания заявки
        timeBusyChannel = time.CalculateTService(serviceSystem.CheckCountFreeChannels());
        for (int i = 0, j = 0; i < serviceSystem.CheckCountFreeChannels();)
        {
            //Ищем свободные каналы
            if (serviceSystem.Channels[j].state == false)
            {
                serviceSystem.Channels[j].state = true;
                serviceSystem.Channels[j].timeBusy.Add(timeBusyChannel);
                serviceSystem.Channels[j].timeCommingRequest.Add(timeComingRequest);
                serviceSystem.Channels[j].IndexRequest = t;
                i++;
            }
        }
    }
    else
    {
        requestDenied++;
    }



}

double sumTimeBusyChannel = 0;
foreach (var channel in serviceSystem.Channels)
{
    foreach (var timeBsy in channel.timeBusy)
    {
        sumTimeBusyChannel += timeBsy / time.GetTimeModeling();
    }
}
Console.WriteLine($"Вероятность занятости канала: {(double)(sumTimeBusyChannel / (int)modelCharacteristics.n)}");
