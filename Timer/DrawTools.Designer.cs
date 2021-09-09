namespace Timer
{
    partial class DrawTools
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.img_box = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.直线工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.矩形工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.圆形工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.多边形工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.橡皮擦ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.img_box)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // img_box
            // 
            this.img_box.ContextMenuStrip = this.contextMenuStrip1;
            this.img_box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.img_box.Location = new System.Drawing.Point(0, 0);
            this.img_box.Name = "img_box";
            this.img_box.Size = new System.Drawing.Size(303, 247);
            this.img_box.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.img_box.TabIndex = 0;
            this.img_box.TabStop = false;
            this.img_box.Paint += new System.Windows.Forms.PaintEventHandler(this.img_box_Paint);
            this.img_box.MouseDown += new System.Windows.Forms.MouseEventHandler(this.img_box_MouseDown);
            this.img_box.MouseUp += new System.Windows.Forms.MouseEventHandler(this.img_box_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.直线工具ToolStripMenuItem,
            this.矩形工具ToolStripMenuItem,
            this.圆形工具ToolStripMenuItem,
            this.多边形工具ToolStripMenuItem,
            this.橡皮擦ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 114);
            // 
            // 直线工具ToolStripMenuItem
            // 
            this.直线工具ToolStripMenuItem.Name = "直线工具ToolStripMenuItem";
            this.直线工具ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.直线工具ToolStripMenuItem.Text = "直线工具";
            this.直线工具ToolStripMenuItem.Click += new System.EventHandler(this.直线工具ToolStripMenuItem_Click);
            // 
            // 矩形工具ToolStripMenuItem
            // 
            this.矩形工具ToolStripMenuItem.Name = "矩形工具ToolStripMenuItem";
            this.矩形工具ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.矩形工具ToolStripMenuItem.Text = "矩形工具";
            this.矩形工具ToolStripMenuItem.Click += new System.EventHandler(this.矩形工具ToolStripMenuItem_Click);
            // 
            // 圆形工具ToolStripMenuItem
            // 
            this.圆形工具ToolStripMenuItem.Name = "圆形工具ToolStripMenuItem";
            this.圆形工具ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.圆形工具ToolStripMenuItem.Text = "圆形工具";
            // 
            // 多边形工具ToolStripMenuItem
            // 
            this.多边形工具ToolStripMenuItem.Name = "多边形工具ToolStripMenuItem";
            this.多边形工具ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.多边形工具ToolStripMenuItem.Text = "多边形工具";
            // 
            // 橡皮擦ToolStripMenuItem
            // 
            this.橡皮擦ToolStripMenuItem.Name = "橡皮擦ToolStripMenuItem";
            this.橡皮擦ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.橡皮擦ToolStripMenuItem.Text = "橡皮擦";
            // 
            // DrawTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.img_box);
            this.Name = "DrawTools";
            this.Size = new System.Drawing.Size(303, 247);
            ((System.ComponentModel.ISupportInitialize)(this.img_box)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox img_box;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 直线工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 矩形工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 圆形工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 多边形工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 橡皮擦ToolStripMenuItem;
    }
}
