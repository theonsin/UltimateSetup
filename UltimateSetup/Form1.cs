using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Net.Http;


namespace UltimateSetup
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer? timer;

        private double progress = 0;
        private double targetProgress = 0;

        private Panel? progressPanel;
        private Label? lblStatus;
        private Label? lblCurrentFile;
        private Label lblClose;

        private TextBox? txtInstallPath;
        private Button? btnBrowse;
        private Panel? pathPanel;

        private Label? lblDiskSpace;


        private bool isInstalling = false;

        private DateTime lastUiUpdate = DateTime.MinValue;

        public Form1()
        {
            InitializeComponent();

            lblDiskSpace = new Label();


            lblDiskSpace.Location =
                new Point(192, 425);

            lblDiskSpace.AutoSize = true;

            lblDiskSpace.ForeColor =
                Color.FromArgb(180, 180, 180);
            lblDiskSpace.BackColor = Color.Transparent;
            lblDiskSpace.Parent = this;

            lblDiskSpace.Font =
                new Font(
                    "Segoe UI",
                    8.5f,
                    FontStyle.Regular);

            lblDiskSpace.Font =
                new Font("Segoe UI", 8);

            Controls.Add(lblDiskSpace);

            InitUI();

        lblCurrentFile = new Label();

            lblCurrentFile.AutoSize = false;
            lblCurrentFile.Location = new Point(192, 505);
            lblCurrentFile.Size = new Size(700, 20);
            lblCurrentFile.ForeColor = Color.Gray;
            lblCurrentFile.BackColor = Color.Transparent;
            lblCurrentFile.Font = new Font("Segoe UI", 8);
            lblCurrentFile.Text = "";

            Controls.Add(lblCurrentFile);

            // ================= CLOSE BUTTON =================

            lblClose = new Label();

            lblClose.AutoSize = false;
            lblClose.Size = new Size(40, 40);

            lblClose.Location =
                new Point(
                    ClientSize.Width - 50,
                    10
                );

            lblClose.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            lblClose.Text = "✕";

            lblClose.TextAlign =
                ContentAlignment.MiddleCenter;

            lblClose.Font =
                new Font(
                    "Corbel Light",
                    18,
                    FontStyle.Bold
                );

            lblClose.ForeColor =
                Color.FromArgb(
                    241,
                    197,
                    123
                );

            lblClose.BackColor =
                Color.Transparent;

            lblClose.Cursor =
                Cursors.Hand;

            lblClose.Click += lblClose_Click;

            lblClose.MouseEnter +=
                (s, e) =>
                {
                    lblClose.ForeColor = Color.White;
                };

            lblClose.MouseLeave +=
                (s, e) =>
                {
                    lblClose.ForeColor =
                        Color.FromArgb(
                            241,
                            197,
                            123
                        );
                };

            Controls.Add(lblClose);
            lblClose.BringToFront();

            DoubleBuffered = true;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);


            UpdateStyles();
        }

        private void UpdateDiskSpaceInfo()
        {
            try
            {
                if (txtInstallPath == null)
                {
                    MessageBox.Show("txtInstallPath == null");
                    return;
                }

                if (lblDiskSpace == null)
                {
                    MessageBox.Show("lblDiskSpace == null");
                    return;
                }

                string path =
                txtInstallPath.Text;

                DriveInfo drive =
                    new DriveInfo(
                        Path.GetPathRoot(path)!);

                double freeGb =
                    drive.AvailableFreeSpace /
                    1024d / 1024d / 1024d;

                lblDiskSpace.Text =
                    $"{freeGb:F0} GB Available (20 GB Required)";

                lblDiskSpace.ForeColor =
                    freeGb >= 20
                    ? Color.FromArgb(180, 180, 180)
                    : Color.FromArgb(220, 120, 120);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void lblClose_Click(
            object? sender,
            EventArgs e)
        {
            this.Opacity = 0.90;

            try
            {
                using (ConfirmExit dlg =
                    new ConfirmExit())
                {
                    if (dlg.ShowDialog(this) ==
                        DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                }
            }
            finally
            {
                this.Opacity = 1.0;
            }
        }

        private void BtnBrowse_Click(
            object? sender,
            EventArgs e)
        {
            using FolderBrowserDialog dialog =
                new FolderBrowserDialog();

            dialog.Description =
                "Select installation folder";

            dialog.SelectedPath =
                txtInstallPath!.Text;

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                txtInstallPath.Text =
                    Path.Combine(
                        dialog.SelectedPath,
                        "Last Chaos Ultimate");

                UpdateDiskSpaceInfo();
            }
        }

        private void InitUI()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 15;
            timer.Tick += Timer_Tick;
            timer.Start();

            progressPanel = new Panel();
            progressPanel.Location =
                new Point(
                    (int)(ClientSize.Width * 0.15),
                    (int)(ClientSize.Height * 0.86)
                );

            progressPanel.Size =
                new Size(
                    (int)(ClientSize.Width * 0.55),
                    20
                );
            progressPanel.BackColor = Color.FromArgb(40, 40, 40);
            progressPanel.Paint += ProgressPanel_Paint;

            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                progressPanel,
                new object[] { true });

            Controls.Add(progressPanel);

            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(192, 450);
            lblStatus.ForeColor = Color.White;
            lblStatus.Font = new Font("Segoe UI", 10);
            lblStatus.Text = "Ready...";
            lblStatus.BackColor = Color.Transparent;
            Controls.Add(lblStatus);

            // ================= INSTALL PATH =================

            pathPanel = new Panel();

            pathPanel.Location = new Point(192, 395);
            pathPanel.Size = new Size(650, 22);

            pathPanel.BackColor =
                Color.FromArgb(40, 40, 40);

            // ================= ROUND CORNERS =================

            GraphicsPath panelPath = new GraphicsPath();

            panelPath.AddArc(0, 0, 12, 12, 180, 90);
            panelPath.AddArc(pathPanel.Width - 12, 0, 12, 12, 270, 90);
            panelPath.AddArc(pathPanel.Width - 12, pathPanel.Height - 12, 12, 12, 0, 90);
            panelPath.AddArc(0, pathPanel.Height - 12, 12, 12, 90, 90);

            panelPath.CloseFigure();

            pathPanel.Region =
                new Region(panelPath);

            // ================= GOLD BORDER =================

            pathPanel.Paint += (s, e) =>
            {
                Color borderColor =
                    isInstalling
                    ? Color.FromArgb(60, 60, 60)
                    : Color.FromArgb(241, 197, 123);

                using Pen pen =
                    new Pen(borderColor, 1);

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                Rectangle rect =
                    new Rectangle(
                        0,
                        0,
                        pathPanel.Width - 1,
                        pathPanel.Height - 1);

                using GraphicsPath borderPath =
                    RoundedRect(
                        rect.X,
                        rect.Y,
                        rect.Width,
                        rect.Height,
                        6);

                e.Graphics.DrawPath(
                    pen,
                    borderPath);
            };

            Controls.Add(pathPanel);

            txtInstallPath = new TextBox();

            txtInstallPath.Location =
                new Point(8, 2);

            txtInstallPath.Size =
                new Size(600, 18);

            txtInstallPath.BorderStyle =
                BorderStyle.None;

            txtInstallPath.BackColor =
                Color.FromArgb(40, 40, 40);

            txtInstallPath.ForeColor =
                Color.FromArgb(241, 197, 123);

            txtInstallPath.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Regular);

            txtInstallPath.Text =
                @"C:\Games\Last Chaos Ultimate";

            pathPanel.Controls.Add(txtInstallPath);

            UpdateDiskSpaceInfo();

            btnBrowse = new Button();

            btnBrowse.Text = "📁";

            btnBrowse.Location =
                new Point(620, -3);

            btnBrowse.Size =
                new Size(24, 24);

            btnBrowse.Font =
                new Font(
                    "Segoe UI Symbol",
                    10F,
                    FontStyle.Regular);



            btnBrowse.FlatStyle = FlatStyle.Flat;

            btnBrowse.FlatAppearance.BorderSize = 0;

            btnBrowse.FlatAppearance.MouseOverBackColor =
                Color.Transparent;

            btnBrowse.FlatAppearance.MouseDownBackColor =
                Color.Transparent;

            btnBrowse.BackColor =
                Color.FromArgb(40, 40, 40);

            btnBrowse.UseVisualStyleBackColor = false;

            btnBrowse.BackColor = Color.Transparent;


            btnBrowse.ForeColor =
                Color.FromArgb(241, 197, 123);

            btnBrowse.Cursor =
                Cursors.Hand;

            btnBrowse.Click += BtnBrowse_Click;

            btnBrowse.MouseEnter += (s, e) =>
            {
                btnBrowse.ForeColor = Color.White;
            };

            btnBrowse.MouseLeave += (s, e) =>
            {
                btnBrowse.ForeColor =
                    Color.FromArgb(241, 197, 123);
            };

            pathPanel.Controls.Add(btnBrowse);

        }


        // ================= INSTALL =================
        private async void btnInstall_Click(object sender, EventArgs e)
        {
            try
            {
                progress = 0;
                targetProgress = 0;

                isInstalling = true;

                pathPanel?.Invalidate();

                btnInstall.Enabled = false;
                btnInstall.ForeColor = Color.FromArgb(120, 120, 120);

                if (pathPanel != null)
                {
                    pathPanel.BackColor =
                        Color.FromArgb(40, 40, 40);

                    pathPanel.ForeColor =
                        Color.FromArgb(80, 80, 80);
                }

                if (btnBrowse != null)
                {
                    btnBrowse.Enabled = false;
                    btnBrowse.ForeColor =
                        Color.FromArgb(120, 120, 120);
                }

                if (txtInstallPath != null)
                {
                    txtInstallPath.ReadOnly = true;
                    txtInstallPath.TabStop = false;
                    txtInstallPath.Cursor = Cursors.Default;
                    txtInstallPath.ForeColor =
                        Color.FromArgb(120, 120, 120);
                }

                this.ActiveControl = null;



                string gameFolder =
                    txtInstallPath!.Text;

                // ================= PATH CHECK =================

                if (string.IsNullOrWhiteSpace(gameFolder))
                {
                    MessageBox.Show(
                        "Please select installation folder.");

                    isInstalling = false;

                    pathPanel?.Invalidate();

                    btnInstall.Enabled = true;

                    if (btnBrowse != null)
                        btnBrowse.Enabled = true;

                    return;
                }

                if (!Path.IsPathRooted(gameFolder))
                {
                    MessageBox.Show(
                        "Invalid installation path.");

                    isInstalling = false;

                    pathPanel?.Invalidate();

                    btnInstall.Enabled = true;

                    if (btnBrowse != null)
                        btnBrowse.Enabled = true;

                    return;
                }

                string root =
    Path.GetPathRoot(gameFolder)!;

                if (!Directory.Exists(root))
                {
                    MessageBox.Show(
                        "Selected drive does not exist.");

                    isInstalling = false;

                    pathPanel?.Invalidate();

                    btnInstall.Enabled = true;

                    if (btnBrowse != null)
                        btnBrowse.Enabled = true;

                    return;
                }

                // ================= FREE SPACE CHECK =================

                DriveInfo drive =
                    new DriveInfo(
                        Path.GetPathRoot(gameFolder)!);

                long freeSpace =
                    drive.AvailableFreeSpace;

                long requiredSpace =
                    20L * 1024 * 1024 * 1024; // 20 GB

                if (freeSpace < requiredSpace)
                {
                    MessageBox.Show(
                        $"Ultimate Last Chaos requires at least 20 GB of free disk space.\n\n" +
                        $"Available space: {(freeSpace / 1024d / 1024d / 1024d):F1} GB",
                        "Insufficient Disk Space",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    isInstalling = false;

                    pathPanel?.Invalidate();

                    btnInstall.Enabled = true;

                    if (btnBrowse != null)
                        btnBrowse.Enabled = true;

                    return;
                }

                // ================= EXISTING INSTALL CHECK =================

                string launcherPath =
                    Path.Combine(
                        gameFolder,
                        "UltimateLauncher.exe");

                if (File.Exists(launcherPath))
                {
                    var result =
                        MessageBox.Show(
                            "Game already exists in this folder.\n\nContinue installation?",
                            "Ultimate Setup",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        isInstalling = false;

                        pathPanel?.Invalidate();

                        btnInstall.Enabled = true;

                        if (btnBrowse != null)
                        {
                            btnBrowse.Enabled = true;

                            btnBrowse.ForeColor =
                                Color.FromArgb(
                                    241,
                                    197,
                                    123);
                        }

                        if (txtInstallPath != null)
                        {
                            txtInstallPath.ReadOnly = false;

                            txtInstallPath.ForeColor =
                                Color.FromArgb(
                                    241,
                                    197,
                                    123);
                        }

                        return;
                    }
                }
                Directory.CreateDirectory(gameFolder);

                string[] sources =
                {
                    "http://151.80.20.168/patches/downloads/archive1.zip",
                    "http://151.80.20.168/patches/downloads/archive2.zip"
                };

                string[] files =
                {
                    Path.Combine(gameFolder, "archive1.zip"),
                    Path.Combine(gameFolder, "archive2.zip")
                };
                long[] archiveSizes =
                {
                    6991218688, // archive1.zip
                    90789888  // archive2.zip
                };

                // ================= COPY =================
                using (HttpClient client = new HttpClient())
                {
                    for (int i = 0; i < sources.Length; i++)

                    {
                        lblCurrentFile!.Text =
                        Path.GetFileName(
                            sources[i]
                        );

                        var response = await client.GetAsync(
                            sources[i],
                            HttpCompletionOption.ResponseHeadersRead);

                        long totalBytes = archiveSizes[i];

                        response.EnsureSuccessStatusCode();


                        long totalRead = 0;

                        using Stream input =
                            await response.Content.ReadAsStreamAsync();

                        using FileStream output =
                            new FileStream(
                                files[i],
                                FileMode.Create,
                                FileAccess.Write,
                                FileShare.None);

                        byte[] buffer = new byte[64 * 1024];

                        int read;

                        while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await output.WriteAsync(buffer, 0, read);

                            totalRead += read;

                            if (totalBytes > 0)
                            {
                                double fileProgress =
                                    (double)totalRead / totalBytes;

                                targetProgress =
                                    (i * 40.0 / sources.Length) +
                                    (fileProgress * 40.0 / sources.Length);

                                lblStatus!.Text =
                                    $"Downloading {i + 1}/{sources.Length} ({(int)(fileProgress * 100)}%)";
                            }
                            else
                            {
                                double mb =
                                    totalRead / 1024d / 1024d;

                                lblStatus!.Text =
                                    $"Downloading {i + 1}/{sources.Length} ({mb:F1} MB)";
                            }
                        }
                    }
                }

                // ================= EXTRACT =================
                for (int i = 0; i < files.Length; i++)
                {
                    using (ZipArchive archive =
                        ZipFile.OpenRead(files[i]))
                    {
                        int totalFiles = archive.Entries.Count;
                        int extracted = 0;

                        foreach (var entry in archive.Entries)
                        {
                            lblCurrentFile!.Text =
                                entry.FullName;

                            Application.DoEvents();

                            string destination =
                                Path.Combine(
                                    gameFolder,
                                    entry.FullName
                                );

                            string? dir =
                                Path.GetDirectoryName(
                                    destination
                                );

                            if (!string.IsNullOrEmpty(dir))
                                Directory.CreateDirectory(dir);

                            if (!string.IsNullOrEmpty(entry.Name))
                                entry.ExtractToFile(destination, true);

                            extracted++;

                            double extractProgress =
                                (double)extracted / totalFiles;

                            targetProgress =
                                40 +
                                (i * 40.0 / files.Length) +
                                (extractProgress * 40.0 / files.Length);

                            lblStatus!.Text =
                                $"Extracting {i + 1}/{files.Length} ({(int)(extractProgress * 100)}%)";
                        }
                    }

                    // архив уже закрыт
                    File.Delete(files[i]);
                }


                // ================= FINAL =================

                lblCurrentFile!.Text =
                    "Installation completed";

                lblStatus!.Text =
                    "Starting game...";

                targetProgress = 100;

                while (progress < 99)
                {
                    await System.Threading.Tasks.Task.Delay(10);
                }

                string exePath =
                    Path.Combine(
                        gameFolder,
                        "UltimateLauncher.exe"
                    );

                if (File.Exists(exePath))
                {
                    var startInfo =
                        new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = exePath,
                            UseShellExecute = true,
                            Verb = "runas"
                        };

                    System.Diagnostics.Process.Start(startInfo);
                }
                else
                {
                    MessageBox.Show(
                        "UltimateLauncher.exe not found"
                    );
                }
                isInstalling = false;
                pathPanel?.Invalidate();

                Close();
            }
            catch (Exception ex)
            {
                isInstalling = false;

                pathPanel?.Invalidate();

                btnInstall.Enabled = true;

                if (btnBrowse != null)
                {
                    btnBrowse.Enabled = true;

                    btnBrowse.ForeColor =
                        Color.FromArgb(
                            241,
                            197,
                            123);
                }

                if (txtInstallPath != null)
                {
                    txtInstallPath.ReadOnly = false;

                    txtInstallPath.ForeColor =
                        Color.FromArgb(
                            241,
                            197,
                            123);
                }

                MessageBox.Show(
                    ex.Message,
                    "Installation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ================= SMOOTH =================
        private void Timer_Tick(
    object? sender,
    EventArgs e)
        {
            progress += (targetProgress - progress) * 0.08;

            if (Math.Abs(targetProgress - progress) < 0.1)
                progress = targetProgress;

            progressPanel?.Invalidate();
        }


        // ================= DRAW =================
        private void ProgressPanel_Paint(
    object? sender,
    PaintEventArgs e)
        {
            if (progressPanel == null)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = progressPanel.Width;
            int h = progressPanel.Height;

            g.Clear(progressPanel.BackColor);

            int radius = 10;

            using (GraphicsPath bg = RoundedRect(0, 0, w, h, radius))
            using (Brush b = new SolidBrush(Color.FromArgb(60, 60, 60)))
                g.FillPath(b, bg);

            int fillWidth = (int)(w * (progress / 100.0));

            if (fillWidth < 2) return;

            using (GraphicsPath fg = RoundedRect(0, 0, fillWidth, h, radius))
            using (LinearGradientBrush br = new LinearGradientBrush(
                new Rectangle(0, 0, fillWidth, h),
                Color.FromArgb(241, 197, 123),
                Color.FromArgb(255, 230, 160),
                LinearGradientMode.Horizontal))
            {
                g.FillPath(br, fg);
            }
        }

        // ================= ROUND RECT =================
        private GraphicsPath RoundedRect(int x, int y, int w, int h, int r)
        {
            GraphicsPath path = new GraphicsPath();
            int rr = r * 2;

            w = Math.Max(w, rr);
            h = Math.Max(h, rr);

            path.AddArc(x, y, rr, rr, 180, 90);
            path.AddArc(x + w - rr, y, rr, rr, 270, 90);
            path.AddArc(x + w - rr, y + h - rr, rr, rr, 0, 90);
            path.AddArc(x, y + h - rr, rr, rr, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}