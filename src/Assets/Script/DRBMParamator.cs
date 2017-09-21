using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drbm_c_sharp
{
    public class DRBMParamator
    {
        protected System.Random rand = new System.Random();

        public int xSize; // input node size
        public int hSize; // hidden node size
        public int ySize; // output node size
        public int trainCount = 0;

        // params
        public double[] w;  // coupling params between input and hidden    
        public double[] c;  // hidden bias params
        public double[] v;  // cpupling params between hidden and output
        public double[] d;  // output bias params

        protected double _uniform()
        {
            // FIXME: Unityで[-0.01, 0.01]の一様乱数を生成する処理を実装してください
            return 2 * (this.rand.NextDouble() - 0.5) * 0.01;
        }

        public DRBMParamator(int x_size, int h_size, int y_size)
        {
            this.xSize = x_size;
            this.hSize = h_size;
            this.ySize = y_size;


            this.w = new double[x_size * h_size];
            /*List<List<double>>(x_size);
            for (int i = 0; i < this.xSize; i++)
            {
                this.w.Add((new double[h_size]).ToList());
            }*/

            this.c = new double[h_size];
            //(new double[h_size]).ToList();

            this.v = new double[h_size * y_size];
            /*
            new List<List<double>>(h_size);
            for (int i = 0; i < this.hSize; i++)
            {
                this.v.Add((new double[y_size]).ToList());
            }
            */

            this.d = new double[y_size];
            //(new double[y_size]).ToList();

            // initialize params
            for (int i = 0; i < this.xSize; i++)
            {
                for (int j = 0; j < this.hSize; j++)
                {
                    this.w[i * this.hSize + j] = this._uniform();
                }
            }

            for (int i = 0; i < this.hSize; i++)
            {
                this.c[i] = this._uniform();
            }

            for (int i = 0; i < this.hSize; i++)
            {
                for (int j = 0; j < this.ySize; j++)
                {
                    this.v[i * y_size + j] = this._uniform();
                }
            }

            for (int i = 0; i < this.ySize; i++)
            {
                this.c[i] = this._uniform();
            }

        }
    }
}
