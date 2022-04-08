using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    internal class Channel
    {
        public bool state;

        public int number;

        public int requestComplete;

        public int IndexRequest;

        /// <summary>
        /// Время обслуживания заявки
        /// </summary>
        public List<double> timeBusy = new();

        /// <summary>
        /// Время, когда заявка пришла и стала обслуживаться каналом
        /// </summary>
        public List<double> timeCommingRequest = new();
    }
}
