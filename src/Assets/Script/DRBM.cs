using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drbm_c_sharp
{
    public class DRBM
    {
        protected System.Random rand = new System.Random();

        public int xSize; // input node size
        public int hSize; // hidden node size
        public int ySize; // output node size

        // variables
        List<double> x;  // input nodes
        List<double> h;  // hidden nodes
        List<double> y;  // output nodes

        // params
        public DRBMParamator param;

        protected double _oneExp(int j, int k)
        {
            double val = 1.0 + Math.Exp(this._lambda(j, k));

            return val;
        }

        protected double _prodOneExpNeqJ(int j, int k)
        {
            double val = 1.0;

            for (int l = 0; l < j; l++)
            {
                val *= this._oneExp(j, k);
            }

            for (int l = j + 1; l < this.hSize; l++)
            {
                val *= this._oneExp(j, k);
            }

            return val;
        }

        protected double _exp(int j, int k)
        {
            return Math.Exp(this._lambda(j, k));
        }

        protected double _sigmoid(ref List<double> data, int label_no, int j)
        {
            double lambda = 0.0;

            for (int i = 0; i < this.xSize; i++)
            {
                lambda += this.param.w[i * param.hSize + j] * data[i];
            }

            lambda += this.param.c[j];
            lambda += this.param.v[j * param.ySize + label_no];

            return 1.0 / (1.0 + Math.Exp(-lambda));
        }

        protected double _lambda(int j, int k)
        {
            double val = 0.0;

            for (int i = 0; i < this.xSize; i++)
            {
                val += this.param.w[i * param.hSize + j] * this.x[i];
            }

            val += this.param.c[j];
            val += this.param.v[j * param.ySize + k];

            return val;
        }

        protected List<List<double>> _calcProdOneExpAndExp(ref List<double> data)
        {
            this.x = data;

            List<List<double>> prod_table = new List<List<double>>(this.hSize);
            for (int i = 0; i < this.hSize; i++)
            {
                prod_table.Add((new double[this.ySize]).ToList());
                for (int j = 0; j < ySize; j++)
                {
                    prod_table[i][j] = 1.0;
                }
            }

            for (int j = 0; j < this.hSize; j++)
            {
                for (int k = 0; k < this.ySize; k++)
                {

                    prod_table[0][k] *= this._oneExp(j, k);
                }
            }

            for (int j = this.hSize - 1; 0 <= j; j--)
            {
                for (int k = this.ySize - 1; 0 <= k; k--)
                {
                    prod_table[j][k] = prod_table[0][k];
                    prod_table[j][k] /= this._oneExp(j, k);
                    prod_table[j][k] *= this._exp(j, k);
                }
            }

            return prod_table;
        }

        protected List<List<double>> _calcExpectedXH(ref List<double> data)
        {
            this.x = data;

            double norm_const = this.normalizeConstant();

            List<List<double>> exp_values = new List<List<double>>(this.xSize);

            for (int i = 0; i < this.xSize; i++)
            {
                exp_values.Add((new double[this.hSize]).ToList());
                for (int j = 0; j < this.hSize; j++)
                {
                    exp_values[i][j] = 1.0 / norm_const;
                }
            }

            List<List<double>> prod_table = this._calcProdOneExpAndExp(ref data);

            for (int i = 0; i < this.xSize; i++)
            {
                for (int j = 0; j < this.hSize; j++)
                {
                    double sum_k = 0.0;

                    for (int k = 0; k < this.ySize; k++)
                    {
                        sum_k += prod_table[j][k] * Math.Exp(this.param.c[k]);
                    }

                    exp_values[i][j] *= sum_k * data[i];
                }
            }

            return exp_values;
        }

        protected List<double> _calcExpectedH(ref List<double> data)
        {
            this.x = data;

            double norm_const = this.normalizeConstant();
            List<double> exp_values = (new double[this.hSize]).ToList();
            
            for (int i = 0; i < this.hSize; i++)
            {
                exp_values[i] = 1.0 / norm_const;
            }

            List<List<double>> prod_table = this._calcProdOneExpAndExp(ref data);

            for (int j = 0; j < this.hSize; j++)
            {
                double tmp_val = 0.0;

                for (int k = 0; k < this.ySize; k++)
                {
                    tmp_val += Math.Exp(this.param.c[k]) * prod_table[j][k];
                }

                exp_values[j] *= tmp_val;
            }

            return exp_values;
        }

        protected List<List<double>> _calcExpectedHY(ref List<double> data)
        {
            this.x = data;

            double norm_const = this.normalizeConstant();

            List<List<double>> exp_values = new List<List<double>>(this.hSize);

            for (int i = 0; i < this.hSize; i++)
            {
                exp_values.Add((new double[this.ySize]).ToList());

                for (int j = 0; j < this.ySize; j++)
                {
                    exp_values[i][j] = 1.0 / norm_const;
                }
            }

            List<List<double>> prod_table = this._calcProdOneExpAndExp(ref data);

            for (int j = 0; j < this.hSize; j++)
            {
                for (int k = 0; k < this.ySize; k++)
                {
                    exp_values[j][k] *= Math.Exp(this.param.c[k]) * prod_table[j][k];

                }

            }

            return exp_values;

        }

        protected List<double> _calcExpectedY(ref List<double> data)
        {
            this.x = data;
            double norm_const = this.normalizeConstant();

            List<double> exp_vals = (new double[this.ySize]).ToList();
            
            for (int i = 0; i < this.ySize; i++)
            {
                exp_vals[i] = 1.0 / norm_const;
            }

            for (int k = 0; k < this.ySize; k++)
            {

                exp_vals[k] *= Math.Exp(this.param.c[k]);

                for (int j = 0; j < this.hSize; j++)
                {
                    exp_vals[k] *= this._oneExp(j, k);
                }
            }

            return exp_vals;
        }


        protected void _shuffle<T>(ref List<T> array)
        {
            System.Random rand = new System.Random();

            int n = array.Count;
            T tmp;
            for (int i = 0; i < n; i++)
            {
                int r = i + (int)(rand.NextDouble() * (n - i));
                tmp = array[r];
                array[r] = array[i];
                array[i] = tmp;
            }
        }

        public DRBM(int x_size, int h_size, int y_size)
        {
            this.xSize = x_size;
            this.hSize = h_size;
            this.ySize = y_size;

            // constracting vars
            this.x = (new double[x_size]).ToList();
            this.h = (new double[h_size]).ToList();
            this.y = (new double[y_size]).ToList();

            //params
            this.param = new DRBMParamator(x_size, h_size, y_size);
        }


        public double normalizeConstant()
        {
            // FIXME: what is x?
            double norm_const = 0.0;
            // y loop
            Func<int, double> exp_prod_one_plus_exp =
                (k) =>
                {
                    double constant = Math.Exp(this.param.c[k]);

                    for (int j = 0; j < this.hSize; j++)
                    {
                        constant *= this._oneExp(j, k);
                    }

                    return constant;
                };

            for (int k = 0; k < this.ySize; k++)
            {
                norm_const += exp_prod_one_plus_exp(k);
            }

            return norm_const;
        }

        public int discriminate(ref List<double> data)
        {
            this.x = data;

            // calc class prob;
            Func<int, double, double> class_prob =
                (k, norm_constnce) =>
                {
                    double potential = Math.Exp(this.param.c[k]) / norm_constnce;

                    for (int j = 0; j < this.hSize; j++)
                    {
                        potential *= this._oneExp(j, k);
                    }

                    return potential;
                };

            double norm_const = this.normalizeConstant();

            // discriminate
            List<double> probs = (new double[this.ySize]).ToList();

            for (int k = 0; k < this.ySize; k++)
            {
                probs[k] = class_prob(k, norm_const);
            }

            // what is max-prob?

            var max_elem = probs.Max();
            int class_no = probs.IndexOf(max_elem);

            return class_no;
        }


        public void train(ref List<List<double>> dataset, ref List<int> labelset, int batch_size, double learning_rate, int epoch)
        {

            List<List<double>> mom_xh = new List<List<double>>(this.xSize);
            for (int i = 0; i < this.xSize; i++)
            {
                mom_xh.Add((new double[this.hSize]).ToList());
            }

            List<double> mom_h = (new double[this.hSize]).ToList();

            List<List<double>> mom_hy = new List<List<double>>(this.hSize);
            for (int i = 0; i < this.hSize; i++)
            {
                mom_hy.Add((new double[this.ySize]).ToList());
            }

            List<double> mom_y = (new double[this.ySize]).ToList();

            List<int> indexes = (new int[labelset.Count]).ToList();

            for (int i = 0; i < indexes.Count; i++) indexes[i] = i;

            // shuffle index
            this._shuffle(ref indexes);

            batch_size = indexes.Count < batch_size ? indexes.Count : batch_size;
 

            for (int e = 0; e < epoch; e++)
            {
                List<List<double>> xh_data = new List<List<double>>(this.xSize);
                for (int i = 0; i < this.xSize; i++)
                {
                    xh_data.Add((new double[this.hSize]).ToList());
                }

                List<double> h_data = (new double[this.hSize]).ToList();

                List<List<double>> hy_data = new List<List<double>>(this.hSize);
                for (int i = 0; i < this.hSize; i++)
                {
                    hy_data.Add((new double[this.ySize]).ToList());
                }

                List<double> y_data = (new double[this.ySize]).ToList();

                List<List<double>> xh_drbm = new List<List<double>>(this.xSize);
                for (int i = 0; i < this.xSize; i++)
                {
                    xh_drbm.Add((new double[this.hSize]).ToList());
                }

                List<double> h_drbm = (new double[this.hSize]).ToList();

                List<List<double>> hy_drbm = new List<List<double>>(this.hSize);
                for (int i = 0; i < this.hSize; i++)
                {
                    hy_drbm.Add((new double[this.ySize]).ToList());
                }

                List<double> y_drbm = (new double[this.ySize]).ToList();



                for (int n = 0; n < batch_size; n++)
                {
                    // calc sample means
                    int index = indexes[n];

                    var data = dataset[index];
                    var label = labelset[index];

                    for (int j = 0; j < this.hSize; j++)
                    {
                        var sig_j = this._sigmoid(ref data, label, j) / (double)batch_size;

                        for (int i = 0; i < this.xSize; i++)
                        {
                            xh_data[i][j] += data[i] * sig_j;
                        }
                    }

                    for (int j = 0; j < this.hSize; j++)
                    {
                        h_data[j] += this._sigmoid(ref data, label, j) / (double)batch_size;
                        hy_data[j][label] += this._sigmoid(ref data, label, j) / (double)batch_size;
                    }

                    y_data[label] += 1.0 / (double)batch_size;

                    // calc DRBMSig expected values
                    var exp_xh = this._calcExpectedXH(ref data);
                    var exp_h = this._calcExpectedH(ref data);
                    var exp_hy = this._calcExpectedHY(ref data);
                    var exp_y = this._calcExpectedY(ref data);

                    for (int i = 0; i < this.xSize; i++)
                    {
                        for (int j = 0; j < this.hSize; j++)
                        {
                            xh_drbm[i][j] += exp_xh[i][j] / (double)batch_size;
                        }
                    }

                    for (int j = 0; j < this.hSize; j++)
                    {
                        h_drbm[j] += exp_h[j] / (double)batch_size;



                        for (int k = 0; k < this.ySize; k++)
                        {
                            hy_drbm[j][k] += exp_hy[j][k] / (double)batch_size;
                        }
                    }

                    for (int k = 0; k < this.ySize; k++)
                    {
                        y_drbm[k] += exp_y[k] / (double)batch_size;
                    }
                }

                // update params
                for (int i = 0; i < this.xSize; i++)
                {
                    for (int j = 0; j < this.hSize; j++)
                    {
                        var delta = 0.95 * mom_xh[i][j] + learning_rate * (xh_data[i][j] - xh_drbm[i][j]);

                        mom_xh[i][j] = delta;
                        this.param.w[i * param.hSize + j] += delta;
                    }
                }

                for (int j = 0; j < this.hSize; j++)
                {
                    var delta = 0.95 * mom_h[j] + learning_rate * (h_data[j] - h_drbm[j]);

                    mom_h[j] = delta;
                    this.param.c[j] += delta;

                    for (int k = 0; k < this.ySize; k++)
                    {
                        var delta2 = 0.95 * mom_hy[j][k] + learning_rate * (hy_data[j][k] - hy_drbm[j][k]);

                        mom_hy[j][k] = delta2;
                        this.param.v[j + param.ySize + k] += delta2;
                    }
                }

                for (int k = 0; k < this.ySize; k++)
                {
                    var delta = 0.95 * mom_y[k] + learning_rate * (y_data[k] - y_drbm[k]);

                    mom_y[k] = delta;
                    this.param.c[k] += delta;
                }
            }


            //学習回数加算
            param.trainCount++;

        }
    }
}
