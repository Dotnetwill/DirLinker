namespace JunctionPointer.Views
{
    partial class ProgessView
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.m_Title = new System.Windows.Forms.Label();
            this.m_CurrentItem = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(429, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // m_Title
            // 
            this.m_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Title.Location = new System.Drawing.Point(12, 38);
            this.m_Title.Name = "m_Title";
            this.m_Title.Size = new System.Drawing.Size(429, 12);
            this.m_Title.TabIndex = 1;
            this.m_Title.Text = "label1";
            this.m_Title.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // m_CurrentItem
            // 
            this.m_CurrentItem.Location = new System.Drawing.Point(12, 50);
            this.m_CurrentItem.Name = "m_CurrentItem";
            this.m_CurrentItem.Size = new System.Drawing.Size(429, 38);
            this.m_CurrentItem.TabIndex = 2;
            this.m_CurrentItem.Text = "label2";
            this.m_CurrentItem.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ProgessView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 97);
            this.ControlBox = false;
            this.Controls.Add(this.m_CurrentItem);
            this.Controls.Add(this.m_Title);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgessView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Working...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label m_Title;
        private System.Windows.Forms.Label m_CurrentItem;
    }
}