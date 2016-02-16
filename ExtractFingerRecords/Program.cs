using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Neurotec.Licensing;

namespace ExtractFingerRecords
{
    class StateObject
    {
        public int LoopCounter;
        public int MaxPoolSize;
    }

    class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {
            //const string Components = "Biometrics.FingerExtraction,Biometrics.FingerMatching,Devices.FingerScanners,Images.WSQ";
            const string Components = "Biometrics.FingerExtraction,Images.WSQ";
            try
            {
                foreach (string component in Components.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    NLicense.ObtainComponents("/local", "5000", component);
                }

                //Helpers.ObtainLicenses(licensesMain);

                //try
                //{
                //    Helpers.ObtainLicenses(licensesBss);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.ToString());
                //}
                Run(args);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

                //MessageBox.Show("Error. Details: " + ex.Message, "Fingers Sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                NLicense.ReleaseComponents(Components);
                //Helpers.ReleaseLicenses(licensesMain);
                //Helpers.ReleaseLicenses(licensesBss);
            }

            //IList<string> licensesMain = new List<string>(new string[] { "FingersExtractor", "FingersMatcher" });
            //IList<string> licensesBss = new List<string>(new string[] { "FingersBSS" });

            //try
            //{
            //    Helpers.ObtainLicenses(licensesMain);

            //    try
            //    {
            //        Helpers.ObtainLicenses(licensesBss);

            //        //if (Data.NFExtractor == null)
            //        //{
            //        //    Data.NFExtractor = new NFExtractor();
            //        //    Data.UpdateNfe();
            //        //    //Data.UpdateNfeSettings();
            //        //}

            //        Run();
            //        //Run(Data.NFExtractor);
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
        static void Run(string[] args)
        {
            Int32 rowcount = 0;
            //for (int i = 0; i < 2; i++)
            //{
                try
                {
                    rowcount = ExtractionProcess.rowcount();
                    //break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    //Console.WriteLine("Time out, try again ");
                }
            //}

            if (rowcount == 0)
            {
                Console.WriteLine("------- Database is empty, press any key to close -------");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Row count: " + rowcount);

            //int limit = 10000;
            int limit;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["chunkSize"], out limit);
            if (limit == 0)
            {
                Console.WriteLine("------- Chunk size is invalid, press any key to close -------");
                Console.ReadKey();
                return;
            }

            int topindex = (int)(rowcount / limit);
            if (rowcount % limit != 0)
                topindex++;
            
            //topindex = 100;
            Task[] taskArray = new Task[topindex];
            //Task[] taskArray = new Task[1];
            int offset = 0;
            int offsetInsideChunk = 0;

            Stopwatch stw = new Stopwatch();
            stw.Start();

            bool go = false;
            if (args != null && args.Length != 0)
            {
                if (Int32.TryParse(args[0], out offset))
                {
                    if (offset < topindex)
                    {
                        offset *= limit;
                        limit = 1000;
                        taskArray = new Task[10];
                        go = true;
                    }

                    //Console.WriteLine(offset);
                }

                if (!go)
                {
                    Console.WriteLine(" --- Wrong parameter value, press any key to close ---");
                    Console.ReadKey();
                    return;
                }
            }
            //limit = 20;
            taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) =>
                {
                    StateObject state = obj as StateObject;

                    var process = new ExtractionProcess(state.MaxPoolSize);
                    //try
                    //{
                    //process.run(state.LoopCounter * limit + offset, state.LoopCounter * limit + limit, limit - offset, Thread.CurrentThread.ManagedThreadId);
                    //process.run(state.LoopCounter * limit + 90000, state.LoopCounter * limit + limit, limit);

                    process.run(state.LoopCounter * limit + offset + offsetInsideChunk, limit - offsetInsideChunk);

                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //}
                    //Console.WriteLine(process.run(1, 2, Thread.CurrentThread.ManagedThreadId));
                },
                new StateObject() { LoopCounter = i, MaxPoolSize = taskArray.Length * 2});
            }

            try
            {
                Task.WaitAll(taskArray);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Flatten().Message);
                //throw ex.Flatten();
                while ((ex is AggregateException) && (ex.InnerException != null))
                    ex = ex.InnerException;

                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine(" ----- Time elapsed: {0} ------------------ Press any key to close -----------------------", stw.Elapsed);
                Console.ReadKey();
            }
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
