namespace VisionPlatform
{
    partial class MatchTemplate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            this.mvdRenderActivex1.MVDShapeChangedEvent -= new VisionDesigner.MVDRenderActivex.MVDShapesChangedEventHandler(this.mvdRenderActivex1_MVDShapeChangedEvent);
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mvdRenderActivex1 = new VisionDesigner.MVDRenderActivex();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ParamNameBoxCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParamValueBoxCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.rtbInfoMessage = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // mvdRenderActivex1
            // 
            this.mvdRenderActivex1.EnableRetainShape = false;
            this.mvdRenderActivex1.InteractType = VisionDesigner.MVDRenderInteractType.Standard;
            this.mvdRenderActivex1.Location = new System.Drawing.Point(11, 11);
            this.mvdRenderActivex1.MenuLanguage = VisionDesigner.MVDRenderMenuLangType.Default;
            this.mvdRenderActivex1.Name = "mvdRenderActivex1";
            this.mvdRenderActivex1.Size = new System.Drawing.Size(570, 460);
            this.mvdRenderActivex1.TabIndex = 13;
            this.mvdRenderActivex1.MVDShapeChangedEvent += new VisionDesigner.MVDRenderActivex.MVDShapesChangedEventHandler(this.mvdRenderActivex1_MVDShapeChangedEvent);
            this.mvdRenderActivex1.MVDMouseEvent += new VisionDesigner.MVDRenderActivex.MVDMouseEventHandler(this.mvdRenderActivex1_MVDMouseEvent);
            this.mvdRenderActivex1.Load += new System.EventHandler(this.mvdRenderActivex1_Load);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(649, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 46);
            this.button1.TabIndex = 14;
            this.button1.Text = "初始化";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(649, 78);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 49);
            this.button2.TabIndex = 15;
            this.button2.Text = "找圆";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParamNameBoxCol,
            this.ParamValueBoxCol});
            this.dataGridView1.Location = new System.Drawing.Point(631, 160);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(225, 277);
            this.dataGridView1.TabIndex = 16;
            // 
            // ParamNameBoxCol
            // 
            this.ParamNameBoxCol.HeaderText = "ParamName";
            this.ParamNameBoxCol.Name = "ParamNameBoxCol";
            this.ParamNameBoxCol.ReadOnly = true;
            this.ParamNameBoxCol.Width = 90;
            // 
            // ParamValueBoxCol
            // 
            this.ParamValueBoxCol.HeaderText = "ParamValue";
            this.ParamValueBoxCol.Name = "ParamValueBoxCol";
            this.ParamValueBoxCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ParamValueBoxCol.Width = 90;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(761, 15);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(83, 46);
            this.button3.TabIndex = 17;
            this.button3.Text = "加载图片";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(761, 78);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(83, 49);
            this.button4.TabIndex = 18;
            this.button4.Text = "找线";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // rtbInfoMessage
            // 
            this.rtbInfoMessage.Location = new System.Drawing.Point(11, 477);
            this.rtbInfoMessage.Name = "rtbInfoMessage";
            this.rtbInfoMessage.Size = new System.Drawing.Size(570, 203);
            this.rtbInfoMessage.TabIndex = 19;
            this.rtbInfoMessage.Text = "";
            // 
            // MatchTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 764);
            this.Controls.Add(this.rtbInfoMessage);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mvdRenderActivex1);
            this.Name = "MatchTemplate";
            this.Text = "MatchTemplate";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private VisionDesigner.MVDRenderActivex mvdRenderActivex1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamNameBoxCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamValueBoxCol;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.RichTextBox rtbInfoMessage;
    }
}