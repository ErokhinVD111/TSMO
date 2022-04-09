using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    internal class Time
    {
        /// <summary>
        /// Время поступления новой заявки
        /// </summary>
        private double ti;


        /// <summary>
        /// Время обслуживания
        /// </summary>
        private double tService;


        /// <summary>
        /// Время моделирования
        /// </summary>
        private int tModeling;


        /// <summary>
        /// Поле для генерации случайного числа от 0 до 1
        /// </summary>
        Random random = new Random();

        private double _lambda;

        private double _mu;

        private int _l;

        public Time(double lambda, double mu, int l)
        {
            _lambda = lambda;
            _mu = mu;
            _l = l;
            ti = 0;
            tModeling = 1000000;
        }


        /// <summary>
        /// Метод для расчета времени прихода новой заявки
        /// </summary>
        /// <returns></returns>
        public double CalculateTi()
        {
            ti = ti + (-100 / _lambda) * Math.Log(random.NextDouble());
            return ti;
        }


        /// <summary>
        /// Метод для расчета времени обслуживания заявки в зависимости от количества каналов
        /// </summary>
        /// <returns></returns>
        public double CalculateTService(int countChannel)
        {
            double tService = 0;
            for (int i = 0; i < countChannel; i++)
                tService += (-100 / (_l * _mu)) * Math.Log(random.NextDouble());
            return tService;
        }


        /// <summary>
        /// Метод для получения времени моделирования
        /// </summary>
        /// <returns></returns>
        public int GetTimeModeling()
        {
            return tModeling;
        }



    }
}
