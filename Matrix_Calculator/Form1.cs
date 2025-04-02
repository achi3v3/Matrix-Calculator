using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matrix_Calculator
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
            Init_Grid(Grid_Matrix_A, panel_For_Grid_A);
            Init_Grid(Grid_Matrix_B, panel_For_Grid_B);
            Init_Grid(Grid_Result, panel_For_Result);

            Set_Start_Position();

            panel_History.Height = 0;
            rtb_HistoryBox.Text = "History: \n";

            label_operation_AB.Text = "OPERATIONS\nA ? B";

            comboBox_Choose_Matrix_Operation.Text = "MATRIX A";
            comboBox_Choose_Matrix_Fill.Text = "MATRIX A";

            radioButton_Integer.Checked = true;
            radioButton_Integer.ForeColor = Color.FromArgb(255, 133, 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /*————————————————RADIOBUTTONS————————————————*/
        private void Integer_Checked(object sender, EventArgs e)
        {
            if (radioButton_Integer.Checked)
                radioButton_Integer.ForeColor = Color.FromArgb(255,133,0);
            else
                radioButton_Integer.ForeColor = Color.White;
        }
        private void Float_Checked(object sender, EventArgs e)
        {
            if (radioButton_Float.Checked)
                radioButton_Float.ForeColor = Color.FromArgb(255, 133, 0);
            else
                radioButton_Float.ForeColor = Color.White;
        }

        /*————————————————VALIDATE INPUT————————————————*/
        private void textBox_Degree_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox_Degree.Text, out int degree))
            {
                if (degree > 20 || degree < -20)
                {
                    Message_Error("Степень должна быть в диапазоне от -20 до 20");
                    textBox_Degree.Text = "0";
                    return;

                }
            }
            textBox_TextChanged(sender, e);

        }
        private void textBox_Multiplier_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBox_Multiplier.Text, out double multiplier))
            {
                if (Math.Abs(multiplier) > 1e6)
                {
                    Message_Error("Множитель слишком большой. Максимальное значение: 1 000 000");
                    textBox_Multiplier.Text = "0";
                    return;
                }
            }
            textBox_TextChanged(sender, e);
        }
        private void textBox_Min_Value_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBox_Fill_Min.Text, out double max))
            {
                if (Math.Abs(max) > 1e6)
                {
                    Message_Error("Значение слишком большое. Максимальное значение: 1 000 000");
                    textBox_Fill_Min.Text = "0";
                    return;
                }
            }
            textBox_TextChanged(sender, e);
        }
        private void textBox_Max_Value_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBox_Fill_Max.Text, out double min))
            {
                if (Math.Abs(min) > 1e6)
                {
                    Message_Error("Значение слишком большое. Максимальное значение: 1 000 000");
                    textBox_Fill_Max.Text = "0";
                    return;
                }
            }
            textBox_TextChanged(sender, e);
        }


        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text;
            int selectionStart = textBox.SelectionStart;
            int selectionLength = textBox.SelectionLength;

            if (!char.IsDigit(e.KeyChar) &&
                e.KeyChar != (char)Keys.Back &&
                !(e.KeyChar == '-' && selectionStart == 0 && !currentText.Contains("-")) &&
                !(e.KeyChar == ',' && !currentText.Contains(",") && currentText.Length > 0))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == ',' &&
                (selectionStart == 0 ||
                 (currentText == "-" && selectionStart == 1) ||
                 (selectionStart == 1 && currentText.StartsWith("-"))))
            {
                e.Handled = true;
                return;
            }
        }
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            int cursorPos = textBox.SelectionStart;

            if (string.IsNullOrEmpty(text) || text == "-")
            {
                textBox.Text = "0";
                textBox.SelectAll();
                return;
            }

            if (text.Length > 1 && !text.Contains(","))
            {
                if (text[0] == '0' && text != "0")
                {
                    string cleaned = text.TrimStart('0');
                    textBox.Text = cleaned == "" ? "0" : cleaned;
                    textBox.SelectionStart = Math.Min(cursorPos, textBox.Text.Length);
                    return;
                }

                if (text.StartsWith("-0") && text != "-0")
                {
                    string cleaned = "-" + text.Substring(2).TrimStart('0');
                    textBox.Text = cleaned == "-" ? "-0" : cleaned;
                    textBox.SelectionStart = Math.Min(cursorPos, textBox.Text.Length);
                    return;
                }
            }

            if (text.Contains(","))
            {
                if (text.StartsWith(","))
                {
                    textBox.Text = "0" + text;
                    textBox.SelectionStart = Math.Min(cursorPos + 1, textBox.Text.Length);
                    return;
                }

                if (text.StartsWith("-,"))
                {
                    textBox.Text = "-0" + text.Substring(1);
                    textBox.SelectionStart = Math.Min(cursorPos + 1, textBox.Text.Length);
                    return;
                }

                if (text.Count(c => c == ',') > 1)
                {
                    int firstComma = text.IndexOf(',');
                    string cleaned = text.Substring(0, firstComma + 1) +
                                    text.Substring(firstComma + 1).Replace(",", "");
                    textBox.Text = cleaned;
                    textBox.SelectionStart = Math.Min(cursorPos, cleaned.Length);
                    return;
                }
            }
        }
        private void textBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;

            if (string.IsNullOrEmpty(text))
            {
                textBox.Text = "0";
            }
            else if (text == "-")
            {
                textBox.Text = "-0";
            }
            else if (text.EndsWith(","))
            {
                textBox.Text = text + "0";
            }
            else if (text.StartsWith(","))
            {
                textBox.Text = "0" + text;
            }
            else if (text.StartsWith("-,"))
            {
                textBox.Text = "-0" + text.Substring(1);
            }
        }

        private void Grid_A_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Validate_Cell_Input(Grid_Matrix_A, e);
        }
        private void Grid_B_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Validate_Cell_Input(Grid_Matrix_B, e);
        }
        private void Validate_Cell_Input(DataGridView dgv, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string text = cell.Value?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(text) || !IsValidNumber(text))
            {
                cell.Value = "0";
                return;
            }

            string normalized = NormalizeNumber(text);
            cell.Value = normalized;
        }

        private bool IsValidNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            bool hasComma = false;
            bool hasMinus = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (char.IsDigit(c)) continue;

                if (c == '-' && i == 0 && !hasMinus)
                {
                    hasMinus = true;
                    continue;
                }

                if (c == ',' && !hasComma)
                {
                    hasComma = true;
                    continue;
                }

                return false;
            }

            if (text == "-" || text == "," || text == "-,") return false;

            return true;
        }
        private string NormalizeNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "0";

            text = text.Replace(" ", "");

            if (text.StartsWith("-0") && (text.Length == 2 || (text.Length > 2 && text[2] == ',')))
            {
                if (text.Length == 2) return "0";
                return "-0," + text.Substring(3).TrimEnd('0');
            }

            if (text.StartsWith("0,"))
            {
                string decimalPart = text.Substring(2).TrimEnd('0');
                return "0" + (decimalPart.Length > 0 ? "," + decimalPart : "");
            }

            if (text.StartsWith("-") && text.Length > 1 && text[1] != ',')
            {
                string numPart = text.Substring(1);
                if (numPart.Contains(","))
                {
                    string[] parts = numPart.Split(',');
                    string intPart = parts[0].TrimStart('0');
                    if (string.IsNullOrEmpty(intPart)) intPart = "0";

                    string decimalPart = parts.Length > 1 ? parts[1].TrimEnd('0') : "";

                    return "-" + intPart + (decimalPart.Length > 0 ? "," + decimalPart : "");
                }
                else
                {
                    string intPart = numPart.TrimStart('0');
                    return "-" + (string.IsNullOrEmpty(intPart) ? "0" : intPart);
                }
            }

            if (text.Contains(","))
            {
                string[] parts = text.Split(',');
                string intPart = parts[0].TrimStart('0');
                if (string.IsNullOrEmpty(intPart)) intPart = "0";

                string decimalPart = parts.Length > 1 ? parts[1].TrimEnd('0') : "";

                return intPart + (decimalPart.Length > 0 ? "," + decimalPart : "");
            }

            string trimmed = text.TrimStart('0');
            return string.IsNullOrEmpty(trimmed) ? "0" : trimmed;
        }
        private void Grid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                tb.KeyPress -= tb_KeyPress;
                tb.TextChanged -= tb_TextChanged;

                tb.KeyPress += tb_KeyPress;
                tb.TextChanged += tb_TextChanged;
            }
        }

        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string currentText = tb.Text;
            int pos = tb.SelectionStart;

            if (!char.IsDigit(e.KeyChar)
                && e.KeyChar != (char)Keys.Back
                && !(e.KeyChar == '-' && pos == 0)
                && !(e.KeyChar == ',' && !currentText.Contains(',')))
            {
                e.Handled = true;
                return;
            }
        }
        private void tb_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string text = tb.Text;
            int pos = tb.SelectionStart;

            // Автоматическая коррекция ввода
            if (text == "-") return; // Разрешаем ввод минуса

            if (text.StartsWith("-0") && text.Length > 2 && text[2] != ',')
            {
                string newText = "-" + text.Substring(2).TrimStart('0');
                if (newText == "-") newText = "0";

                tb.Text = newText;
                tb.SelectionStart = newText.Length;
            }
            else if (text.StartsWith("0") && text.Length > 1 && text[1] != ',')
            {
                string newText = text.TrimStart('0');
                if (string.IsNullOrEmpty(newText)) newText = "0";

                tb.Text = newText;
                tb.SelectionStart = newText.Length;
            }
        }
        private void Grid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            // Альтернативная валидация при уходе с ячейки
            var grid = (DataGridView)sender;
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string text = cell.Value?.ToString() ?? string.Empty;

                if (!IsValidNumber(text))
                {
                    cell.Value = "0";
                }
                else
                {
                    string normalized = NormalizeNumber(text);
                    if (normalized != text)
                    {
                        cell.Value = normalized;
                    }
                }
            }
        }

        /*————————————————HISTORY PANEL————————————————*/
        private void Click_Open_History(object sender, EventArgs e)
        {
            if (panel_History.Height < 100)
            {
                button_History.BackColor = Color.Green;
                panel_History.Height = 380;

                Change_Status_Bar("HISTORY OPENED");
            } else
            {
                button_History.BackColor = Color.FromArgb(255, 133, 0);
                panel_History.Height = 0;
             
                Change_Status_Bar("HISTORY CLOSED");
            }
        }
        private void Click_Clean_History(object sender, EventArgs e)
        {
            rtb_HistoryBox.Text = "History: \n";

            Change_Status_Bar("HISTORY CLEANED");
        }
        private void Add_To_History(string operation, DataGridView matrixA = null, DataGridView matrixB = null, DataGridView result = null)
        {
            rtb_HistoryBox.SelectionStart = rtb_HistoryBox.TextLength;

            rtb_HistoryBox.SelectionColor = Color.LightSkyBlue;
            rtb_HistoryBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {operation}\n");

            if (matrixA != null)
            {
                rtb_HistoryBox.SelectionColor = Color.White;
                rtb_HistoryBox.AppendText("Матрица A:\n");
                rtb_HistoryBox.AppendText(MatrixToString(matrixA));
            }

            if (matrixB != null)
            {
                rtb_HistoryBox.SelectionColor = Color.White;
                rtb_HistoryBox.AppendText("Матрица B:\n");
                rtb_HistoryBox.AppendText(MatrixToString(matrixB));
            }

            if (result != null)
            {
                rtb_HistoryBox.SelectionColor = Color.LightGreen;
                rtb_HistoryBox.AppendText("Результат:\n");
                rtb_HistoryBox.AppendText(MatrixToString(result));
            }

            rtb_HistoryBox.AppendText("\n");
            rtb_HistoryBox.ScrollToCaret();
        }
        private string MatrixToString(DataGridView dgv)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dgv.RowCount; i++)
            {
                sb.Append("  ");
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    string value = dgv.Rows[i].Cells[j].Value?.ToString() ?? "0";
                    if (double.TryParse(value, out double num))
                    {
                        if (double.IsInfinity(num))
                        {
                            value = "∞";
                        }
                        else if (double.IsNaN(num))
                        {
                            value = "NaN";
                        }
                        else if (Math.Abs(num) >= 1e15)
                        {
                            value = num.ToString("0.###e+0");
                        }
                        else if (Math.Abs(num) >= 1e6)
                        {
                            value = num.ToString("#,###");
                        }
                        else if (value.Contains(",") || value.Contains("."))
                        {
                            value = Math.Round(num, 4).ToString();
                        }
                    }

                    sb.Append(value.PadLeft(15));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        
        /*————————————————STATUS BAR — MESSAGES————————————————*/
        private void Change_Status_Bar(string status)
        {
            label_Change_StatusBar.Text = status;
        }
        private void Message_Error(string s)
        {
            MessageBox.Show(s, "Error");
        }
        private DataGridView Get_Active_Grid(ComboBox box)
        {
            if (box.Text == "MATRIX B")
            {
                return Grid_Matrix_B;
            } else 
            {
                return Grid_Matrix_A;
            } 
        }

        /*————————————————X BUTTONS SIZE————————————————*/
        private void Click_A_X(object sender, EventArgs e)
        {
            comboBox_A_Y.SelectedIndex = comboBox_A_X.SelectedIndex;
            Change_Status_Bar("MATRIX A: RESIZED Z×Z");
        }
        private void Click_B_X(object sender, EventArgs e)
        {
            comboBox_B_Y.SelectedIndex = comboBox_B_X.SelectedIndex;
            Change_Status_Bar("MATRIX B: RESIZED Z×Z");
        }

        /*————————————————COPY RESULT TO MATRICES————————————————*/
        private void Click_Copy_Result_To_Matrix_A(object sender, EventArgs e)
        {
            Copy_Result_To_Matrix(Grid_Matrix_A, panel_For_Grid_A, comboBox_A_X, comboBox_A_Y);
            Change_Status_Bar($"RESULT MATRIX WAS COPIED AT MATRIX A");

        }
        private void Click_Copy_Result_To_Matrix_B(object sender, EventArgs e)
        {
            Copy_Result_To_Matrix(Grid_Matrix_B, panel_For_Grid_B, comboBox_B_X, comboBox_B_Y);
            Change_Status_Bar($"RESULT MATRIX WAS COPIED AT MATRIX B");

        }
        private void Copy_Result_To_Matrix(DataGridView targetMatrix, Panel panel, ComboBox comboBox_X, ComboBox comboBox_Y)
        {
            if (Grid_Result.RowCount == 0 || Grid_Result.ColumnCount == 0)
            {
                Message_Error("Нет данных для копирования!");
                return;
            }

            Set_Matrix_Size(targetMatrix, panel, Grid_Result.RowCount, Grid_Result.ColumnCount);

            comboBox_X.Text = Grid_Result.RowCount.ToString();
            comboBox_Y.Text = Grid_Result.ColumnCount.ToString();


            targetMatrix.RowCount = Grid_Result.RowCount;
            targetMatrix.ColumnCount = Grid_Result.ColumnCount;
            if (targetMatrix.RowCount != Grid_Result.RowCount || targetMatrix.ColumnCount != Grid_Result.ColumnCount)
            {
                var result = MessageBox.Show("Размерности не совпадают. Изменить размер целевой матрицы?",
                                           "Подтверждение",
                                           MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes) return;
            }
            for (int i = 0; i < Grid_Result.RowCount; i++)
            {
                for (int j = 0; j < Grid_Result.ColumnCount; j++)
                {
                    targetMatrix.Rows[i].Cells[j].Value = Grid_Result.Rows[i].Cells[j].Value;
                }
            }
        }
        
        /*————————————————SWITCH MATRICES————————————————*/
        private void Click_Swap_Matrices(object sender, EventArgs e)
        {
            Swap_Matrices(Grid_Matrix_A, Grid_Matrix_B);
            Change_Status_Bar($"MATRICES A||B: SWITCHED");
        }
        private void Swap_Matrices(DataGridView matrixA, DataGridView matrixB)
        {
            if (matrixA.RowCount != matrixB.RowCount || matrixA.ColumnCount != matrixB.ColumnCount)
            {
                MessageBox.Show("Матрицы должны иметь одинаковые размеры для обмена!");
                return;
            }
            double[,] tempValues = new double[matrixA.RowCount, matrixA.ColumnCount];

            for (int i = 0; i < matrixA.RowCount; i++)
            {
                for (int j = 0; j < matrixA.ColumnCount; j++)
                {
                    tempValues[i, j] = Convert.ToDouble(matrixA.Rows[i].Cells[j].Value);
                }
            }

            for (int i = 0; i < matrixB.RowCount; i++)
            {
                for (int j = 0; j < matrixB.ColumnCount; j++)
                {
                    matrixA.Rows[i].Cells[j].Value = matrixB.Rows[i].Cells[j].Value;
                }
            }

            for (int i = 0; i < matrixA.RowCount; i++)
            {
                for (int j = 0; j < matrixA.ColumnCount; j++)
                {
                    matrixB.Rows[i].Cells[j].Value = tempValues[i, j];
                }
            }
        }

        /*————————————————AMOUNT OR SUBTRACT MATRICES————————————————*/
        private void Click_Amount(object sender, EventArgs e)
        {
            Amount_Or_Subtract(sender, e, 1);
        }
        private void Click_Subtract(object sender, EventArgs e)
        {
            Amount_Or_Subtract(sender, e, -1);
        }

        private void Amount_Or_Subtract(object sender, EventArgs e, int flag)
        {
            if (Grid_Matrix_A.RowCount == Grid_Matrix_B.RowCount && Grid_Matrix_A.ColumnCount == Grid_Matrix_B.ColumnCount)
            {
                Set_Matrix_Size(Grid_Result, panel_For_Result, Grid_Matrix_A.ColumnCount, Grid_Matrix_A.RowCount);

                for (int i = 0; i < Grid_Matrix_A.RowCount; i++)
                {
                    for (int j = 0; j < Grid_Matrix_A.ColumnCount; j++)
                    {
                        double valA = Convert.ToDouble(Grid_Matrix_A.Rows[i].Cells[j].Value);
                        double valB = Convert.ToDouble(Grid_Matrix_B.Rows[i].Cells[j].Value);
                        Grid_Result.Rows[i].Cells[j].Value = (valA + valB*flag).ToString();
                    }
                }
                if (flag == 1) {
                    Change_Status_Bar("MATRIX A AMOUNT MATRIX B");
                    Add_To_History("Сложение матриц", Grid_Matrix_A, Grid_Matrix_B, Grid_Result);

                } else {
                    Change_Status_Bar("MATRIX A SUBTRACT MATRIX B");
                    Add_To_History("Вычетание матриц", Grid_Matrix_A, Grid_Matrix_B, Grid_Result);
                }
            }
            else
            {
                Message_Error("Матрицы должны иметь одинаковые размерности для сложения.");
            }
        }

        /*————————————————MULTIPLY MATRIX ON MULTIPLIER————————————————*/
        private void Click_Multiply_On(object sender, EventArgs e)
        {
            Multiply_On_Multiplier(sender, e);
        }
        private void Multiply_On_Multiplier(object sender, EventArgs e)
        {

            DataGridView Grid = Get_Active_Grid(comboBox_Choose_Matrix_Operation);
            if ((textBox_Multiplier.Text != string.Empty) && (textBox_Multiplier.Text != "-"))
            {
                Set_Matrix_Size(Grid_Result, panel_For_Result, Grid.ColumnCount, Grid.RowCount);

                for (int i = 0; i < Grid.RowCount; i++)
                    for (int j = 0; j < Grid.ColumnCount; j++)
                        Grid_Result.Rows[i].Cells[j].Value = Convert.ToString((Convert.ToDouble(Grid.Rows[i].Cells[j].Value)) * Convert.ToDouble(textBox_Multiplier.Text));
                
                Add_To_History($"Умножение {comboBox_Choose_Matrix_Operation.Text} на {textBox_Multiplier.Text}", Grid, null, Grid_Result);
                Change_Status_Bar($"{comboBox_Choose_Matrix_Operation.Text} MULTIPLIED ON {textBox_Multiplier.Text}");
            }
            else
                MessageBox.Show("Поле ввода множителя пустое!", "Attention");
        }

        /*————————————————REVERSE MATRIX————————————————*/
        private void Click_Reverse(object sender, EventArgs e)
        {
            //DataGridView Grid = Get_Active_Grid(comboBox_Choose_Matrix_Operation);
            Reverse_Matrix(sender, e);
            //Matrix_Power(Grid, -1);
        }
        private void Reverse_Matrix(object sender, EventArgs e)
        {

            int K = 0, F;
            double temp;
            bool check;
            DataGridView Grid = Get_Active_Grid(comboBox_Choose_Matrix_Operation);
            double[,] Mat = new double[Grid.RowCount, Grid.ColumnCount * 2];


            if (Grid.RowCount == Grid.ColumnCount)
            {
                if (Determinator() == 0)
                {
                    Message_Error("Dererminator равен нулю.\nПодсчитать обратную матрицу невозможно");
                    return;
                }

                else
                {
                    Set_Matrix_Size(Grid_Result, panel_For_Result, Grid.ColumnCount, Grid.RowCount);

                    Grid_Result.RowCount = Grid.RowCount;
                    Grid_Result.ColumnCount = Grid.RowCount;

                    for (int i = 0; i < Grid.RowCount; i++)
                    {
                        for (int j = 0; j < Grid.ColumnCount * 2 / 2; j++)
                        {
                            Mat[i, j] = Convert.ToDouble(Grid.Rows[i].Cells[j].Value);
                        }
                        Mat[i, Grid.ColumnCount * 2 / 2 + K] = 1;
                        K++;
                    }
                    for (int i = 0; i < Grid.RowCount; i++)
                    {
                        if (Mat[i, i] == 0)
                        {
                            F = i + 1;
                            check = true;
                            while (F != Grid.RowCount && check == true)
                            {
                                if (Mat[F, i] == 0) F++;
                                else check = false;
                            }
                            if (F != Grid.RowCount)
                            {
                                for (K = i; K < Grid.ColumnCount * 2; K++)
                                {
                                    temp = Mat[i, K];
                                    Mat[i, K] = Mat[F, K];
                                    Mat[F, K] = temp;
                                }
                            }
                        }
                        temp = Mat[i, i];
                        for (int j = i; j < Grid.ColumnCount * 2; j++)
                        {
                            Mat[i, j] = Mat[i, j] / temp;
                        }
                        for (K = 0; K < Grid.RowCount; K++)
                        {
                            if (K != i)
                            {
                                temp = Mat[K, i];
                                for (int j = i; j < Grid.ColumnCount * 2; j++)
                                {
                                    Mat[K, j] -= temp * Mat[i, j];
                                }
                            }
                        }
                    }
                    for (int i = 0; i < Grid.RowCount; i++)
                        for (int j = 0; j < Grid.ColumnCount * 2 / 2; j++)
                        {
                            Grid_Result.Rows[i].Cells[j].Value = Mat[i, j + Grid.ColumnCount * 2 / 2];
                        }
                }
            }
            else
            {
                Message_Error("Размерность матрицы должна быть равной ZxZ.");
            }

            Change_Status_Bar($"{comboBox_Choose_Matrix_Operation.Text}: REVERSED");
            Add_To_History("REVERSE MATRIX:", Grid, null, Grid_Result);
        }

        /*————————————————MULTIPLY MATRICESMATRIX————————————————*/
        private void Click_Multiply_Matrices(object sender, EventArgs e)
        {
            Multiply_Matrices(Grid_Matrix_A, Grid_Matrix_B);
        }
        private void Multiply_Matrices(DataGridView Matrix_A, DataGridView Matrix_B)
        {
            if (Matrix_A.ColumnCount != Matrix_B.RowCount)
            {
                Message_Error("Количество столбцов матрицы MATRIX A не равно количеству строк матрицы MATRIX B.");
                return;
            }

            Set_Matrix_Size(Grid_Result, panel_For_Result, Matrix_B.ColumnCount, Matrix_A.RowCount);

            while (Grid_Result.RowCount < Matrix_A.RowCount)
            {
                Grid_Result.Rows.Add();
            }
            while (Grid_Result.ColumnCount < Matrix_B.ColumnCount)
            {
                Grid_Result.Columns.Add("", "");
            }

            for (int i = 0; i < Matrix_A.RowCount; i++)
            {
                if (i >= Grid_Result.RowCount)
                {
                    Grid_Result.Rows.Add();
                }

                for (int j = 0; j < Matrix_B.ColumnCount; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < Matrix_A.ColumnCount; k++)
                    {
                        double val1 = Convert.ToDouble(Matrix_A.Rows[i].Cells[k].Value ?? "0");
                        double val2 = Convert.ToDouble(Matrix_B.Rows[k].Cells[j].Value ?? "0");
                        sum += val1 * val2;
                    }

                    if (j >= Grid_Result.ColumnCount)
                    {
                        Grid_Result.Columns.Add("", "");
                    }

                    Grid_Result.Rows[i].Cells[j].Value = sum.ToString("0.###");
                }
            }

            Add_To_History("Умножение матриц:", Matrix_A, Matrix_B, Grid_Result);
            Change_Status_Bar("MATRIX A MULTIPLIED ON MATRIX B");
        }
        
        /*————————————————RAIZE TO POWER————————————————*/
        private void Click_Raise_Power(object sender, EventArgs e)
        {
            DataGridView Grid = Get_Active_Grid(comboBox_Choose_Matrix_Operation);
            if (string.IsNullOrEmpty(textBox_Degree.Text))
            {
                Message_Error("ERROR: Введите степень!");
                return;
            }

            if (double.TryParse(textBox_Degree.Text, out double power))
            {
                if (Math.Abs(power) > 10)
                {
                    var result = MessageBox.Show($"Вы действительно хотите возвести матрицу в степень {power}? Это может занять много времени и привести к большим числам.",
                                               "Attention",
                                               MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes) return;
                }

                Matrix_Power(Grid, power);
                Add_To_History($"Матрица возведена в степень {power}", Grid, null, Grid_Result);
                Change_Status_Bar($"MATRIX RAISED TO POWER {power}");
            }
            else
            {
                Message_Error("ERROR: Степень должна быть числом (например, 2 или -1)");
            }
        }
        private void Matrix_Power(DataGridView matrix, double power)
        {
            if (matrix.RowCount != matrix.ColumnCount)
            {
                Message_Error("Матрица должна быть квадратной для возведения в степень!");
                return;
            }

            DataGridView tempGrid = new DataGridView();
            tempGrid.RowCount = matrix.RowCount;
            tempGrid.ColumnCount = matrix.ColumnCount;

            if (power == 0)
            {
                Set_Matrix_Size(Grid_Result, panel_For_Result, matrix.ColumnCount, matrix.RowCount);
                for (int i = 0; i < matrix.RowCount; i++)
                {
                    for (int j = 0; j < matrix.ColumnCount; j++)
                    {
                        Grid_Result.Rows[i].Cells[j].Value = (i == j) ? "1" : "0";
                    }
                }
                return;
            }

            if (power < 0)
            {
                Reverse_Matrix(null, EventArgs.Empty);

                for (int i = 0; i < Grid_Result.RowCount; i++)
                {
                    for (int j = 0; j < Grid_Result.ColumnCount; j++)
                    {
                        tempGrid.Rows[i].Cells[j].Value = Grid_Result.Rows[i].Cells[j].Value;
                    }
                }

                Matrix_Power(tempGrid, -power);
                return;
            }

            if (power % 1 != 0)
            {
                MessageBox.Show("Дробные степени для матриц не поддерживаются.", "Attention");
                return;
            }

            int intPower = (int)power;

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    tempGrid.Rows[i].Cells[j].Value = matrix.Rows[i].Cells[j].Value;
                }
            }

            if (intPower == 1) // nothing changes
            {
                Set_Matrix_Size(Grid_Result, panel_For_Result, tempGrid.ColumnCount, tempGrid.RowCount);
                for (int i = 0; i < tempGrid.RowCount; i++)
                {
                    for (int j = 0; j < tempGrid.ColumnCount; j++)
                    {
                        Grid_Result.Rows[i].Cells[j].Value = tempGrid.Rows[i].Cells[j].Value;
                    }
                }
                return;
            }

            DataGridView result = new DataGridView();
            result.RowCount = matrix.RowCount;
            result.ColumnCount = matrix.ColumnCount;

            for (int i = 0; i < matrix.RowCount; i++) // copy
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    result.Rows[i].Cells[j].Value = matrix.Rows[i].Cells[j].Value;
                }
            }

            for (int p = 1; p < intPower; p++) // fill
            {
                Multiply_Matrices(result, tempGrid);

                for (int i = 0; i < Grid_Result.RowCount; i++)
                {
                    for (int j = 0; j < Grid_Result.ColumnCount; j++)
                    {
                        result.Rows[i].Cells[j].Value = Grid_Result.Rows[i].Cells[j].Value;
                    }
                }
            }

            Set_Matrix_Size(Grid_Result, panel_For_Result, result.ColumnCount, result.RowCount);
            for (int i = 0; i < result.RowCount; i++)
            {
                for (int j = 0; j < result.ColumnCount; j++)
                {
                    Grid_Result.Rows[i].Cells[j].Value = result.Rows[i].Cells[j].Value;
                }
            }
        }
        private void Click_Check_Inverse(object sender, EventArgs e)
        {
            Check_Inverse_Matrix(sender, e);
        }
        private void Check_Inverse_Matrix(object sender, EventArgs e)
        {
            DataGridView originalGrid = Get_Active_Grid(comboBox_Choose_Matrix_Operation);

            if (Determinator() == 0)
            {
                Message_Error("Dererminator равен нулю.\nПодсчитать обратную матрицу невозможно");
                return;
            }

            if (originalGrid.RowCount != originalGrid.ColumnCount)
            {
                Message_Error("Матрица должна быть квадратной для проверки обратной матрицы!");
                return;
            }

            Reverse_Matrix(null, EventArgs.Empty);

            DataGridView inverseMatrix = new DataGridView();
            inverseMatrix.RowCount = Grid_Result.RowCount;
            inverseMatrix.ColumnCount = Grid_Result.ColumnCount;

            for (int i = 0; i < Grid_Result.RowCount; i++)
            {
                for (int j = 0; j < Grid_Result.ColumnCount; j++)
                {
                    inverseMatrix.Rows[i].Cells[j].Value = Grid_Result.Rows[i].Cells[j].Value;
                }
            }

            Multiply_Matrices(originalGrid, inverseMatrix);

            double tolerance = 1e-6;
            bool isIdentity = true;

            for (int i = 0; i < Grid_Result.RowCount; i++)
            {
                for (int j = 0; j < Grid_Result.ColumnCount; j++)
                {
                    double value = Convert.ToDouble(Grid_Result.Rows[i].Cells[j].Value);

                    if (i == j)
                    {
                        if (Math.Abs(value - 1) > tolerance)
                        {
                            isIdentity = false;
                            break;
                        }
                    }
                    else
                    {
                        if (Math.Abs(value) > tolerance)
                        {
                            isIdentity = false;
                            break;
                        }
                    }
                }
                if (!isIdentity) break;
            }

            if (isIdentity)
            {
                MessageBox.Show("Проверка пройдена: A × A⁻¹ = I (с точностью до 0.000001)", "Success");
            }
            else
            {
                Message_Error("Ошибка: A × A⁻¹ ≠ I\nОбратная матрица вычислена неверно");
            }

            Add_To_History("Проверка обратной матрицы:", originalGrid, inverseMatrix, Grid_Result);
            Change_Status_Bar("INVERSE MATRIX CHECK COMPLETED");
        }
        
        /*————————————————DETERMINATOR MATRIX————————————————*/
        private void Click_Determinator(object sender, EventArgs e)
        {
            DataGridView Grid = Get_Active_Grid(comboBox_Choose_Matrix_Operation);

            if (Grid.RowCount == Grid.ColumnCount)
            {
                textBox_Determinator.Text = Determinator().ToString();
                Change_Status_Bar($"{comboBox_Choose_Matrix_Operation.Text} DETERMINATOR CALCULATED");
            }
            else
            {
                Message_Error("Размерность матрицы должна быть равной ZxZ.");
            }
        }
        private double Determinator()
        {
            double temp, b, det = 1;
            int z;
            bool f;
            DataGridView Grid = Get_Active_Grid(comboBox_Choose_Matrix_Operation);

            double[,] M = new double[Grid.RowCount, Grid.RowCount];
            for (int i = 0; i < Grid.RowCount; i++)
                for (int j = 0; j < Grid.RowCount; j++)
                {
                    M[i, j] = Convert.ToDouble(Grid.Rows[i].Cells[j].Value);
                }
            for (int i = 0; i < Grid.RowCount; i++)
            {
                for (int j = i + 1; j < Grid.RowCount; j++)
                {
                    if (M[i, i] == 0)
                    {
                        z = i + 1;
                        f = true;
                        while (z != Grid.RowCount && f == true)
                        {
                            if (M[z, j - 1] == 0) z++;
                            else f = false;
                        }
                        if (z != Grid.RowCount)
                        {
                            for (int k = i; k < Grid.RowCount; k++)
                            {
                                temp = M[i, k];
                                M[i, k] = M[z, k];
                                M[z, k] = temp;
                            }
                            det = -1 * det;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    b = M[j, i] / M[i, i];
                    for (int k = i; k < Grid.RowCount; k++)
                        M[j, k] -= M[i, k] * b;
                }
                det *= M[i, i];
            }
            return det;
        }
        
        /*————————————————TRANSPOSITION MATRIX————————————————*/
        private void Click_Transposition(object sender, EventArgs e)

        {
            DataGridView Grid = Get_Active_Grid(comboBox_Choose_Matrix_Operation);
            Set_Matrix_Size(Grid_Result, panel_For_Result, Grid.ColumnCount, Grid.RowCount);
            Transpose_Matrix(Grid, Grid_Result);
            Change_Status_Bar($"TRANSPOSITION {comboBox_Choose_Matrix_Operation.Text}");

            Grid_Result.Refresh();
        }
        private void Transpose_Matrix(DataGridView Grid, DataGridView ResultGrid)
        {
            ResultGrid.ColumnCount = Grid.RowCount;
            ResultGrid.RowCount = Grid.ColumnCount;
            for (int i = 0; i < Grid.RowCount; i++)
                for (int j = 0; j < Grid.ColumnCount; j++)
                    ResultGrid.Rows[j].Cells[i].Value = Grid.Rows[i].Cells[j].Value;
            ResultGrid.Visible = true;

            Add_To_History($"Транспонирование {comboBox_Choose_Matrix_Operation.Text}",
                    matrixA: Grid,
                    result: Grid_Result);
            Change_Status_Bar($"{comboBox_Choose_Matrix_Operation.Text}: TRANSPOSED");
        }

        /*————————————————COMBO BOX CHANGED————————————————*/
        private void comboBox_Changed_A_X(object sender, EventArgs e)
        {
            int X, Y;
            if ( (int.TryParse(comboBox_A_X.Text, out X)) && (int.TryParse(comboBox_A_Y.Text, out Y)) )
            {
                Set_Matrix_Size(Grid_Matrix_A, panel_For_Grid_A, X, Y);

                Change_Status_Bar($"MATRIX A: SET {comboBox_A_X.Text} ROWS");
            }

        }
        private void comboBox_Changed_A_Y(object sender, EventArgs e)
        {
            int X, Y;
            if ((int.TryParse(comboBox_A_X.Text, out X)) && (int.TryParse(comboBox_A_Y.Text, out Y)))
            {
                Set_Matrix_Size(Grid_Matrix_A, panel_For_Grid_A, X, Y);

                Change_Status_Bar($"MATRIX A: SET {comboBox_A_Y.Text} COLS");
            }
        }
        private void comboBox_Changed_B_X(object sender, EventArgs e)
        {
            int X, Y;
            if ((int.TryParse(comboBox_B_X.Text, out X)) && (int.TryParse(comboBox_B_Y.Text, out Y)))
            {
                Set_Matrix_Size(Grid_Matrix_B, panel_For_Grid_B, X, Y);

                Change_Status_Bar($"MATRIX B: SET {comboBox_B_X.Text} ROWS");
            }
        }
        private void comboBox_Changed_B_Y(object sender, EventArgs e)
        {
            int X, Y;
            if ((int.TryParse(comboBox_B_X.Text, out X)) && (int.TryParse(comboBox_B_Y.Text, out Y)))
            {
                Set_Matrix_Size(Grid_Matrix_B, panel_For_Grid_B, X, Y);

                Change_Status_Bar($"MATRIX B: SET {comboBox_B_Y.Text} COLS");
            }
        }

        /*————————————————CLEAN MATRIX————————————————*/
        private void Click_Clean_Matrix_A(object sender, EventArgs e)
        {
            Clean_Matrix(Grid_Matrix_A);

            Change_Status_Bar("MATRIX A: CLEANED");
        }
        private void Click_Clean_Matrix_B(object sender, EventArgs e)
        {
            Clean_Matrix(Grid_Matrix_B);

            Change_Status_Bar("MATRIX B: CLEANED");
        }

        private void Clean_Matrix(DataGridView grid)
        {
            for (int i = 0; i < grid.RowCount; i++)
            {
                for (int j = 0; j < grid.ColumnCount; j++)
                {
                    grid.Rows[i].Cells[j].Value = "0";
                }
            }
        }

        /*————————————————FILLING MATRIX————————————————*/
        private void Click_Fill_Matrix(object sender, EventArgs e)
        {
            DataGridView Grid = Get_Active_Grid(comboBox_Choose_Matrix_Fill);
            double min_double = 0, max_double = 0;
            if ((double.TryParse(textBox_Fill_Min.Text, out min_double)) && (double.TryParse(textBox_Fill_Max.Text, out max_double)))
            {
                if (min_double > max_double)
                {

                    textBox_Fill_Min.Text = max_double.ToString();
                    textBox_Fill_Max.Text = min_double.ToString();

                    double temp = min_double;
                    min_double = max_double;
                    max_double = temp;

                    MessageBox.Show("Your value of Min > Max value, we switch your values for ease of perception", "Attention");

                }
                if (radioButton_Integer.Checked)
                {
                    Fill_Matrix_Integers(Grid, min_double, max_double);
                    Change_Status_Bar($"{comboBox_Choose_Matrix_Fill.Text}: FILL RANDOM INTEGER NUMBERS");
                }
                else if (radioButton_Float.Checked)
                {
                    Fill_Matrix_Float(Grid, min_double, max_double);
                    Change_Status_Bar($"{comboBox_Choose_Matrix_Fill.Text}: FILL RANDOM FLOAT NUMBERS");
                }
            }
            else
            {
                Message_Error("ERROR: Can't parse VALUES «Min» or «Max» in menu «FILL RANDON NUMBERS MATRIX»");

            }
        }
        private void Fill_Matrix_Float(DataGridView matrix, double minValue = -10, double maxValue = 10)
        {
            if (minValue > maxValue)
            {

                textBox_Fill_Min.Text = maxValue.ToString();
                textBox_Fill_Max.Text = minValue.ToString();

                double temp = minValue;
                minValue = maxValue;
                maxValue = temp;

                MessageBox.Show("Your value of Min > Max value, we switch your values for ease of perception", "Attention");

            }
            Random rand = new Random();

            int maxDecimalPlaces = Math.Max( Count_Decimal_Places(minValue), Count_Decimal_Places(maxValue) );
            if (maxDecimalPlaces < 1)
                maxDecimalPlaces = 2;
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    double randomValue = rand.NextDouble() * (maxValue - minValue) + minValue;
                    matrix.Rows[i].Cells[j].Value = Math.Round(randomValue, maxDecimalPlaces);
                }
            }
        }
        private int Count_Decimal_Places(double value)
        {
            string str = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            int decimalPoint = str.IndexOf('.');

            if (decimalPoint < 0)
                return 0;

            int decimalPlaces = str.Length - decimalPoint - 1;
            for (int i = str.Length - 1; i > decimalPoint; i--)
            {
                if (str[i] != '0')
                    break;
                decimalPlaces--;
            }

            return decimalPlaces;
        }
        private void Fill_Matrix_Integers(DataGridView matrix, double minValue = -10, double maxValue = 10)
        {
            int intMin = (int)Math.Round(minValue);
            int intMax = (int)Math.Round(maxValue);
            if (intMin > intMax)
            {

                textBox_Fill_Min.Text = intMax.ToString();
                textBox_Fill_Max.Text = intMin.ToString();

                int temp = intMin;
                intMin = intMax;
                intMax = temp;

                MessageBox.Show("Your value of Min > Max value, we switch your values for ease of perception", "Attention");

            }
            Random rand = new Random();

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    matrix.Rows[i].Cells[j].Value = rand.Next(intMin, intMax + 1);
                }
            }
        }

        /*————————————————SETUPS FOR MATRICES————————————————*/
        private void Set_Start_Position()
        {
            comboBox_A_X.SelectedIndex = 3;
            comboBox_A_Y.SelectedIndex = 3;
            comboBox_B_X.SelectedIndex = 3;
            comboBox_B_Y.SelectedIndex = 3;
        }
        private void Init_Grid(DataGridView Grid, Panel panel)
        {
            Grid.Dock                     = DockStyle.Fill;
            Grid.ScrollBars               = ScrollBars.None;
            Grid.AllowUserToResizeColumns = false;
            Grid.AllowUserToResizeRows    = false;
            Grid.AutoSizeColumnsMode      = DataGridViewAutoSizeColumnsMode.None;
            Grid.AutoSizeRowsMode         = DataGridViewAutoSizeRowsMode.None;
            Grid.RowHeadersVisible        = false;
            Grid.ColumnHeadersVisible     = false;
            Grid.AllowUserToAddRows       = false;

            panel.Resize += (sender, e) =>
            {
                if (Grid.ColumnCount > 0 && Grid.RowCount > 0)
                    Adjust_Grid_Size(Grid, panel, Grid.RowCount, Grid.ColumnCount);
            };
        }
        private void Adjust_Grid_Size(DataGridView Grid, Panel panel, int rowCount, int columnCount)
        {
            if (Grid == null || panel == null || rowCount <= 0 || columnCount <= 0)
                return;

            Grid.SuspendLayout();
            Grid.ScrollBars = ScrollBars.None;

            Grid.RowCount = rowCount;
            Grid.ColumnCount = columnCount;

            int cellWidth = (panel.Width - 1) / columnCount;
            int cellHeight = (panel.Height - 1) / rowCount;

            if (cellWidth < 1) cellWidth = 1;
            if (cellHeight < 1) cellHeight = 1;

            int fontSize = (int)(Math.Min(cellWidth, cellHeight) * 0.7);
            fontSize = Math.Max(6, Math.Min(fontSize, 20));

            Grid.DefaultCellStyle.Font = new Font(Grid.Font.FontFamily, fontSize);

            foreach (DataGridViewColumn column in Grid.Columns)
            {
                column.Width = cellWidth;
            }

            foreach (DataGridViewRow row in Grid.Rows)
            {
                row.Height = cellHeight;
            }

            Grid.Width = cellWidth * columnCount;
            Grid.Height = cellHeight * rowCount;

            Grid.ResumeLayout();
        }
        private void Set_Matrix_Size(DataGridView Grid, Panel panel, int rows, int columns)
        {
            Adjust_Grid_Size(Grid, panel, rows, columns);

            var oldData = new object[Grid.RowCount, Grid.ColumnCount];
            for (int i = 0; i < Grid.RowCount; i++)
            {
                for (int j = 0; j < Grid.ColumnCount; j++)
                {
                    oldData[i, j] = Grid.Rows[i].Cells[j].Value;
                }
            }

            Grid.RowCount = rows;
            Grid.ColumnCount = columns;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (i < oldData.GetLength(0) && j < oldData.GetLength(1))
                    {
                        Grid.Rows[i].Cells[j].Value = oldData[i, j] ?? 0;
                    }
                    else
                    {
                        Grid.Rows[i].Cells[j].Value = 0;
                    }
                }
            }
        }
    }
}
