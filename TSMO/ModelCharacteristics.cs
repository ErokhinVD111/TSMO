using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    internal class ModelCharacteristics
    {
        #region Characteristics
        public readonly double _g;
        public readonly double _muMid;
        public readonly double _p;
        public readonly double _v;
        public readonly double _I;
        public readonly double _a;
        public readonly double lambda;
        public readonly double eta;
        public readonly double mu;
        public readonly double muOut;
        #endregion

        private static ModelCharacteristics modelInstance = null;

        private ModelCharacteristics(double g, double muMid, double p, double v, double I, double a)
        {
            _g = g;
            _muMid = muMid;
            _p = p;
            _v = v;
            _I = I;
            _a = a;
            lambda = v / I;
            eta = v / a;
            mu = g * muMid * p;
            muOut = mu + eta;
        }

        public static ModelCharacteristics SetModel(double g, double muMid, double p, double v, double I, double a)
        {
            if(modelInstance == null)
            {
                modelInstance = new ModelCharacteristics(g, muMid, p, v, I, a);
            }
            return modelInstance;
        }

    }
}
