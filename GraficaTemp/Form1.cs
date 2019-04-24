using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GraficaTemp
{
    public partial class Temperatura : Form
    {

        System.IO.Ports.SerialPort puerto;
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();
        string datos_puerto;
        double serie1 = 0.0, serie2 = 0.0;
        double tiempo = 0.0;
        int x = 0;
        int y = 0;
        string Xg;
        string Yg;
        System.Drawing.Rectangle r1 = new System.Drawing.Rectangle();
        public Temperatura()
        {
            InitializeComponent();
            this.comboBox1.DataSource = listado_puerto; // listado de puertos

        }

        public void serial()
        {

            try
            {
                this.puerto = new System.IO.Ports.SerialPort("" + comboBox1.SelectedItem, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                this.puerto.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(recepcion);

            }
            catch (Exception)
            {
                MessageBox.Show("Verifique:" + System.Environment.NewLine + "- Voltage" + System.Environment.NewLine + "- Conexion del puerto", "Error de puerto COM", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public void recepcion(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(10);
                datos_puerto = this.puerto.ReadLine();
                Console.WriteLine(datos_puerto);
                if (datos_puerto.StartsWith("$"))
                { this.Invoke(new EventHandler(actualizar)); }

            }

            catch (Exception) { }


        }

     
   

        private void timer1_Tick(object sender, EventArgs e)
        {
            tiempo++;
            chart2.Series[0].Points.AddXY((tiempo/2), serie1);
            textBox1.Text = ""+ serie2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                serial();
                puerto.Open();
                timer1.Start();
            }
            catch (Exception)
            { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            puerto.Close();
            timer1.Stop();
        }

        private void chart2_PostPaint(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            System.Drawing.Font drawFont = new System.Drawing.Font("Verdana", 8);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            if (!(string.IsNullOrEmpty(Xg) && string.IsNullOrEmpty(Yg)))
            {
                e.ChartGraphics.Graphics.DrawString("ELong=" + Xg + "\n" + "Fuerza=" + Yg, drawFont, drawBrush, x + 5, y - 2);
                e.ChartGraphics.Graphics.DrawRectangle(new Pen(Color.Red, 3), r1);
            }
        }

        private void chart2_MouseMove(object sender, MouseEventArgs e)
        {
            var pos1 = e.Location;
            if (prevPosition.HasValue && pos1 == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos1;
            var results1 = chart2.HitTest(pos1.X, pos1.Y, false,
                                            ChartElementType.DataPoint);
            foreach (var result1 in results1)
            {
                if (result1.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop1 = result1.Object as DataPoint;
                    if (prop1 != null)
                    {
                        var pointXPixel = result1.ChartArea.AxisX.ValueToPixelPosition(prop1.XValue);
                        var pointYPixel = result1.ChartArea.AxisY.ValueToPixelPosition(prop1.YValues[0]);

                        // check if the cursor is really close to the point (2 pixels around)

                        tooltip.Show("Tiempo=" + prop1.XValue + ", Fuerza=" + prop1.YValues[0], this.chart2,
                                        pos1.X, pos1.Y - 15);

                    }
                }
            }
        }
        System.Drawing.Point? prevPosition = null;
        ToolTip tooltip = new ToolTip();
        private void chart2_MouseDown(object sender, MouseEventArgs e)
        {
            chart2.Invalidate();
            r1.X = x;
            r1.Y = y;
            r1.Width = 3;
            r1.Height = 3;
        }

        public void actualizar(object s, EventArgs e)
        {
            datos_puerto = datos_puerto.Remove(0, 1);
            string[] arreglo = datos_puerto.Split(';');
            serie1 = Convert.ToDouble(arreglo[0].Replace(".", ","));
            serie2 = Convert.ToDouble(arreglo[1].Replace(".", ","));
        }






    }
}
