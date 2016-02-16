using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace SplitFingerTemplates
{
    class BackgroundWorkerProcess
    {
        private BackgroundWorker backgroundWorkerProcess;
        private int _max = 100;

        public BackgroundWorkerProcess()
        {
            backgroundWorkerProcess = new BackgroundWorker();
            backgroundWorkerProcess.WorkerSupportsCancellation = true;
            backgroundWorkerProcess.WorkerReportsProgress = true;
            backgroundWorkerProcess.DoWork += new DoWorkEventHandler(backgroundWorkerProcess_DoWork);
            backgroundWorkerProcess.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerProcess_RunWorkerCompleted);
            backgroundWorkerProcess.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerProcess_ProgressChanged);
        }

        public void startProcess()
        {
            if (backgroundWorkerProcess.IsBusy)
                return;

            backgroundWorkerProcess.RunWorkerAsync("My Name");
        }

        public void stopProcess()
        {
            if (backgroundWorkerProcess.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                backgroundWorkerProcess.CancelAsync();
            }

        }

        private void backgroundWorkerProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = e.Argument.ToString();

            // This method will run on a thread other than the UI thread.
            // Be sure not to manipulate any Windows Forms controls created
            // on the UI thread from this method.

            BackgroundWorker worker = sender as BackgroundWorker;

            worker.ReportProgress(0, "Working...");

            for (int i = 1; i < _max; ++i)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Introduce some delay to simulate a more complicated calculation.
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress((100 * i) / _max, "Working...");
                }
            }

            worker.ReportProgress(100, "Complete!");
        }

        private void backgroundWorkerProcess_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage.ToString() + "%");
        }

        private void backgroundWorkerProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                Console.WriteLine("Canceled, thread name -  " + Thread.CurrentThread.Name);
            }
            else if (e.Error != null)
            {
                Console.WriteLine("Error, thread name -  " + Thread.CurrentThread.Name + "   " + e.Error.Message);
            }
            else
            {
                Console.WriteLine("Done, thread name -  " + Thread.CurrentThread.Name);
            }
        }
    }
}
