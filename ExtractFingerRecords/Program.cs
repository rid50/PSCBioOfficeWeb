using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Neurotec.Licensing;
using System.Runtime.InteropServices;
using System.Threading;

namespace ExtractFingerRecords
{
    class StateObject
    {
        public int LoopCounter;
        public int MaxPoolSize;
        public CancellationToken ct;
    }

    class Program
    {
        private delegate bool ConsoleCtrlHandlerDelegate(int sig);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerDelegate handler, bool add);

        static ConsoleCtrlHandlerDelegate _consoleCtrlHandler;

        private static CancellationTokenSource _tokenSource;

        private static int Usage()
        {
            Console.WriteLine("usage:");
            //Console.WriteLine("\t[chunk offset] [number of chunks] ( chunk size set in app.config file )");
            Console.WriteLine("\t[chunk offset] [number of chunks] ( chunk size is 10.000 database records )");
            Console.WriteLine("\t[chunk offset]     - chunk position in a database");
            Console.WriteLine("\t[number of chunks] - number of chunks to process");
            Console.WriteLine(" ------------------ Press any key to close -----------------------");
            Console.ReadKey();
            return 1;
        }

        //[STAThread]
        static int Main(string[] args)
        {
            if (args.Length > 2)
            {
                return Usage();
            }

            _consoleCtrlHandler += s =>
            {
                if (_tokenSource != null)
                {
                    _tokenSource.Cancel();
                }
                return true;
            };

            SetConsoleCtrlHandler(_consoleCtrlHandler, true);

            //const string Components = "Biometrics.FingerExtraction,Biometrics.FingerMatching,Devices.FingerScanners,Images.WSQ";
            //const string Components = "Biometrics.FingerExtractionFast,Biometrics.FingerSegmentationFast,Images.WSQ";
            const string Components = "Biometrics.FingerExtractionFast,Images.WSQ";
            try
            {
                foreach (string component in Components.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!NLicense.IsComponentActivated(component))
                    {
                        if (!NLicense.ObtainComponents("/local", "5000", component))
                            if (component.Equals("Biometrics.FingerExtractionFast"))
                                NLicense.ObtainComponents("/local", "5000", "Biometrics.FingerExtraction");
                    }
                }

                Run(args);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                Console.WriteLine(" ------------------ Press any key to close -----------------------");
                Console.ReadKey();
                return 1;
            }
            finally
            {
                NLicense.ReleaseComponents(Components);
            }

            return 0;
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

            int limit = 10000;
//            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["chunkSize"], out limit);
            //if (limit == 0)
            //{
            //    Console.WriteLine("------- Chunk size is invalid, press any key to close -------");
            //    Console.ReadKey();
            //    return;
            //}

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

            if (args != null && args.Length != 0)
            {
                bool go = false;
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

                int numofchunks = 0;
                if (args.Length > 1 && Int32.TryParse(args[1], out numofchunks))
                {
                    taskArray = new Task[numofchunks * 10];
                    if (offset + (taskArray.Length * 1000) > rowcount)
                        go = false;
                }

                if (!go)
                {
                    Console.WriteLine(" --- Wrong parameters value, number of records to process exceeds the number of records in the database, press any key to close ---");
                    Console.ReadKey();
                    return;
                }
            }

            _tokenSource = new CancellationTokenSource();
            CancellationToken ct = _tokenSource.Token;

            //return;
            //limit = 15;
            //taskArray = new Task[1];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) =>
                {
                    if (ct.IsCancellationRequested)
                        return;

                    StateObject state = obj as StateObject;

                    var process = new ExtractionProcess(state.MaxPoolSize, state.ct);
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
                new StateObject() { LoopCounter = i, MaxPoolSize = taskArray.Length * 2, ct = ct},
                ct,
                //_tokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
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
