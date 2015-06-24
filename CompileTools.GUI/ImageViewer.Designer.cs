namespace CompileTools.GUI
{
    partial class ImageViewer
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.inputPictureBox = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.palettePanel1 = new CompileTools.GUI.PalettePanel(this.components);
            this.palettePanel2 = new CompileTools.GUI.PalettePanel(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(407, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(399, 271);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // inputPictureBox
            // 
            this.inputPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputPictureBox.Location = new System.Drawing.Point(3, 3);
            this.inputPictureBox.Name = "inputPictureBox";
            this.inputPictureBox.Size = new System.Drawing.Size(398, 271);
            this.inputPictureBox.TabIndex = 0;
            this.inputPictureBox.TabStop = false;
            this.inputPictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.inputPictureBox_DragDrop);
            this.inputPictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.inputPictureBox_DragEnter);
            this.inputPictureBox.DragOver += new System.Windows.Forms.DragEventHandler(this.inputPictureBox_DragOver);
            this.inputPictureBox.DragLeave += new System.EventHandler(this.inputPictureBox_DragLeave);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.inputPictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.palettePanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.palettePanel2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(809, 555);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // palettePanel1
            // 
            this.palettePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.palettePanel1.Location = new System.Drawing.Point(3, 280);
            this.palettePanel1.Name = "palettePanel1";
            this.palettePanel1.Size = new System.Drawing.Size(398, 272);
            this.palettePanel1.TabIndex = 2;
            // 
            // palettePanel2
            // 
            this.palettePanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.palettePanel2.Location = new System.Drawing.Point(407, 280);
            this.palettePanel2.Name = "palettePanel2";
            this.palettePanel2.Size = new System.Drawing.Size(399, 272);
            this.palettePanel2.TabIndex = 3;
            // 
            // ImageViewer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ImageViewer";
            this.Size = new System.Drawing.Size(809, 555);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox inputPictureBox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private PalettePanel palettePanel1;
        private PalettePanel palettePanel2;
    }
}
