using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Manejo_de_Json
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private class beer
        {
            public int id { get; set; }
            public string userId { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
        private void CreateId()
        {
            if (dgvData.RowCount == 0) 
            {
                txtID.Text = "1";
            }
            else
            {
                int lastIndex = dgvData.RowCount - 1;
                int id = int.Parse(dgvData.Rows[lastIndex].Cells[0].Value.ToString()) + 1;
                txtID.Text = id.ToString();
            }
        }
        private void limpiar()
        {
            foreach(Control control in dgvData.Controls)
            {
                if (control.GetType() == typeof(TextBox) &&  control != txtJason){
                    control.Text = "";
                }
            }
        }
        private bool ValidateData()
        {
            bool status = true;
            foreach(Control con in tlpForm.Controls)
            {
                if (con.GetType() == typeof(TextBox))
                    if (con.Text.TrimEnd().Length == 0 && con != txtJason)
                    {
                        errorProvider1.SetError(con, "Please insert the correct data.");
                        status = false;
                    }
                    else
                    {
                        errorProvider1.SetError(con, "");
                    }
            }
            return status;
        }
        private void json()
        {
            try
            {
                List<beer> list = new List<beer>();
                foreach (DataGridViewRow rows in dgvData.Rows)
                {
                    list.Add(new beer()
                    {
                        id = int.Parse(rows.Cells[0].Value.ToString()),
                        userId = rows.Cells[1].Value.ToString(),
                        name = rows.Cells[2].Value.ToString(),
                        description = rows.Cells[3].Value.ToString()
                    });
                }
                var json = JsonConvert.SerializeObject(list);
                var json2 = JsonConvert.DeserializeObject(json);
                txtJason.Text = json2.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (dgvData.RowCount > 0)
                json();
            else
                MessageBox.Show("You don't have any data in your table. Please add it!");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dgvData.DataSource == null)
            {
                if (ValidateData())
                {
                    dgvData.Rows.Add(txtID.Text, txtUserId.Text, txtName.Text, txtDescription.Text);
                    limpiar();
                    CreateId();
                }
            }
            else
                MessageBox.Show("For the moment this opction is not available on loaded .Json", "System");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvData.RowCount > 0)
            {
                string msg = "Do you realy want to delete this data?";
                if (MessageBox.Show(msg, "Conformation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                {
                    int index = dgvData.CurrentRow.Index;
                    dgvData.Rows.RemoveAt(index);
                }
            }
        }

        private void convertToJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(txtJason.Text.Length > 0)
            {
                try
                {
                    string msg = "Do you want to export this data to a .Json?";
                    if (MessageBox.Show(msg, "Conformation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        using (SaveFileDialog save = new SaveFileDialog())
                        {
                            save.ShowDialog();
                            if (save.FileName != "")
                            {
                                TextWriter txt = new StreamWriter(save.FileName);
                                txt.WriteLine(txtJason.Text);
                                txt.Close();
                                MessageBox.Show("data stored successfully", "System");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void loadAJsonFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.ShowDialog();
                if (open.FileName != "")
                {
                    StreamReader sr = new StreamReader(open.FileName);
                    string data = sr.ReadToEnd();
                    if (data.Length > 0)
                    {
                        txtJason.Text = data;
                    }
                    sr.Close();
                }
            }
            if(txtJason.Text.Length > 0)
            {
                if (dgvData.RowCount > 0)
                {
                    string msg = "You have data in your table. This data will be deleted. Do you want to keep going?";
                    if (MessageBox.Show(msg, "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dynamic json = JsonConvert.DeserializeObject<List<beer>>(txtJason.Text);
                        dgvData.DataSource = null;
                        dgvData.Rows.Clear();
                        dgvData.Columns.Clear();
                        dgvData.DataSource = json;
                        txtJason.Clear();
                        dgvData.ClearSelection();

                        limpiar();
                        CreateId();
                    }
                    else
                        MessageBox.Show("Before keep going, make sure to save all of this data and then say yes!.", "System"); txtJason.Clear();
                }
                else
                {
                    dynamic json = JsonConvert.DeserializeObject<List<beer>>(txtJason.Text);
                    dgvData.Columns.Clear();
                    dgvData.Rows.Clear();
                    dgvData.DataSource = json;
                    txtJason.Clear();
                    dgvData.ClearSelection();

                    limpiar();
                    CreateId();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if(dgvData.CurrentRow.Index > -1)
            {
                dgvData.CurrentRow.Cells[0].Value = txtID.Text;
                dgvData.CurrentRow.Cells[1].Value = txtUserId.Text;
                dgvData.CurrentRow.Cells[2].Value = txtName.Text;
                dgvData.CurrentRow.Cells[3].Value = txtDescription.Text;
                btnEdit.Enabled = false;
                btnAdd.Enabled = true;
                limpiar();
                CreateId();
            }
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex > -1)
            {
                btnAdd.Enabled = false;
                btnEdit.Enabled = true;

                txtID.Text = dgvData.CurrentRow.Cells[0].Value.ToString();
                txtUserId.Text = dgvData.CurrentRow.Cells[1].Value.ToString();
                txtName.Text = dgvData.CurrentRow.Cells[2].Value.ToString();
                txtDescription.Text = dgvData.CurrentRow.Cells[3].Value.ToString();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            CreateId();
        }
    }

}