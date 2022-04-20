using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    internal class TimeCalculator
    {
        /// <summary>
        /// Время поступления новой заявки
        /// </summary>
        private double timeComingRequest;


        /// <summary>
        /// Время моделирования
        /// </summary>
        public int TimeModeling { get; init; }


        /// <summary>
        /// Поле для генерации случайного числа от 0 до 1
        /// </summary>
        Random random = new Random();


        private static TimeCalculator timeCalculatorInstance = null;



        private TimeCalculator()
        {
            timeComingRequest = 0;
            TimeModeling = 1000000;
        }


        /// <summary>
        /// Метод для расчета времени прихода новой заявки
        /// </summary>
        /// <returns></returns>
        public double CalculateTimeComingRequest(double lambda)
        {
            timeComingRequest = timeComingRequest + (-100 / lambda) * Math.Log(random.NextDouble());
            return timeComingRequest;
        }


        /// <summary>
        /// Метод для расчета времени обслуживания заявки в зависимости от количества каналов
        /// </summary>
        /// <returns></returns>
        public double CalculateTimeService(double mu, double numberOfChannel)
        {
            if (numberOfChannel == 0)
                numberOfChannel = 1;
            return (-100 / (numberOfChannel * mu)) * Math.Log(random.NextDouble());
        }

        public static TimeCalculator GetTimeCalculator()
        {
            
            if(timeCalculatorInstance == null)
            {
                timeCalculatorInstance = new TimeCalculator();
            }

            return timeCalculatorInstance;
        }

    }
}
