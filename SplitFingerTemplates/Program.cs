using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using Neurotec.Biometrics;

namespace SplitFingerTemplates
{
    class StateObject
    {
        public int LoopCounter;
    }

    class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {

            IList<string> licensesMain = new List<string>(new string[] { "FingersExtractor", "FingersMatcher" });
            IList<string> licensesBss = new List<string>(new string[] { "FingersBSS" });

            try
            {
                Helpers.ObtainLicenses(licensesMain);

                try
                {
                    Helpers.ObtainLicenses(licensesBss);

                    //if (Data.NFExtractor == null)
                    //{
                    //    Data.NFExtractor = new NFExtractor();
                    //    Data.UpdateNfe();
                    //    //Data.UpdateNfeSettings();
                    //}

                    Run();
                    //Run(Data.NFExtractor);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error. Details: " + ex.Message);
            }
            finally
            {
                Helpers.ReleaseLicenses(licensesMain);
                Helpers.ReleaseLicenses(licensesBss);
            }

            //var worker = new BackgroundWorkerProcess();
            //worker.startProcess();
        }

        //static void Run(NFExtractor NFExtractor)
        static void Run()
        {
            //int limit = 100;
            //Task[] taskArray = new Task[1];

            //int offset = 1000;
            Int32 rowcount = SerializationProcess.rowcount();
            int limit = 10000;
            int topindex = (int)(rowcount/limit + 1);
            //topindex = 100;
            Task[] taskArray = new Task[topindex];
            //Task[] taskArray = new Task[8];

            Stopwatch stw = new Stopwatch();
            stw.Start();

            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) =>
                {
                    StateObject state = obj as StateObject;

                    var process = new SerializationProcess();
                    //try
                    //{
                    //process.run(state.LoopCounter * limit + offset, state.LoopCounter * limit + limit, limit - offset, Thread.CurrentThread.ManagedThreadId);
                    process.run(state.LoopCounter * limit, state.LoopCounter * limit + limit, limit);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //}
                    //Console.WriteLine(process.run(1, 2, Thread.CurrentThread.ManagedThreadId));
                },
                new StateObject() {LoopCounter = i});
            }
            try
            {
                Task.WaitAll(taskArray);
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten();

                //Console.WriteLine(ex.Message);
            }

            Console.WriteLine(" ----- Time elapsed: {0}", stw.Elapsed);

        }


/*
        var results = new Double[taskArray.Length];
        Double sum = 0;

        for (int i = 0; i < taskArray.Length; i++) {
            results[i] = taskArray[i].Result;
            Console.Write("{0:N1} {1}", results[i], 
                              i == taskArray.Length - 1 ? "= " : "+ ");
            sum += results[i];
*/


            /*
                //for (unsigned int i = 0; i < 8; i++) {
                parallel_for(0u, 8u, [](unsigned int i) {
                    unsigned int limit = 10000;
                    run(i * limit, i * limit + limit);
                    cout << "---------" << std::endl; 
                });
            */
    }
}
