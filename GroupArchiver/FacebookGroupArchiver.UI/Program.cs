using FacebookGroupArchiver;
using FacebookGroupArchiver.Data;
using FacebookGroupArchiver.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookGroupArchiver.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            UnityContainer container = new UnityContainer();
            //container.RegisterInstance<Form1>(new Form1());
            container.RegisterInstance<IConfigurationManager>(new DiskConfigurationManager());
            container.RegisterType<IPostsRepository, DiskPostsRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISummaryRepository, DiskSummaryRepository>(new ContainerControlledLifetimeManager());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(container.Resolve<Form1>());
        }
    }
}
