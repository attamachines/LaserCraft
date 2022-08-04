namespace laserCraft_Control
{
    partial class CusDeginCtrl
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
            this.ruler2 = new Ruler.Ruler();
            this.ruler1 = new Ruler.Ruler();
            this.ctrlScene = new laserCraft_Control.SceneCtrl();
            this.SuspendLayout();
            // 
            // ruler2
            // 
            this.ruler2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ruler2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ruler2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ruler2.isVertical = true;
            this.ruler2.Location = new System.Drawing.Point(3, 3);
            this.ruler2.Name = "ruler2";
            this.ruler2.pixelsPerUnit = 50F;
            this.ruler2.Size = new System.Drawing.Size(21, 592);
            this.ruler2.TabIndex = 21;
            // 
            // ruler1
            // 
            this.ruler1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ruler1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ruler1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ruler1.isVertical = false;
            this.ruler1.Location = new System.Drawing.Point(23, 595);
            this.ruler1.Name = "ruler1";
            this.ruler1.pixelsPerUnit = 50F;
            this.ruler1.Size = new System.Drawing.Size(839, 21);
            this.ruler1.TabIndex = 20;
            // 
            // ctrlScene
            // 
            this.ctrlScene.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlScene.Location = new System.Drawing.Point(24, 3);
            this.ctrlScene.Name = "ctrlScene";
            this.ctrlScene.Size = new System.Drawing.Size(838, 592);
            this.ctrlScene.TabIndex = 22;
            // 
            // CusDeginCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctrlScene);
            this.Controls.Add(this.ruler2);
            this.Controls.Add(this.ruler1);
            this.Name = "CusDeginCtrl";
            this.Size = new System.Drawing.Size(865, 617);
            this.ResumeLayout(false);

        }

        #endregion

        private Ruler.Ruler ruler2;
        private Ruler.Ruler ruler1;
        private SceneCtrl ctrlScene;
    }
}
