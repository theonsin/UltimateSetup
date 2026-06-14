using System;
using System.Drawing;
using System.Windows.Forms;

namespace UltimateSetup
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Button btnInstall;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            btnInstall = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // btnInstall
            // 
            btnInstall.BackColor = Color.Transparent;
            btnInstall.BackgroundImageLayout = ImageLayout.None;
            btnInstall.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnInstall.FlatAppearance.MouseOverBackColor = Color.Wheat;
            btnInstall.FlatStyle = FlatStyle.Flat;
            btnInstall.Font = new Font("Corbel Light", 19F, FontStyle.Bold);
            btnInstall.ForeColor = Color.FromArgb(241, 197, 123);
            btnInstall.Location = new Point(972, 450);
            btnInstall.Name = "btnInstall";
            btnInstall.Size = new Size(160, 50);
            btnInstall.TabIndex = 1;
            btnInstall.Text = "INSTALL";
            btnInstall.UseVisualStyleBackColor = false;
            btnInstall.Click += btnInstall_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Corbel Light", 12F);
            label1.ForeColor = Color.WhiteSmoke;
            label1.Location = new Point(56, 43);
            label1.Name = "label1";
            label1.Size = new Size(217, 19);
            label1.TabIndex = 2;
            label1.Text = "Last Chaos Ultimate Installer v1.0";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Corbel Light", 12F);
            label2.ForeColor = Color.Gray;
            label2.Location = new Point(56, 62);
            label2.Name = "label2";
            label2.Size = new Size(140, 19);
            label2.TabIndex = 3;
            label2.Text = "Created by t.heonsin";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.ulcinst;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1280, 548);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnInstall);

            FormBorderStyle = FormBorderStyle.None;

            Name = "Form1";

            StartPosition = FormStartPosition.CenterScreen;

            Text = "Ultimate Setup";

            Icon = Icon.ExtractAssociatedIcon(
                Application.ExecutablePath);

            ResumeLayout(false);
            PerformLayout();
        }

        private Label label1;
        private Label label2;
    }
}