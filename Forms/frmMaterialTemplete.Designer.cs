namespace laserCraft_Control
{
    partial class frmMaterialTemplete
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
            this.grbInfo = new System.Windows.Forms.GroupBox();
            this.numThickness = new System.Windows.Forms.NumericUpDown();
            this.lblThickness = new System.Windows.Forms.Label();
            this.numRepeatTimes = new System.Windows.Forms.NumericUpDown();
            this.lblRepeatTimes = new System.Windows.Forms.Label();
            this.numLaserIntensity = new System.Windows.Forms.NumericUpDown();
            this.lblLaserIntensity = new System.Windows.Forms.Label();
            this.tbxName = new System.Windows.Forms.TextBox();
            this.lblObjectName = new System.Windows.Forms.Label();
            this.numFeedRate = new System.Windows.Forms.NumericUpDown();
            this.lblFeedRate = new System.Windows.Forms.Label();
            this.gdData = new System.Windows.Forms.DataGridView();
            this.pnControl = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.grbInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRepeatTimes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLaserIntensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFeedRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdData)).BeginInit();
            this.pnControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbInfo
            // 
            this.grbInfo.Controls.Add(this.btnDelete);
            this.grbInfo.Controls.Add(this.btnSave);
            this.grbInfo.Controls.Add(this.btnNew);
            this.grbInfo.Controls.Add(this.numThickness);
            this.grbInfo.Controls.Add(this.lblThickness);
            this.grbInfo.Controls.Add(this.numRepeatTimes);
            this.grbInfo.Controls.Add(this.lblRepeatTimes);
            this.grbInfo.Controls.Add(this.numLaserIntensity);
            this.grbInfo.Controls.Add(this.lblLaserIntensity);
            this.grbInfo.Controls.Add(this.tbxName);
            this.grbInfo.Controls.Add(this.lblObjectName);
            this.grbInfo.Controls.Add(this.numFeedRate);
            this.grbInfo.Controls.Add(this.lblFeedRate);
            this.grbInfo.Location = new System.Drawing.Point(12, 12);
            this.grbInfo.Name = "grbInfo";
            this.grbInfo.Size = new System.Drawing.Size(760, 116);
            this.grbInfo.TabIndex = 0;
            this.grbInfo.TabStop = false;
            this.grbInfo.Text = "Information";
            // 
            // numThickness
            // 
            this.numThickness.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numThickness.Location = new System.Drawing.Point(363, 75);
            this.numThickness.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThickness.Name = "numThickness";
            this.numThickness.Size = new System.Drawing.Size(101, 21);
            this.numThickness.TabIndex = 9;
            this.numThickness.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblThickness
            // 
            this.lblThickness.AutoSize = true;
            this.lblThickness.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThickness.Location = new System.Drawing.Point(266, 75);
            this.lblThickness.Name = "lblThickness";
            this.lblThickness.Size = new System.Drawing.Size(65, 15);
            this.lblThickness.TabIndex = 8;
            this.lblThickness.Text = "Thickness:";
            // 
            // numRepeatTimes
            // 
            this.numRepeatTimes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numRepeatTimes.Location = new System.Drawing.Point(363, 50);
            this.numRepeatTimes.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numRepeatTimes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRepeatTimes.Name = "numRepeatTimes";
            this.numRepeatTimes.Size = new System.Drawing.Size(101, 21);
            this.numRepeatTimes.TabIndex = 7;
            this.numRepeatTimes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblRepeatTimes
            // 
            this.lblRepeatTimes.AutoSize = true;
            this.lblRepeatTimes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRepeatTimes.Location = new System.Drawing.Point(266, 50);
            this.lblRepeatTimes.Name = "lblRepeatTimes";
            this.lblRepeatTimes.Size = new System.Drawing.Size(87, 15);
            this.lblRepeatTimes.TabIndex = 6;
            this.lblRepeatTimes.Text = "Repeat Times:";
            // 
            // numLaserIntensity
            // 
            this.numLaserIntensity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numLaserIntensity.Location = new System.Drawing.Point(125, 77);
            this.numLaserIntensity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numLaserIntensity.Name = "numLaserIntensity";
            this.numLaserIntensity.Size = new System.Drawing.Size(101, 21);
            this.numLaserIntensity.TabIndex = 5;
            // 
            // lblLaserIntensity
            // 
            this.lblLaserIntensity.AutoSize = true;
            this.lblLaserIntensity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLaserIntensity.Location = new System.Drawing.Point(28, 77);
            this.lblLaserIntensity.Name = "lblLaserIntensity";
            this.lblLaserIntensity.Size = new System.Drawing.Size(88, 15);
            this.lblLaserIntensity.TabIndex = 4;
            this.lblLaserIntensity.Text = "Laser Intensity:";
            // 
            // tbxName
            // 
            this.tbxName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxName.Location = new System.Drawing.Point(126, 24);
            this.tbxName.Name = "tbxName";
            this.tbxName.Size = new System.Drawing.Size(338, 21);
            this.tbxName.TabIndex = 3;
            // 
            // lblObjectName
            // 
            this.lblObjectName.AutoSize = true;
            this.lblObjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectName.Location = new System.Drawing.Point(29, 27);
            this.lblObjectName.Name = "lblObjectName";
            this.lblObjectName.Size = new System.Drawing.Size(44, 15);
            this.lblObjectName.TabIndex = 2;
            this.lblObjectName.Text = "Name:";
            // 
            // numFeedRate
            // 
            this.numFeedRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numFeedRate.Location = new System.Drawing.Point(126, 50);
            this.numFeedRate.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numFeedRate.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numFeedRate.Name = "numFeedRate";
            this.numFeedRate.Size = new System.Drawing.Size(101, 21);
            this.numFeedRate.TabIndex = 1;
            this.numFeedRate.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblFeedRate
            // 
            this.lblFeedRate.AutoSize = true;
            this.lblFeedRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFeedRate.Location = new System.Drawing.Point(29, 50);
            this.lblFeedRate.Name = "lblFeedRate";
            this.lblFeedRate.Size = new System.Drawing.Size(67, 15);
            this.lblFeedRate.TabIndex = 0;
            this.lblFeedRate.Text = "Feed Rate:";
            // 
            // gdData
            // 
            this.gdData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdData.Location = new System.Drawing.Point(12, 140);
            this.gdData.Name = "gdData";
            this.gdData.Size = new System.Drawing.Size(760, 311);
            this.gdData.TabIndex = 1;
            this.gdData.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gdData_CellMouseClick);
            this.gdData.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gdData_CellMouseDoubleClick);
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.btnOk);
            this.pnControl.Controls.Add(this.btnCancel);
            this.pnControl.Location = new System.Drawing.Point(12, 462);
            this.pnControl.Name = "pnControl";
            this.pnControl.Size = new System.Drawing.Size(760, 43);
            this.pnControl.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.Image = global::laserCraft_Control.Properties.Resources.icons8_ok_24;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(592, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(74, 30);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::laserCraft_Control.Properties.Resources.icons8_cancel_24;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(672, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = global::laserCraft_Control.Properties.Resources.icons8_trash_24;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(672, 66);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 30);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "Delete";
            this.btnDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::laserCraft_Control.Properties.Resources.icons8_save_24;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(580, 66);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 30);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnNew
            // 
            this.btnNew.Image = global::laserCraft_Control.Properties.Resources.icons8_plus_26;
            this.btnNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNew.Location = new System.Drawing.Point(478, 66);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(96, 30);
            this.btnNew.TabIndex = 10;
            this.btnNew.Text = "Add New";
            this.btnNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNew.UseVisualStyleBackColor = true;
            // 
            // frmMaterialTemplete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 509);
            this.Controls.Add(this.pnControl);
            this.Controls.Add(this.gdData);
            this.Controls.Add(this.grbInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMaterialTemplete";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Material Templates";
            this.Load += new System.EventHandler(this.frmMaterialTemplete_Load);
            this.grbInfo.ResumeLayout(false);
            this.grbInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRepeatTimes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLaserIntensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFeedRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdData)).EndInit();
            this.pnControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbInfo;
        private System.Windows.Forms.DataGridView gdData;
        private System.Windows.Forms.Panel pnControl;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.NumericUpDown numThickness;
        private System.Windows.Forms.Label lblThickness;
        private System.Windows.Forms.NumericUpDown numRepeatTimes;
        private System.Windows.Forms.Label lblRepeatTimes;
        private System.Windows.Forms.NumericUpDown numLaserIntensity;
        private System.Windows.Forms.Label lblLaserIntensity;
        private System.Windows.Forms.TextBox tbxName;
        private System.Windows.Forms.Label lblObjectName;
        private System.Windows.Forms.NumericUpDown numFeedRate;
        private System.Windows.Forms.Label lblFeedRate;
        private System.Windows.Forms.Button btnDelete;
    }
}