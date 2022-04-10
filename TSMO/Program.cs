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
    l: 2,
    n: N.n3
);


// Создаем экземпляр для расчета времени поступления и обслуживания заявок
Time time = new Time(modelCharacteristics.lambda, modelCharacteristics.mu, modelCharacteristics.l);


// Создаем экземпляр системы обслуживания
ServiceSystem serviceSystem = ServiceSystem.GetServiceSystem(modelCharacteristics.n, modelCharacteristics.l);


//Устанавливаем в системе начальные значения для каналов обслуживания
serviceSystem.SetDefaultChannels();


//Время прихода заявки
double timeComingRequest = 0;


//Время обслуживания заявки
double timeBusyChannel = 0;


//Счетчик отказов
int requestDenied = 0;

//Счетчик заявок
int countRequest = 0;

//Счетчик обслуженных заявок
int countProccesedRequest = 0;


//Основной цикл (генерация заявки происходит на каждой итерации)
for (int t = 0; t < time.GetTimeModeling(); t++)
{
    timeComingRequest = time.CalculateTi(); //Рассчитываем время поступления заявки

    //Проверка, что каналы можно осовбодить (т.к. время обслуживания вышло)
    foreach (var freeChannel in serviceSystem.Channels)
    {
        //Проверяем состояние каждого канала
        if (freeChannel.IsActive == true)
        {
            //Проверка что время обслуживания вышло
            if (timeComingRequest >= freeChannel.timeCommingRequest.Last() + freeChannel.timeBusy.Last())
            {

                freeChannel.IsActive = false;

                serviceSystem.RequestComplete++;

                //Начинаем перераспределение при условии, что оно нужно

                int countBusyChannel = 0; //количество каналов, которые обслуживают определенную заявку 

                foreach (var busyChannel in serviceSystem.Channels)
                {
                    //Если индекс заявки освободившегося канала не равен индексу заявки занятого, то проверяем не нужна ли помощь занятому каналу
                    if (freeChannel.IndexRequest != busyChannel.IndexRequest && busyChannel.IsActive == true)
                    {
                        //Через цикл ищем количество каналов, которые обслуживают нужный индекс заявки
                        foreach (var channel in serviceSystem.Channels)
                        {
                            if (channel.IndexRequest == busyChannel.IndexRequest)
                            {
                                countBusyChannel++;
                            }
                        }
                        //Если количество облуживающих заявку каналов меньше l
                        if (countBusyChannel < serviceSystem.CountParalelChannels)
                        {
                            freeChannel.IsActive = true;

                            freeChannel.IndexRequest = busyChannel.IndexRequest;

                            int countSupportChannel = 0;

                            foreach (var supportChannel in serviceSystem.Channels)
                            {
                                if (supportChannel.IndexRequest == busyChannel.IndexRequest)
                                {
                                    countSupportChannel++;
                                }
                            }

                            //Время, сколько канал в одиночку обрабатывал заявку до момента взаимопомощи
                            double timeNewRequest = busyChannel.timeBusy.Last() - freeChannel.timeCommingRequest.Last() + freeChannel.timeBusy.Last();

                            //Оставшееся время обработки заявки для l каналов
                            double timeNewBusy = timeNewRequest / (serviceSystem.CountParalelChannels / countSupportChannel);

                            //Меняем время загруженности канала
                            busyChannel.timeBusy[busyChannel.timeBusy.Count - 1] = timeNewRequest + timeNewBusy;

                            freeChannel.timeBusy.Add(timeNewBusy);
                            freeChannel.timeCommingRequest.Add(timeNewRequest);

                            break;

                        }

                    }
                    
                }

            }
        }
    }


    //Проверям состояние каналов (есть ли среди них свободные и удовлетворяющие условию число_свободных_каналов >= l)
    if (serviceSystem.CheckCountFreeChannels() >= serviceSystem.CountParalelChannels)
    {
        //Рассчитываем время обслуживания заявки
        timeBusyChannel = time.CalculateTService();
        for (int i = 0, j = 0; i < serviceSystem.CountParalelChannels;)
        {
            //Ищем свободные каналы
            if (serviceSystem.Channels[j].IsActive == false)
            {
                serviceSystem.Channels[j].IsActive = true;
                serviceSystem.Channels[j].timeBusy.Add(timeBusyChannel);
                serviceSystem.Channels[j].timeCommingRequest.Add(timeComingRequest);
                serviceSystem.Channels[j].IndexRequest = t;
                i++;
            }
            j++;
        }
        countRequest++;
        countProccesedRequest++;

    }
    //Проверям состояние каналов (есть ли вообще свободные)
    else if (serviceSystem.CheckCountFreeChannels() > 0)
    {
        //Рассчитываем время обслуживания заявки
        timeBusyChannel = time.CalculateTService();
        for (int i = 0, j = 0; i < serviceSystem.CheckCountFreeChannels();)
        {
            //Ищем свободные каналы
            if (serviceSystem.Channels[j].IsActive == false)
            {
                serviceSystem.Channels[j].IsActive = true;
                serviceSystem.Channels[j].timeBusy.Add(timeBusyChannel);
                serviceSystem.Channels[j].timeCommingRequest.Add(timeComingRequest);
                serviceSystem.Channels[j].IndexRequest = t;
                i++;
            }
            j++;
        }
        countRequest++;
        countProccesedRequest++;
    }
    else
    {
        requestDenied++;
        countRequest++;

    }

}
// served - число обслуженных заявок, amount - число поступивших заявок
//k_sr = (lambda * served ) / (mu * amount); 

//P_obs = mu * k_sr / lambda;

//lamda_0 = P_obs * lambda;

//Pз.к. = k/n

//tп.к. = (1 / mu) * ((1 - Pз.к.) / Pз.к.)

//Расчет необходимых характеристик:

double k_sr = (modelCharacteristics.lambda * countProccesedRequest) / (modelCharacteristics.mu * countRequest);
double p_obs = modelCharacteristics.mu * k_sr / modelCharacteristics.lambda;
double lamda_0 = p_obs * modelCharacteristics.lambda;
double p_zk = k_sr / (int)modelCharacteristics.n;
double t_pk = (1 / modelCharacteristics.mu) * ((1 - p_zk) / p_zk);


Console.WriteLine($"К среднее = {k_sr}");
Console.WriteLine($"P обслуживания = {p_obs}");
Console.WriteLine($"l0 = {lamda_0}");
Console.WriteLine($"P загр. канала = {p_zk}");
Console.WriteLine($"Время простоя канала = {t_pk}");
