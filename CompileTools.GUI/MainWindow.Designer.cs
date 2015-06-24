namespace CompileTools.GUI
{
    partial class MainWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doesNothingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutHeroToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainStatus = new System.Windows.Forms.StatusStrip();
            this.sidebarMenu = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.mainContainer = new System.Windows.Forms.Panel();
            this.imageViewer1 = new CompileTools.GUI.ImageViewer();
            this.imageViewerButton = new CompileTools.GUI.HeroButton();
            this.homeButton = new CompileTools.GUI.HeroButton();
            this.mainMenu.SuspendLayout();
            this.sidebarMenu.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(944, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doesNothingToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // doesNothingToolStripMenuItem
            // 
            this.doesNothingToolStripMenuItem.Name = "doesNothingToolStripMenuItem";
            this.doesNothingToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.doesNothingToolStripMenuItem.Text = "Does Nothing";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(144, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutHeroToolToolStripMenuItem,
            this.supportToolStripMenuItem,
            this.toolStripSeparator2,
            this.checkForUpdatesToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutHeroToolToolStripMenuItem
            // 
            this.aboutHeroToolToolStripMenuItem.Name = "aboutHeroToolToolStripMenuItem";
            this.aboutHeroToolToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.aboutHeroToolToolStripMenuItem.Text = "About HeroTool";
            // 
            // supportToolStripMenuItem
            // 
            this.supportToolStripMenuItem.Name = "supportToolStripMenuItem";
            this.supportToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.supportToolStripMenuItem.Text = "Support";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(167, 6);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for updates";
            // 
            // mainStatus
            // 
            this.mainStatus.Location = new System.Drawing.Point(0, 579);
            this.mainStatus.Name = "mainStatus";
            this.mainStatus.Size = new System.Drawing.Size(944, 22);
            this.mainStatus.TabIndex = 1;
            this.mainStatus.Text = "statusStrip1";
            // 
            // sidebarMenu
            // 
            this.sidebarMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.sidebarMenu.BackColor = System.Drawing.SystemColors.Control;
            this.sidebarMenu.Controls.Add(this.label1);
            this.sidebarMenu.Controls.Add(this.imageViewerButton);
            this.sidebarMenu.Controls.Add(this.homeButton);
            this.sidebarMenu.Location = new System.Drawing.Point(0, 23);
            this.sidebarMenu.Name = "sidebarMenu";
            this.sidebarMenu.Size = new System.Drawing.Size(135, 556);
            this.sidebarMenu.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 2);
            this.label1.TabIndex = 5;
            // 
            // mainContainer
            // 
            this.mainContainer.AllowDrop = true;
            this.mainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainContainer.Controls.Add(this.imageViewer1);
            this.mainContainer.Location = new System.Drawing.Point(135, 23);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Size = new System.Drawing.Size(809, 556);
            this.mainContainer.TabIndex = 3;
            // 
            // imageViewer1
            // 
            this.imageViewer1.AllowDrop = true;
            this.imageViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageViewer1.Location = new System.Drawing.Point(0, 0);
            this.imageViewer1.Name = "imageViewer1";
            this.imageViewer1.Size = new System.Drawing.Size(809, 556);
            this.imageViewer1.TabIndex = 0;
            // 
            // imageViewerButton
            // 
            this.imageViewerButton.FlatAppearance.BorderSize = 0;
            this.imageViewerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.imageViewerButton.Image = global::CompileTools.GUI.Properties.Resources.film;
            this.imageViewerButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.imageViewerButton.Location = new System.Drawing.Point(6, 56);
            this.imageViewerButton.Name = "imageViewerButton";
            this.imageViewerButton.Selected = false;
            this.imageViewerButton.Size = new System.Drawing.Size(123, 24);
            this.imageViewerButton.TabIndex = 4;
            this.imageViewerButton.Text = "Image Viewer";
            this.imageViewerButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.imageViewerButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.imageViewerButton.UseVisualStyleBackColor = true;
            this.imageViewerButton.Click += new System.EventHandler(this.imageViewerButton_Click);
            // 
            // homeButton
            // 
            this.homeButton.FlatAppearance.BorderSize = 0;
            this.homeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.homeButton.Image = global::CompileTools.GUI.Properties.Resources.home;
            this.homeButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.homeButton.Location = new System.Drawing.Point(6, 15);
            this.homeButton.Name = "homeButton";
            this.homeButton.Selected = false;
            this.homeButton.Size = new System.Drawing.Size(123, 24);
            this.homeButton.TabIndex = 0;
            this.homeButton.Text = "Home";
            this.homeButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.homeButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.homeButton.UseVisualStyleBackColor = true;
            this.homeButton.Click += new System.EventHandler(this.homeButton_Click);
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(944, 601);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.sidebarMenu);
            this.Controls.Add(this.mainStatus);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainWindow";
            this.Text = "HeroTool";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.sidebarMenu.ResumeLayout(false);
            this.mainContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.StatusStrip mainStatus;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel sidebarMenu;
        private System.Windows.Forms.ToolStripMenuItem doesNothingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutHeroToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private HeroButton homeButton;
        private HeroButton imageViewerButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel mainContainer;
        private ImageViewer imageViewer1;
    }
}

