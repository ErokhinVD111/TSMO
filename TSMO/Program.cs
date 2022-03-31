using TSMO;

ModelCharacteristics modelCharacteristics = ModelCharacteristics.GetModel
(
    g: 2,
    a: 30,
    I: 5,
    p: 0.51,
    muMid: 20,
    v: 1600
);

Console.WriteLine($"eta = {modelCharacteristics.eta}");
Console.WriteLine($"lambda = {modelCharacteristics.lambda}");
Console.WriteLine($"mu = {modelCharacteristics.mu}");
Console.WriteLine($"muOut = {modelCharacteristics.muOut}");

