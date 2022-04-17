using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    public enum NumberOfChannels : int
    {
        n1 = 2, n2, n3
    }
    internal class BaseCharacteristicsSMO
    {

        #region Characteristics

        /// <summary>
        /// Количество пусковых установок
        /// </summary>
        public int G { get; init; }


        /// <summary>
        /// Количество взаимопомогающих каналов
        /// </summary>
        public int L { get; init; }


        /// <summary>
        /// Скорострельность каждой пусковой установки
        /// </summary>
        public double MuSingle { get; init; }


        /// <summary>
        /// Вероятность поражения цели одной установкой
        /// </summary>
        public double P { get; init; }


        /// <summary>
        /// Скорость движения ракеты противника
        /// </summary>
        public double V { get; init; }


        /// <summary>
        /// Средний линейный интервал между ракетами
        /// </summary>
        public double I { get; init; }


        /// <summary>
        /// Зона обстрела
        /// </summary>
        public double A { get; init; }


        /// <summary>
        /// Кол-во каналов в системе
        /// </summary>
        public NumberOfChannels NumOfCh { get; set; }


        /// <summary>
        /// Плотность входящего потока заявок
        /// </summary>
        public double Lambda { get; init; }


        /// <summary>
        /// Плотность потока обслуживания
        /// </summary>
        public double Mu { get; init; } 

        #endregion


        private static BaseCharacteristicsSMO baseChSMOInstance = null;


        private BaseCharacteristicsSMO(int g, double muSing, double p, double v, double i, double a, int l, NumberOfChannels n)
        {
            NumOfCh = n;
            G = g;
            MuSingle = muSing;
            P = p;
            V = v;
            I = i;
            A = a;
            L = l;
            Lambda = (v / I) / 60;
            Mu = g * muSing * p;
        }

        public static BaseCharacteristicsSMO GetModel(int g, double muSing, double p, double v, double i, double a, int l, NumberOfChannels n)
        {
            if (baseChSMOInstance == null)
            {
                baseChSMOInstance = new BaseCharacteristicsSMO(g, muSing, p, v, i, a, l, n);
            }
            return baseChSMOInstance;
        }

    }
}
