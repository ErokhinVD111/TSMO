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
    foreach(var channel in serviceSystem.Channels)
    {
        if(channel.state == true)
        {
            if(timeComingRequest >= channel.timeCommingRequest.Last() + channel.timeBusy.Last())
            {
                channel.state = false;
                channel.requestComplete++;
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
            if (serviceSystem.Channels[j].state == false)
            {
                serviceSystem.Channels[j].state = true;
                serviceSystem.Channels[j].timeBusy.Add(timeBusyChannel);
                i++;
            }
        }

    }
    //Проверям состояние каналов (есть ли вообще свободные)
    else if(serviceSystem.CheckCountFreeChannels() > 0)
    {
        //Рассчитываем время обслуживания заявки
        timeBusyChannel = time.CalculateTService();
        for (int i = 0, j = 0; i < serviceSystem.CheckCountFreeChannels();)
        {
            if (serviceSystem.Channels[j].state == false)
            {
                serviceSystem.Channels[j].state = true;
                serviceSystem.Channels[j].timeBusy.Add(timeBusyChannel);
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

/*проверка на выполнение заявки, если true, то освобождаем каналы
if (t >= timeComingRequest + timeBusyChannel)
{
    foreach (var channel in serviceSystem.Channels)
    {
        channel.state = false;
    }
}*/