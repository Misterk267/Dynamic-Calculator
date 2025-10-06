namespace Dynamic_Calculator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtInput = new TextBox();
            btnProcess = new Button();
            lblOutput = new Label();
            SuspendLayout();
            // 
            // txtInput
            // 
            txtInput.Location = new Point(12, 26);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(155, 23);
            txtInput.TabIndex = 0;
            // 
            // btnProcess
            // 
            btnProcess.Location = new Point(188, 15);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(107, 42);
            btnProcess.TabIndex = 1;
            btnProcess.Text = "Get Answer";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Location = new Point(12, 52);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(99, 15);
            lblOutput.TabIndex = 2;
            lblOutput.Text = "Output goes here";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(308, 145);
            Controls.Add(lblOutput);
            Controls.Add(btnProcess);
            Controls.Add(txtInput);
            Name = "Form1";
            Text = "Dynamic Calculator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtInput;
        private Button btnProcess;
        private Label lblOutput;
    }
}
