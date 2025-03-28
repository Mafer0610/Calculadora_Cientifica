using System;
using System.IO;
using System.Windows.Forms;
using com.calitha.goldparser;

namespace Calculadora_Cientifica
{
    public partial class Calculadora : Form
    {
        private double memoria = 0;
        private AnalizadorGoldParser parser;
        public Calculadora()
        {
            InitializeComponent();
            parser = new AnalizadorGoldParser("gramatica.cgt");
        }
        private void btn9_Click(object sender, EventArgs e) { txtOpe.Text += "9"; }
        private void btn8_Click(object sender, EventArgs e) { txtOpe.Text += "8"; }
        private void btn7_Click(object sender, EventArgs e) { txtOpe.Text += "7"; }
        private void btn6_Click(object sender, EventArgs e) { txtOpe.Text += "6"; }
        private void btn5_Click(object sender, EventArgs e) { txtOpe.Text += "5"; }
        private void btn4_Click(object sender, EventArgs e) { txtOpe.Text += "4"; }
        private void btn3_Click(object sender, EventArgs e) { txtOpe.Text += "3"; }
        private void btn2_Click(object sender, EventArgs e) { txtOpe.Text += "2"; }
        private void btn1_Click(object sender, EventArgs e) { txtOpe.Text += "1"; }
        private void btn0_Click(object sender, EventArgs e) { txtOpe.Text += "0"; }
        private void btnPunto_Click(object sender, EventArgs e) { txtOpe.Text += "."; }
        private void btnPi_Click(object sender, EventArgs e) { txtOpe.Text += "3.14159"; }
        private void btnLParen_Click(object sender, EventArgs e) { txtOpe.Text += "("; }
        private void btnRParen_Click(object sender, EventArgs e) { txtOpe.Text += ")"; }
        private void btnC_Click(object sender, EventArgs e) { txtOpe.Text = ""; txtRespu.Text = ""; }
        private void btnCE_Click(object sender, EventArgs e) { if (txtOpe.Text.Length > 0) txtOpe.Text = txtOpe.Text.Substring(0, txtOpe.Text.Length - 1); }
        private void btnSIN_Click(object sender, EventArgs e) { txtOpe.Text += "sin"; }
        private void btnRaiz_Click(object sender, EventArgs e) { txtOpe.Text += "raiz"; }
        private void btnSuma_Click(object sender, EventArgs e) { txtOpe.Text += "+"; }
        private void btnResta_Click(object sender, EventArgs e) { txtOpe.Text += "-"; }
        private void btnMultiplica_Click(object sender, EventArgs e) { txtOpe.Text += "*"; }
        private void btnDividir_Click(object sender, EventArgs e) { txtOpe.Text += "/"; }
        private void btnM_Click(object sender, EventArgs e) { if (double.TryParse(txtRespu.Text, out double valorMemoria)) { memoria = valorMemoria; } }
        private void btnMR_Click(object sender, EventArgs e) { txtOpe.Text += memoria.ToString(); }

        private void btnIgual_Click(object sender, EventArgs e)
        {
            string expresion = txtOpe.Text;
            double resultado;

            if (parser.EvaluarExpresion(expresion, out resultado))
            {
                txtRespu.Text = "Resultado: " + resultado.ToString();
            }
            else
            {
                txtRespu.Text = "Error en la expresión.";
            }
        }
    }

    public class AnalizadorGoldParser
    {
        private ShiftReduceParser parser;

        public AnalizadorGoldParser(string archivoGramatica)
        {
            if (!File.Exists(archivoGramatica))
            {
                MessageBox.Show("No se encontró el archivo de gramática.");
                return;
            }

            parser = new ShiftReduceParser(new FileStream(archivoGramatica, FileMode.Open));
            parser.OnReduce += new ReduceHandler(AceptarEvento);
        }

        public bool EvaluarExpresion(string expresion, out double resultado)
        {
            parser.Parse(expresion);
            if (parser.CurrentReduction != null)
            {
                resultado = (double)parser.CurrentReduction.Value;
                return true;
            }

            resultado = 0;
            return false;
        }

        private void AceptarEvento(ReduceNode nodo)
        {
            switch (nodo.Rule.Index)
            {
                case 1: // Suma
                    nodo.Value = (double)nodo.Tokens[0].Value + (double)nodo.Tokens[2].Value;
                    break;
                case 2: // Resta
                    nodo.Value = (double)nodo.Tokens[0].Value - (double)nodo.Tokens[2].Value;
                    break;
                case 3: // Multiplicación
                    nodo.Value = (double)nodo.Tokens[0].Value * (double)nodo.Tokens[2].Value;
                    break;
                case 4: // División
                    nodo.Value = (double)nodo.Tokens[0].Value / (double)nodo.Tokens[2].Value;
                    break;
                case 5: // Paréntesis
                    nodo.Value = nodo.Tokens[1].Value;
                    break;
                case 6: // Número
                    nodo.Value = double.Parse(nodo.Tokens[0].Text);
                    break;
                default:
                    nodo.Value = 0;
                    break;
            }
        }
    }

}
