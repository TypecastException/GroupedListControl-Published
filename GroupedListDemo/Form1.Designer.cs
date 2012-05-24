namespace GroupedListDemo
{
    partial class Form1
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
            this.chkSingleItemOnlyMode = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupListControl1 = new GroupedListControl.GroupListControl();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkSingleItemOnlyMode
            // 
            this.chkSingleItemOnlyMode.AutoSize = true;
            this.chkSingleItemOnlyMode.Location = new System.Drawing.Point(12, 52);
            this.chkSingleItemOnlyMode.Name = "chkSingleItemOnlyMode";
            this.chkSingleItemOnlyMode.Size = new System.Drawing.Size(224, 17);
            this.chkSingleItemOnlyMode.TabIndex = 1;
            this.chkSingleItemOnlyMode.Text = "Demo Single-Group-Only Expansion Mode";
            this.chkSingleItemOnlyMode.UseVisualStyleBackColor = true;
            this.chkSingleItemOnlyMode.CheckedChanged += new System.EventHandler(this.chkSingleItemOnlyMode_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Sans", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Grouped List Control Demo";
            // 
            // groupListControl1
            // 
            this.groupListControl1.AutoScroll = true;
            this.groupListControl1.BackColor = System.Drawing.SystemColors.Control;
            this.groupListControl1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.groupListControl1.Location = new System.Drawing.Point(12, 75);
            this.groupListControl1.Name = "groupListControl1";
            this.groupListControl1.SingleItemOnlyExpansion = false;
            this.groupListControl1.Size = new System.Drawing.Size(508, 363);
            this.groupListControl1.TabIndex = 0;
            this.groupListControl1.WrapContents = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(303, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Remove Column";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(415, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(105, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Remove Item";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkSingleItemOnlyMode);
            this.Controls.Add(this.groupListControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupedListControl.GroupListControl groupListControl1;
        private System.Windows.Forms.CheckBox chkSingleItemOnlyMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

