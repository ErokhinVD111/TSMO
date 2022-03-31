using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    internal class Time
    {
        private double ti;

        private double tService;

        private int tModeling;

        Random random = new Random();

        private double _lambda;

        private double _muOut;

        private int _l;

        public Time(double lambda, double muOut, int l)
        {
            _lambda = lambda;
            _muOut = muOut;
            _l = l;
            ti = 0;
            tModeling = 1000000;
        }

        public double CalculateTi()
        {
            ti = ti + (-100 / _lambda) * Math.Log(random.NextDouble());
            return ti;
        }

        public double CalculateTService()
        {
            tService = (-100 / (_l * _muOut)) * Math.Log(random.NextDouble());
            return tService;
        }

        public int GetTimeModeling()
        {
            return tModeling;
        }



    }
}
