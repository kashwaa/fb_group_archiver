using FacebookGroupArchiver;
using FacebookGroupArchiver.Entities;
using FacebookGroupArchiver.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookGroupArchiver.UI
{
    public partial class Form1 : Form
    {
        IConfigurationManager _configManager;
        IPostsRepository _postsRepo;
        ISummaryRepository _summaryRepo;
        public Form1(IConfigurationManager configManager,IPostsRepository postsRepo,ISummaryRepository summaryRepo)
        {
            _configManager = configManager;
            _postsRepo = postsRepo;
            _summaryRepo = summaryRepo;
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            LoopProgressBar();
            GroupArchiver groupArchiver = new GroupArchiver();
            await groupArchiver.Archive(_configManager, _postsRepo, _summaryRepo);
            StopProgressBar();
        }
        CancellationTokenSource _tokenSource=new CancellationTokenSource();
        private async Task LoopProgressBar()
        {
            await Task.Run(() => {
                while (true)
                {
                    if (_tokenSource.IsCancellationRequested)
                    {
                        this.Invoke(new Action(() => { progressBar1.Value = progressBar1.Maximum; }));
                        break;
                    }
                    if(progressBar1.Value<progressBar1.Maximum)
                    {
                        this.Invoke(new Action(()=> { progressBar1.Increment(1); }));
                    }
                    else
                    {
                        this.Invoke(new Action(() => { progressBar1.Value = 0; }));
                    }
                    Thread.Sleep(1000);
                }
            }, _tokenSource.Token);
        }
        private void StopProgressBar()
        {
            _tokenSource.Cancel();
        }
    }
}
