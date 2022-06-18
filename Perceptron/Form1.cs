using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perceptron
{
    public partial class Form1 : Form
    {
        List<Data> dataList;
        int classCount;
        public Form1()
        {
            InitializeComponent();
            dataList = new List<Data>();
            double[] d1 = { 1, 2 };
            double[] d2 = { 2, 3 };
            double[] d3 = { -1, 0 };
            double[] d4 = { -1, -2 };
            double[] d5 = { 1, 0 };
            double[] d6 = { -1, 1 };
            dataList.Add(new Data(d1,1));
            dataList.Add(new Data(d2, 1));
            dataList.Add(new Data(d3, -1));
            dataList.Add(new Data(d4, -1));
            dataList.Add(new Data(d5, 1));
            dataList.Add(new Data(d6, -1));
            HashSet<int> classArray = new HashSet<int>();//aynı elemandan 2. eleman eklememizi sağlıyor hashet
            for (int i = 0; i < dataList.Count; i++)
            {
                classArray.Add(dataList[i].output);
            }

        }

        public abstract class Function
        {
            
            public double net(double[] input,double[] w,double bias)//toplamfonksiyonu
            {
                double sum = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    sum += input[i]*w[i];
                }
                return sum + (w[w.Length - 1] * bias);
            }

            public abstract double calculate(double net);
        }

        class Continue : Function
        {
            public override double calculate(double net)
            {
                return 1 / (1 + Math.Pow(Math.E, -net));
            }
        }
        class Binary : Function
        {
            public override double calculate(double net)
            {
                if (net > 0)
                {
                    return 1;
                }
                else
                {
                    return - 1;
                }
            }
        }

        class Data
        {
            public double[] input { get; set; }

            public int output { get; set; }
            public Data(double[] input,int output)
            {
                this.input = input;
                this.output = output;
            }
        }
        class Neuron
        {
            
            public double bias { get; set; }
            public double[] w { get; set; }

            public Function function { get; set; }

            
            public double getClass(int classNumber,int classIndex)
            {
                if (classNumber == classIndex)
                {
                    return 1;
                }
                return -1;
            }
            
            public String getW()
            {
                string data = "";

                for (int i = 0; i < w.Length; i++)
                {
                    data = data + " " + w[i];
                }
                return data;
            } 

            public Neuron(double bias,Function function,int dimension)
            {
                this.bias = bias;
                w = new double[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    w[i] = new Random().NextDouble(); 
                }
                this.function = function;
            }

        }
        // ax + by + c = 0    a b ve c ->w1 , w2 , w0 - bias  x y z -> girisler

        // deltaW = c*(d-o)*x  -> c- öğrenme katsayıdı d- sınıf etiketi   o- hesaplanan etiket değeri  x- veri girişi
        //wi = wi-1 + deltaW

        private void button1_Click(object sender, EventArgs e)
        {
            SingleLayerSingleNeuron(0.1, dataList[0].input.Length + 1, 1);

        }

        public void SingleLayerSingleNeuron(double c, int dimension, double bias)
        {
            Neuron neuron = new Neuron(bias,new Binary(),dimension);
             // c-öğrenme katsayısı
            while (true)//epoch sayısı
            {

                double error = 0;
                for (int i = 0; i < dataList.Count; i++)
                {
                    double net = neuron.function.net(dataList[i].input, neuron.w, neuron.bias);
                    double fnet = neuron.function.calculate(net);

                    for (int j = 0; j < dimension - 1; j++)
                    {
                        neuron.w[j] = neuron.w[j] + c * (dataList[i].output - fnet) * dataList[i].input[j];
                    }
                    neuron.w[dimension - 1] = neuron.w[dimension - 1] + c * (dataList[i].output - fnet) * neuron.bias;
                    error = error + Math.Pow(dataList[i].output - fnet,2)/2;
                   

                }
                
                if (error < 0.1)
                {


                    break;
                }

            }
            MessageBox.Show(neuron.getW());

        }
        public void SingleLayerMultiNeuron(double c, int dimension, double bias)
        {
            List<Neuron> neuronList = new List<Neuron>();
            for (int i = 0; i < classCount; i++)
            {
                neuronList.Add(new Neuron(bias, new Binary(), dimension));
            }
            while (true)
            {
                double error = 0;
                for (int i = 0; i < dataList.Count; i++)
                {
                    for (int j = 0; j < classCount; j++)
                    {
                        Neuron neuron = neuronList[j];
                        double net = neuron.function.net(dataList[i].input, neuron.w, neuron.bias);
                        double fnet = neuron.function.calculate(net);


                        for (int k = 0; k < dimension - 1; k++)
                        {
                            neuron.w[k] = neuron.w[k] + c * (neuron.getClass(dataList[i].output, j) - fnet) * dataList[i].input[k];
                        }
                        neuron.w[dimension - 1] = neuron.w[dimension - 1] + c * (neuron.getClass(dataList[i].output, j) - fnet) * neuron.bias;
                        error = error + Math.Pow(neuron.getClass(dataList[i].output, j)- fnet, 2)/2;
                    }
                }
            }
        }
    }
}
