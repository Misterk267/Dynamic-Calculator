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
            btnTest = new Button();
            lstHistory = new ListBox();
            txtOutput = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            SuspendLayout();
            // 
            // txtInput
            // 
            txtInput.BorderStyle = BorderStyle.FixedSingle;
            txtInput.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtInput.Location = new Point(296, 26);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(156, 101);
            txtInput.TabIndex = 0;
            // 
            // btnProcess
            // 
            btnProcess.BackColor = Color.White;
            btnProcess.FlatStyle = FlatStyle.Flat;
            btnProcess.Location = new Point(297, 193);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(107, 42);
            btnProcess.TabIndex = 1;
            btnProcess.Text = "Evaluate";
            btnProcess.UseVisualStyleBackColor = false;
            btnProcess.Click += btnProcess_Click;
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblOutput.Location = new Point(297, 139);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(52, 15);
            lblOutput.TabIndex = 2;
            lblOutput.Text = "Answer:";
            // 
            // btnTest
            // 
            btnTest.BackColor = Color.White;
            btnTest.FlatStyle = FlatStyle.Flat;
            btnTest.Location = new Point(298, 374);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(106, 46);
            btnTest.TabIndex = 3;
            btnTest.Text = "Run Test";
            btnTest.UseVisualStyleBackColor = false;
            btnTest.Click += btnTest_Click;
            // 
            // lstHistory
            // 
            lstHistory.BorderStyle = BorderStyle.FixedSingle;
            lstHistory.Font = new Font("Courier New", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lstHistory.FormattingEnabled = true;
            lstHistory.ItemHeight = 14;
            lstHistory.Location = new Point(6, 26);
            lstHistory.Name = "lstHistory";
            lstHistory.Size = new Size(285, 394);
            lstHistory.TabIndex = 4;
            // 
            // txtOutput
            // 
            txtOutput.BorderStyle = BorderStyle.FixedSingle;
            txtOutput.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtOutput.Location = new Point(296, 157);
            txtOutput.Name = "txtOutput";
            txtOutput.ReadOnly = true;
            txtOutput.Size = new Size(156, 21);
            txtOutput.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(297, 8);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 6;
            label1.Text = "Input:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(296, 246);
            label2.Name = "label2";
            label2.Size = new Size(76, 15);
            label2.TabIndex = 7;
            label2.Text = "Instructions:";
            // 
            // label3
            // 
            label3.Location = new Point(296, 261);
            label3.Name = "label3";
            label3.Size = new Size(156, 110);
            label3.TabIndex = 8;
            label3.Text = "Enter a solvable expression into the input box and click the \"Evaluate\" button to get the answer. The history box will keep track of every step the program takes to evaluate the expression.\r\n";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(6, 8);
            label4.Name = "label4";
            label4.Size = new Size(66, 15);
            label4.TabIndex = 9;
            label4.Text = "Operation:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(133, 8);
            label5.Name = "label5";
            label5.Size = new Size(50, 15);
            label5.TabIndex = 10;
            label5.Text = "Output:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(457, 427);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtOutput);
            Controls.Add(lstHistory);
            Controls.Add(btnTest);
            Controls.Add(lblOutput);
            Controls.Add(btnProcess);
            Controls.Add(txtInput);
            Name = "Form1";
            Text = "Dynamic Calculator";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtInput;
        private Button btnProcess;
        private Label lblOutput;
        private Button btnTest;
        private ListBox lstHistory;
        private TextBox txtOutput;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}
