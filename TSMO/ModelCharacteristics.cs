using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    public enum N : int
    {
        n1 = 2, n2, n3
    }
    internal class ModelCharacteristics
    {

        #region Characteristics

        public readonly double g; //Количество пусковых установок

        public readonly int l; //Количество взаимопомогающих каналов

        public readonly double muMid; //Скорострельность каждой пусковой установки

        public readonly double p; //Вероятность поражения цели одной установкой

        public readonly double v; //Скорость движения ракеты противника

        public readonly double I; //Средний линейный интервал между ракетами

        public readonly double a; //Зона обстрела

        public readonly double lambda;

        public readonly double mu;

        public double k; // Среднее число занятных каналов

        public double p_obs; 

        public int countRequestServices;

        public double countRequest;

        //public readonly double _tLoadedChannel;

        public N n;

        #endregion

        #region RequiredCharacteristics

        public double PLoadedChannel { get; set; } // Вероятность загруженности канала

        public double LambdaZero { get; set; }  // Плотность потока обслуженных заявок

        public double TEmptyChannel { get; set; } // Среднее время простоя канала

        #endregion

        private static ModelCharacteristics modelInstance = null;

        private ModelCharacteristics(double g, double muMid, double p, double v, double I, double a, int l, N n)
        {
            this.n = n;
            this.g = g;
            this.muMid = muMid;
            this.p = p;
            this.v = v;
            this.I = I;
            this.a = a;
            this.l = l;
            lambda = (v / I) / 60;
            mu = g * muMid * p;
        }

        public static ModelCharacteristics GetModel(double g, double muMid, double p, double v, double I, double a, int l, N n)
        {
            if(modelInstance == null)
            {
                modelInstance = new ModelCharacteristics(g, muMid, p, v, I, a, l, n);
            }
            return modelInstance;
        }

    }
}
