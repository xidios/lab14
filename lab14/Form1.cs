using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab14
{
    public partial class Form1 : Form
    {
        public List<double> values;
        public List<double> stats;


        double theory_chi = 11.070; 
        double mean;
        double variable;
        int N = 1;
        int m; // Count of intervals
        Random Rand = new Random();

        public Form1()
        {
            InitializeComponent();
            numericUpDown1.DecimalPlaces = 2;
            numericUpDown2.DecimalPlaces = 2;
        }

        double P(double x)
        {
            return Math.Pow(Math.E, -1.0 * (x - mean) * (x - mean) / (2.0 * variable)) / (Math.Sqrt(2 * Math.PI * variable));
        }

        void ProcessData()
        {



            N = (int)numericUpDown3.Value;

            mean = (double)numericUpDown1.Value;

            variable = (double)numericUpDown2.Value;
            m = (int)Math.Sqrt(N) + 1;

            values = new List<double>();

            double EmpM = 0;
            double EmpD = 0;

            double rvs;
            for (int i = 0; i < N; i++)
            {
                rvs = GeneratePsi();
                values.Add(rvs);
                EmpM += rvs;
                EmpD += rvs * rvs;
            }



            EmpM /= N;
            EmpD = EmpD / N - EmpM * EmpM;
            double minValue = values.Min();
            double maxValue = values.Max();

            stats = new List<double>();
            stats.AddRange(Enumerable.Repeat(0d, m));

            int k = (int)Math.Log(N, 2) + 1;

            for (int i = 0; i < N; i++)
            {
                double cur = values[i];
                int j = 0;
                while (cur >= minValue)
                {
                    cur -= (double)(maxValue - minValue) / m;
                    j++;
                }
                if (j > k)
                    j = k; 
                stats[j - 1] += (double)(1.0 / N);
            }

            double A = minValue;
            double step = (double)(maxValue - minValue) / m;
            double Chi_part0 = 0;

            for (int i = 0; i < m; i++)
            {
                Chi_part0 += stats[i] * stats[i] / (double)(N * step * P((A + step) / 2.0));
                A += step;
            }

            double Chi_squared_test = Chi_part0 - N;

            CountStats(EmpM, EmpD, Chi_squared_test);

        }


        double CountAbsoluteMeanError(double mean, double EmpM)
        {
            return Math.Abs(EmpM - mean);

        }

        double CountAbsoluteVarError(double DVar, double EmpVar)
        {
            return Math.Abs(EmpVar - DVar);
        }

        double CountRelativeMeanError(double mean, double EmpM)
        {
            return CountAbsoluteMeanError(mean, EmpM) / Math.Abs(mean);
        }

        double CountRelativeVarError(double DVar, double EmpVar)
        {
            return CountAbsoluteVarError(DVar, EmpVar) / Math.Abs(DVar);
        }

        void CountStats(double EmpMean, double EmpVar, double chiSquaredTest)
        {
            double rme = CountRelativeMeanError(mean, EmpMean) * 100;
            double rve = CountRelativeVarError(variable, EmpVar) * 100;
            int rme_i = (int)rme;
            int rve_i = (int)rve;

            label7.Text = EmpMean.ToString() + "(" + rme_i.ToString() + "%)";
            label8.Text = EmpVar.ToString() + "(" + rve_i.ToString() + "%)";

            label9.Text = chiSquaredTest.ToString() + " > " + theory_chi.ToString();
            bool result = !(chiSquaredTest > theory_chi);
            label10.Text = result.ToString();
        }

        double GeneratePsi()
        {
            double psi = Math.Sqrt(-2.0 * Math.Log(Rand.NextDouble())) * Math.Cos(2.0 * Math.PI * Rand.NextDouble());

            psi = psi * Math.Sqrt(variable) + mean;
            return psi;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ProcessData();
        }
    }
}
