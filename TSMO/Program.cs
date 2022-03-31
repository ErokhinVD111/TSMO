using TSMO;

ModelCharacteristics modelCharacteristics = ModelCharacteristics.GetModel
(
    g: 2,
    a: 30,
    I: 5,
    p: 0.51,
    muMid: 20,
    v: 1600,
    l: 2
);

Time time = new Time(modelCharacteristics.lambda, modelCharacteristics.muOut, modelCharacteristics._l);

ServiceSystem serviceSystem = ServiceSystem.GetServiceSystem(modelCharacteristics.n, modelCharacteristics._l);
serviceSystem.SetChannels();
serviceSystem.CheckStateChannels();




/// <summary>
/// Основной цикл
/// </summary>
for(int t = 0; t < time.GetTimeModeling(); t++)
{
    //если true, то мы принимаем заявку на обслуживание
    if(serviceSystem.CheckStateChannels() >= serviceSystem.CountParalelChannels)
    {
        //рассчитываем время поступления заявки
        double timeComingRequest = time.CalculateTi();
        //если время моделирования больше времени поступления заявки, то начинаем обслуживать заявку 
        if((double)t >= timeComingRequest)
        {
            foreach (var channel in serviceSystem.Channels)
            {
                channel.state = true;
            }
        }
        
    }
}

