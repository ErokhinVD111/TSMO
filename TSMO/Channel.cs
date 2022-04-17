using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    internal class Channel
    {
        /// <summary>
        /// Состояние канала (если обрабатывает заявку IsActive = True)
        /// </summary>
        public bool IsActive { get; set; }


        /// <summary>
        /// Индекс заявки, которую обрабатывает канал
        /// </summary>
        public int IndexRequest { get; set; }


        /// <summary>
        /// Время обслуживания заявки
        /// </summary>
        public double TimeBusyChannel { get; set; }


        /// <summary>
        /// Время, когда канал взял заявку на обслуживание
        /// </summary>
        public double  TimeComingRequest { get; set; }

       
    }
}
