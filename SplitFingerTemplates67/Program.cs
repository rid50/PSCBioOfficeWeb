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
using Neurotec.Licensing;

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

            const string Address = "/local";
            const string Port = "5000";
            const string Components = "Biometrics.FingerExtraction,Biometrics.FingerMatching,Devices.FingerScanners,Images.WSQ,Biometrics.FingerSegmentation";
            try
            {
                foreach (string component in Components.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    NLicense.ObtainComponents(Address, Port, component);
                }

                Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                NLicense.ReleaseComponents(Components);
            }

            ////IList<string> licensesMain = new List<string>(new string[] { "FingersExtractor", "FingersMatcher" });
            ////IList<string> licensesBss = new List<string>(new string[] { "FingersBSS" });
            //IList<string> licensesMain = new List<string>(new string[] { "Biometrics.FingerExtraction,Biometrics.FingerMatching,Devices.FingerScanners,Images.WSQ,Biometrics.FingerSegmentation" });
            //IList<string> licensesBss = new List<string>(new string[] { "FingersBSS" });

            //try
            //{
            //    Helpers.ObtainLicenses(licensesMain);

            //    try
            //    {
            //        Helpers.ObtainLicenses(licensesBss);

            //        if (Data.NFExtractor == null)
            //        {
            //            Data.NFExtractor = new NFExtractor();
            //            Data.UpdateNfe();
            //            //Data.UpdateNfeSettings();
            //        }

            //        Run(Data.NFExtractor);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Error. Details: " + ex.Message);
            //}
            //finally
            //{
            //    Helpers.ReleaseLicenses(licensesMain);
            //    Helpers.ReleaseLicenses(licensesBss);
            //}

            //var worker = new BackgroundWorkerProcess();
            //worker.startProcess();
        }

        //static void Run(NFExtractor NFExtractor)
        static void Run()
        {
            Int32 rowcount = 0;
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    rowcount = SerializationProcess.rowcount();
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Time out, try again ");
                }
            }

            if (rowcount == 0)
                return;

            Console.WriteLine("Row count: " + rowcount);
            //return;
            int limit = 1000;
            int topindex = (int)(rowcount / limit + 1);
            //topindex = 100;
            //Task[] taskArray = new Task[topindex];
            Task[] taskArray = new Task[6];

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
                    //process.run(state.LoopCounter * limit, state.LoopCounter * limit + limit, limit, Thread.CurrentThread.ManagedThreadId);
                    //process.run(state.LoopCounter * limit + 90000, state.LoopCounter * limit + limit, limit);
                    process.run(state.LoopCounter * limit + 90000, state.LoopCounter * limit + limit, limit);
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
