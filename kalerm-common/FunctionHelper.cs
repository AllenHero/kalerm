using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalerm_common
{
    public class FunctionHelper
    {
        /// <summary>
        /// 初始数组
        /// </summary>
        public decimal[] Samples;

        /// <summary>
        /// 最大值 MAX
        /// </summary>
        public decimal Max { get; set; }

        /// <summary>
        /// 最小值 MIN
        /// </summary>
        public decimal Min { get; set; }

        /// <summary>
        /// 平均值 AVE
        /// </summary>
        public decimal Mean { get; set; }

        public FunctionHelper(decimal[] Samples)
        {
            this.Samples = Samples;
            GetMax();
            GetMin();
            GetMean();       
        }

        #region 最大值 MAX

        /// <summary>
        /// 最大值 MAX
        /// </summary>
        /// <returns></returns>
        public void GetMax()
        {
            decimal max = this.Samples[0];
            for (int i = 0; i < this.Samples.Length; i++)
            {
                max = Math.Max(max, this.Samples[i]);
            }
            this.Max = max;
        }

        #endregion

        #region 最小值 MIN

        /// <summary>
        /// 最小值 MIN
        /// </summary>
        /// <returns></returns>
        public void GetMin()
        {
            decimal min = this.Samples[0];
            for (int i = 0; i < this.Samples.Length; i++)
            {
                min = Math.Min(min, this.Samples[i]);
            }
            this.Min = min;
        }

        #endregion

        #region 平均值 AVE

        /// <summary>
        /// 平均值 AVE
        /// </summary>
        /// <returns></returns>
        public void GetMean()
        {
            decimal sumup = 0;
            for (int i = 0; i < this.Samples.Length; i++)
            {
                sumup += this.Samples[i];
            }
            this.Mean = (sumup / ((decimal)this.Samples.Length));
        }

        #endregion
    }
}
