namespace LTDCE
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public class Form1 : Form
    {
        private uint BGOffset = 0x3d0ce;
        private Button button_OpenPGF;
        private Button button_OpenROM;
        private Button button_SavePGF;
        private Button button_SaveROM;
        private Button button_SaveText;
        private uint CardOffset = 0x3ce00;
        private CheckBox checkBox_BG;
        private CheckBox checkBox_SaveAll;
        private ComboBox comboBox_List;
        private IContainer components;
        private uint DateOffset = 0x3e484;
        private uint JPOffset = 0x3dedf;
        private uint KOROffset = 0x3e1af;
        private Label label_Title;
        private MemoryStream ms = new MemoryStream(0);
        private uint PGFOffset = 0x3ce04;
        private uint ReadLetter;
        private TextBox textBox_Text;
        private uint TextOffset = 0x3ced4;
        private bool ValidROM;
        private Label label1;
        private Label label2;
        private uint VersionOffset = 0x3ced2;

        public Form1()
        {
            this.InitializeComponent();
        }

        private void button_OpenPGF_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                Title = "打开宝可梦神秘礼物文件",
                Filter = "宝可梦神秘礼物文件(*.pgf)|*.pgf"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                BinaryReader reader = new BinaryReader(File.OpenRead(dialog.FileName));
                if (reader.BaseStream.Length == 0xccL)
                {
                    if (this.checkBox_SaveAll.Checked)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            this.ms.Position = this.PGFOffset + (i * 720);
                            reader.BaseStream.Position = 0L;
                            for (int j = 0; j < reader.BaseStream.Length; j++)
                            {
                                this.ms.WriteByte(reader.ReadByte());
                            }
                        }
                        reader.Close();
                    }
                    else
                    {
                        this.ms.Position = this.PGFOffset + (this.comboBox_List.SelectedIndex * 720);
                        reader.BaseStream.Position = 0L;
                        for (int k = 0; k < reader.BaseStream.Length; k++)
                        {
                            this.ms.WriteByte(reader.ReadByte());
                        }
                        reader.Close();
                    }
                    this.ShowText();
                }
                else
                {
                    MessageBox.Show("这似乎并不是一个可用的PGF文件", "不可用的PGF文件", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void button_OpenROM_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                Title = "打开NDS官方配信ROM",
                Filter = "NDS官方配信ROM(*.nds)|*.nds"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                BinaryReader reader = new BinaryReader(File.OpenRead(dialog.FileName)) {
                    BaseStream = { Position = 0L }
                };
                byte[] buffer = new byte[0x12];
                reader.Read(buffer, 0, 0x12);
                if (Encoding.UTF8.GetString(buffer) == "POKWBLIBERTYY8KP01")
                {
                    this.ValidROM = true;
                    this.button_SaveROM.Enabled = true;
                    this.button_OpenPGF.Enabled = true;
                    this.button_SavePGF.Enabled = true;
                    this.button_SaveText.Enabled = true;
                    this.comboBox_List.Enabled = true;
                    this.textBox_Text.Enabled = true;
                    this.checkBox_SaveAll.Enabled = true;
                    this.checkBox_BG.Enabled = true;
                }
                else
                {
                    this.ValidROM = false;
                    this.button_SaveROM.Enabled = false;
                    this.button_OpenPGF.Enabled = false;
                    this.button_SavePGF.Enabled = false;
                    this.button_SaveText.Enabled = false;
                    this.comboBox_List.Enabled = false;
                    this.textBox_Text.Enabled = false;
                    this.checkBox_SaveAll.Enabled = false;
                    this.checkBox_BG.Enabled = false;
                    this.label_Title.Text = "";
                    this.textBox_Text.Text = "";
                    this.comboBox_List.SelectedIndex = 0;
                    reader.Close();
                    MessageBox.Show("这似乎并不是官方配信自由船票ROM", "不可用的ROM", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                if (this.ValidROM)
                {
                    reader.BaseStream.Position = 0L;
                    this.ms.Position = 0L;
                    byte[] buffer2 = new byte[reader.BaseStream.Length];
                    reader.Read(buffer2, 0, (int) reader.BaseStream.Length);
                    this.ms.Write(buffer2, 0, (int) reader.BaseStream.Length);
                    reader.Close();
                    this.ShowText();
                }
            }
        }

        private void button_SavePGF_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog {
                Title = "保存宝可梦神秘礼物文件",
                Filter = "宝可梦神秘礼物文件(*.pgf)|*.pgf"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                BinaryWriter writer = new BinaryWriter(File.Create(dialog.FileName));
                this.ms.Position = this.PGFOffset + (this.comboBox_List.SelectedIndex * 720);
                writer.BaseStream.Position = 0L;
                for (int i = 0; i < 0xcc; i++)
                {
                    writer.Write((byte) this.ms.ReadByte());
                }
                writer.Close();
                this.ShowText();
            }
        }

        private void button_SaveROM_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog {
                Title = "生成NDS自制配信ROM",
                Filter = "NDS自制配信ROM(*.nds)|*.nds"
            };
            this.ms.Position = this.DateOffset;
            for (int i = 0; i < 7; i++)
            {
                this.ms.WriteByte(0);
                this.ms.WriteByte(0);
                this.ms.WriteByte(1);
                this.ms.WriteByte(1);
                this.ms.WriteByte(0xff);
                this.ms.WriteByte(0xff);
                this.ms.WriteByte(12);
                this.ms.WriteByte(0x1f);
            }
            for (int j = 0; j < 7; j++)
            {
                this.ms.Position = this.VersionOffset + (j * 720);
                this.ms.WriteByte(240);
            }
            this.ms.Position = this.CardOffset;
            this.ms.WriteByte(7);
            this.ms.Position = this.JPOffset;
            this.ms.WriteByte(1);
            this.ms.Position = this.KOROffset;
            this.ms.WriteByte(8);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(dialog.FileName, this.ms.ToArray());
            }
        }

        private void button_SaveText_Click(object sender, EventArgs e)
        {
            if (this.checkBox_SaveAll.Checked)
            {
                for (int i = 0; i < 7; i++)
                {
                    this.ms.Position = this.TextOffset + (i * 720);
                    for (int j = 0; j < 0x1fa; j++)
                    {
                        this.ms.WriteByte(0xff);
                    }
                    this.ms.Position = this.TextOffset + (i * 720);
                    for (int k = 0; k < this.textBox_Text.Text.Length; k++)
                    {
                        if (this.textBox_Text.Text[k] != '\r')
                        {
                            this.ms.WriteByte(Convert.ToByte((int) (this.textBox_Text.Text[k] - ((this.textBox_Text.Text[k] / 'Ā') * 0x100))));
                            this.ms.WriteByte(Convert.ToByte((int) (this.textBox_Text.Text[k] / 'Ā')));
                        }
                        else
                        {
                            this.ms.WriteByte(0xfe);
                            this.ms.WriteByte(0xff);
                            k++;
                        }
                    }
                }
            }
            else
            {
                this.ms.Position = this.TextOffset + (this.comboBox_List.SelectedIndex * 720);
                for (int m = 0; m < 0x1fa; m++)
                {
                    this.ms.WriteByte(0xff);
                }
                this.ms.Position = this.TextOffset + (this.comboBox_List.SelectedIndex * 720);
                for (int n = 0; n < this.textBox_Text.Text.Length; n++)
                {
                    if (this.textBox_Text.Text[n] != '\r')
                    {
                        this.ms.WriteByte(Convert.ToByte((int) (this.textBox_Text.Text[n] - ((this.textBox_Text.Text[n] / 'Ā') * 0x100))));
                        this.ms.WriteByte(Convert.ToByte((int) (this.textBox_Text.Text[n] / 'Ā')));
                    }
                    else
                    {
                        this.ms.WriteByte(0xfe);
                        this.ms.WriteByte(0xff);
                        n++;
                    }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_SaveAll.Checked)
            {
                for (int i = 0; i < 7; i++)
                {
                    this.ms.Position = this.BGOffset + (i * 720);
                    if (this.checkBox_BG.Checked)
                    {
                        this.ms.WriteByte(1);
                    }
                    else
                    {
                        this.ms.WriteByte(0);
                    }
                }
            }
            else
            {
                this.ms.Position = this.BGOffset + (this.comboBox_List.SelectedIndex * 720);
                if (this.checkBox_BG.Checked)
                {
                    this.ms.WriteByte(1);
                }
                else
                {
                    this.ms.WriteByte(0);
                }
            }
        }

        private void comboBox_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ValidROM)
            {
                this.ShowText();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.comboBox_List.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.button_OpenROM = new System.Windows.Forms.Button();
            this.button_SaveROM = new System.Windows.Forms.Button();
            this.button_OpenPGF = new System.Windows.Forms.Button();
            this.button_SavePGF = new System.Windows.Forms.Button();
            this.button_SaveText = new System.Windows.Forms.Button();
            this.comboBox_List = new System.Windows.Forms.ComboBox();
            this.label_Title = new System.Windows.Forms.Label();
            this.textBox_Text = new System.Windows.Forms.TextBox();
            this.checkBox_BG = new System.Windows.Forms.CheckBox();
            this.checkBox_SaveAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_OpenROM
            // 
            this.button_OpenROM.Location = new System.Drawing.Point(12, 6);
            this.button_OpenROM.Name = "button_OpenROM";
            this.button_OpenROM.Size = new System.Drawing.Size(112, 40);
            this.button_OpenROM.TabIndex = 0;
            this.button_OpenROM.Text = "打开官方配信ROM";
            this.button_OpenROM.UseVisualStyleBackColor = true;
            this.button_OpenROM.Click += new System.EventHandler(this.button_OpenROM_Click);
            // 
            // button_SaveROM
            // 
            this.button_SaveROM.Enabled = false;
            this.button_SaveROM.Location = new System.Drawing.Point(143, 6);
            this.button_SaveROM.Name = "button_SaveROM";
            this.button_SaveROM.Size = new System.Drawing.Size(112, 40);
            this.button_SaveROM.TabIndex = 1;
            this.button_SaveROM.Text = "生成自制配信ROM";
            this.button_SaveROM.UseVisualStyleBackColor = true;
            this.button_SaveROM.Click += new System.EventHandler(this.button_SaveROM_Click);
            // 
            // button_OpenPGF
            // 
            this.button_OpenPGF.Enabled = false;
            this.button_OpenPGF.Location = new System.Drawing.Point(12, 50);
            this.button_OpenPGF.Name = "button_OpenPGF";
            this.button_OpenPGF.Size = new System.Drawing.Size(112, 35);
            this.button_OpenPGF.TabIndex = 2;
            this.button_OpenPGF.Text = "导入神秘礼物\nPGF文件";
            this.button_OpenPGF.UseVisualStyleBackColor = true;
            this.button_OpenPGF.Click += new System.EventHandler(this.button_OpenPGF_Click);
            // 
            // button_SavePGF
            // 
            this.button_SavePGF.Enabled = false;
            this.button_SavePGF.Location = new System.Drawing.Point(143, 50);
            this.button_SavePGF.Name = "button_SavePGF";
            this.button_SavePGF.Size = new System.Drawing.Size(112, 35);
            this.button_SavePGF.TabIndex = 3;
            this.button_SavePGF.Text = "导出神秘礼物\nPGF文件";
            this.button_SavePGF.UseVisualStyleBackColor = true;
            this.button_SavePGF.Click += new System.EventHandler(this.button_SavePGF_Click);
            // 
            // button_SaveText
            // 
            this.button_SaveText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_SaveText.Enabled = false;
            this.button_SaveText.Location = new System.Drawing.Point(270, 50);
            this.button_SaveText.Name = "button_SaveText";
            this.button_SaveText.Size = new System.Drawing.Size(90, 35);
            this.button_SaveText.TabIndex = 4;
            this.button_SaveText.Text = "保存文本";
            this.button_SaveText.UseVisualStyleBackColor = true;
            this.button_SaveText.Click += new System.EventHandler(this.button_SaveText_Click);
            // 
            // comboBox_List
            // 
            this.comboBox_List.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBox_List.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox_List.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_List.Enabled = false;
            this.comboBox_List.FormattingEnabled = true;
            this.comboBox_List.Items.AddRange(new object[] {
            "英语",
            "法语",
            "意大利语",
            "德语",
            "西班牙语",
            "日语",
            "韩语"});
            this.comboBox_List.Location = new System.Drawing.Point(270, 26);
            this.comboBox_List.Name = "comboBox_List";
            this.comboBox_List.Size = new System.Drawing.Size(90, 20);
            this.comboBox_List.TabIndex = 5;
            this.comboBox_List.SelectedIndexChanged += new System.EventHandler(this.comboBox_List_SelectedIndexChanged);
            // 
            // label_Title
            // 
            this.label_Title.AutoSize = true;
            this.label_Title.Location = new System.Drawing.Point(12, 90);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(0, 12);
            this.label_Title.TabIndex = 7;
            // 
            // textBox_Text
            // 
            this.textBox_Text.Enabled = false;
            this.textBox_Text.Location = new System.Drawing.Point(12, 113);
            this.textBox_Text.MaxLength = 252;
            this.textBox_Text.Multiline = true;
            this.textBox_Text.Name = "textBox_Text";
            this.textBox_Text.Size = new System.Drawing.Size(348, 92);
            this.textBox_Text.TabIndex = 8;
            // 
            // checkBox_BG
            // 
            this.checkBox_BG.AutoSize = true;
            this.checkBox_BG.Enabled = false;
            this.checkBox_BG.Location = new System.Drawing.Point(183, 210);
            this.checkBox_BG.Name = "checkBox_BG";
            this.checkBox_BG.Size = new System.Drawing.Size(72, 16);
            this.checkBox_BG.TabIndex = 10;
            this.checkBox_BG.Text = "黑色背景";
            this.checkBox_BG.UseVisualStyleBackColor = true;
            this.checkBox_BG.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox_SaveAll
            // 
            this.checkBox_SaveAll.AutoSize = true;
            this.checkBox_SaveAll.Enabled = false;
            this.checkBox_SaveAll.Location = new System.Drawing.Point(12, 210);
            this.checkBox_SaveAll.Name = "checkBox_SaveAll";
            this.checkBox_SaveAll.Size = new System.Drawing.Size(144, 16);
            this.checkBox_SaveAll.TabIndex = 11;
            this.checkBox_SaveAll.Text = "将变化保存到所有槽位";
            this.checkBox_SaveAll.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(268, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 15);
            this.label1.TabIndex = 35;
            this.label1.Text = "语种：";
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(289, 211);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 16);
            this.label2.TabIndex = 36;
            this.label2.Text = "by KazoWAR";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 231);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox_SaveAll);
            this.Controls.Add(this.checkBox_BG);
            this.Controls.Add(this.textBox_Text);
            this.Controls.Add(this.label_Title);
            this.Controls.Add(this.comboBox_List);
            this.Controls.Add(this.button_SaveText);
            this.Controls.Add(this.button_SavePGF);
            this.Controls.Add(this.button_OpenPGF);
            this.Controls.Add(this.button_SaveROM);
            this.Controls.Add(this.button_OpenROM);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Liberty Ticket Distribution Card Editor（汉化by卧看微尘）";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ShowText()
        {
            this.ms.Position = (this.PGFOffset + (this.comboBox_List.SelectedIndex * 720)) + 0x60L;
            this.label_Title.Text = "";
            for (int i = 0; i < 0x24; i++)
            {
                this.label_Title.Text = this.label_Title.Text + Convert.ToChar((int) (this.ms.ReadByte() + (this.ms.ReadByte() * 0x100)));
            }
            this.ms.Position = this.TextOffset + (this.comboBox_List.SelectedIndex * 720);
            this.textBox_Text.Text = "";
            for (int j = 0; j < 0xfb; j++)
            {
                this.ReadLetter = Convert.ToUInt32((int) (this.ms.ReadByte() + (this.ms.ReadByte() * 0x100)));
                if ((this.ReadLetter != 0xffff) && (this.ReadLetter != 0xfffe))
                {
                    this.textBox_Text.Text = this.textBox_Text.Text + Convert.ToChar(this.ReadLetter);
                }
                else if (this.ReadLetter == 0xfffe)
                {
                    this.textBox_Text.Text = this.textBox_Text.Text + "\r\n";
                }
            }
            this.ms.Position = this.BGOffset + (this.comboBox_List.SelectedIndex * 720);
            this.ReadLetter = (uint) this.ms.ReadByte();
            if (this.ReadLetter == 0)
            {
                this.checkBox_BG.Checked = false;
            }
            else
            {
                this.checkBox_BG.Checked = true;
            }
        }
    }
}

