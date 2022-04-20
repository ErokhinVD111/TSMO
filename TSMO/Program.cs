using TSMO;


// Создаем экземпляр с начальными характеристиками для СМО
BaseCharacteristicsSMO characteristicsSMO = BaseCharacteristicsSMO.GetModel
(
    g: 2,
    a: 30,
    i: 5,
    p: 0.51,
    muSing: 0.333,
    v: 1600,
    l: 2,
    n: NumberOfChannels.n3
);


// Создаем экземпляр для расчета времени поступления и обслуживания заявок
TimeCalculator timeCalculator = TimeCalculator.GetTimeCalculator();


// Создаем экземпляр системы обслуживания
ServiceSystem serviceSystem = new ServiceSystem(characteristicsSMO, timeCalculator);



//Основной цикл (генерация заявки происходит на каждой итерации)
for (int t = 0; t < timeCalculator.TimeModeling; t++)
{

    //Время прихода заявки
    double timeComingRequest = timeCalculator.CalculateTimeComingRequest(characteristicsSMO.Lambda); 

    //Время обслуживания заявки
    double timeBusyChannel = timeCalculator.CalculateTimeService(characteristicsSMO.Mu, serviceSystem.GetCountFreeChannels()); 

    //Обслуживание пришедшей заявки
    serviceSystem.AddRequest(timeComingRequest, t);

}


//Расчет необходимых характеристик:

double k_sr = (double) serviceSystem.CountBusyChPerIter.Sum() / timeCalculator.TimeModeling;
double p_obs = (double) serviceSystem.CountRequestComplete / serviceSystem.CountRequest;
double lamda_0 = p_obs * characteristicsSMO.Lambda;
double p_zk = k_sr / (int)characteristicsSMO.NumOfCh;
//double t_pk = (1 / characteristicsSMO.Mu) * ((1 - p_zk) / p_zk);


Console.WriteLine($"Mю: = {characteristicsSMO.Mu}");
Console.WriteLine($"Lamda: = {characteristicsSMO.Lambda}");
Console.WriteLine($"Кол-во заявок: = {serviceSystem.CountRequest}");
Console.WriteLine($"Кол-во обслуженных заявок = {serviceSystem.CountRequestComplete}");
Console.WriteLine($"Кол-во необслуженных заявок = {serviceSystem.CountRequestDenied}");
Console.WriteLine($"К среднее = {k_sr}");
Console.WriteLine($"P обслуживания = {p_obs}");
Console.WriteLine($"l0 = {lamda_0}");
Console.WriteLine($"P загр. канала = {p_zk}");
//Console.WriteLine($"Время простоя канала = {t_pk}");
