﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace SecureSimulator
{

    public static class SimulatorLibrary
    {
        //meta data about the last run of "update global state"
        public static Int32 numberOn;
        public static Int32 numberFlipped;
        public static new List<UInt32> big5List = new List<UInt32> { 15169, 8075, 32934, 22822, 20940 };

        public static char[] space = { ' ' };

        static bool justProgress = true;
        //what utility function to use.
        //default to "basic utility" (size of customer subtree)
        public static UtilityComputations.ComputeUtility utilityComputation = new UtilityComputations.ComputeUtility(UtilityComputations.basicUtility);

        //whether or not stubs break ties based on security
        static bool stubsDontCare = false;

        //whether to use the per destination weights or not.
        public static bool useMiniDWeight = false;

        public static bool useLessHarshWeight = false;//to use this, useminidweight also nees to be true.
          /// <summary>
        /// static constructor
        /// </summary>
        static SimulatorLibrary()
        {
            if (File.Exists("paramsFile.txt"))
            {
                StreamReader input = new StreamReader("paramsFile.txt");
                string setUtilityTo ="";
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine().ToLower();
                    //hashing
                     if (line.IndexOf("setjustprogress") == 0)
                    {
                        bool inputJustProgress;
                        if (readParameter(line, out inputJustProgress))
                            justProgress = inputJustProgress;
                    }
                     //stubs dont care
                     if (line.IndexOf("setstubsdontcare") == 0)
                     {
                         bool inputStubsDontCare;
                         if (readParameter(line, out inputStubsDontCare))
                             stubsDontCare = inputStubsDontCare;
                     }
                    //use miniD weight
                     if (line.IndexOf("setuseminidweight") == 0)
                     {
                         bool inputUseMiniDWeight;
                         if (readParameter(line, out inputUseMiniDWeight))
                             useMiniDWeight = inputUseMiniDWeight;
                     }
                     //use less harsh weight
                     if (line.IndexOf("setuselessharshweight") == 0)
                     {
                         bool inputUseLessHarshWeight;
                         if (readParameter(line, out inputUseLessHarshWeight))
                             useLessHarshWeight = inputUseLessHarshWeight;
                     }
                     //utility
                     else if (line.IndexOf("setutility") == 0)
                     {
                         UtilityComputationType inputUtilityComputation;
                         if (readParameter(line, out inputUtilityComputation))
                         {
                             setUtilityComputation(inputUtilityComputation);
                             switch (inputUtilityComputation)
                             {
                                 case UtilityComputationType.basic:
                                     setUtilityTo = "basic";
                                     break;
                                 case UtilityComputationType.incoming:
                                     setUtilityTo = "incoming";
                                     break;
                                 case UtilityComputationType.outgoing:
                                     setUtilityTo = "outgoing";
                                     break;
                             }
                         }
                     }

                }
                input.Close();
                Console.WriteLine("*******************");
                Console.WriteLine("Done reading parameters in SimulatorLibrary.");
          
                Console.WriteLine("JustProgress is " + justProgress);
                Console.WriteLine("Utility computation was set to :" + setUtilityTo);
                Console.WriteLine("Stubs dont care was set to :" + stubsDontCare);
                Console.WriteLine("setuseminidweight was set to :" + useMiniDWeight);
                Console.WriteLine("uselessharshweight was set to :" + useLessHarshWeight
                    + ". If this is true, it only has an effect if setuseminidweight is also true" );
                Console.WriteLine("*******************");
            }
        }

        /// <summary>
        /// parses a parameter line, prints relevant error messages, returns false
        /// if an error is encountered otherwise puts the parameter into the out bool.
        /// </summary>
        /// <param name="parameterLine"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool readParameter(string parameterLine,out bool parameter)
        {
            string[] pieces = parameterLine.Split(space, StringSplitOptions.RemoveEmptyEntries);
            if (pieces.Length < 2)
            {
                Console.WriteLine("Error processing parameter line: " + parameterLine);
                parameter = false;
                return false;
            }

            if (pieces[1].IndexOf("true") >= 0)
                parameter = true;
            else
                parameter = false;

            return true;

        }

        /// <summary>
        /// parses a parameter line, prints relevant error messages, returns false
        /// if an error is encountered otherwise puts the parameter into the out bool.
        /// </summary>
        /// <param name="parameterLine"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool readParameter(string parameterLine, out UtilityComputationType parameter)
        {
            string[] pieces = parameterLine.Split(space, StringSplitOptions.RemoveEmptyEntries);
            if (pieces.Length < 2)
            {
                Console.WriteLine("Error processing parameter line: " + parameterLine);
                parameter = UtilityComputationType.basic;
                return false;
            }

            if (pieces[1].IndexOf("incoming") >= 0)
                parameter = UtilityComputationType.incoming;
            else if (pieces[1].IndexOf("outgoing") >= 0)
                parameter = UtilityComputationType.outgoing;
            else if (pieces[1].IndexOf("basic") >= 0)
                parameter = UtilityComputationType.basic;
            else
            {
                Console.WriteLine("error unidentified utility computation type: " + parameterLine);
                parameter = UtilityComputationType.basic;
                return false;
            }


            return true;

        }

        public static void setUtilityComputation(UtilityComputationType utilityType)
        {
            switch (utilityType)
            {
                case UtilityComputationType.basic:
                    utilityComputation = new UtilityComputations.ComputeUtility(UtilityComputations.basicUtility);
                    break;
                case UtilityComputationType.incoming:
                    utilityComputation = new UtilityComputations.ComputeUtility(UtilityComputations.incomingUtility);
                    break;
                case UtilityComputationType.outgoing:
                    utilityComputation = new UtilityComputations.ComputeUtility(UtilityComputations.outgoingUtility);
                    break;
                case UtilityComputationType.source:
                    utilityComputation = new UtilityComputations.ComputeUtility(UtilityComputations.sourceUtility);
                    break;
            }
            
        }

        public static void setJustProgress(bool setTo)
        {
            justProgress = setTo;
        }
        public static void setHash(bool setTo)
        {
            ModifiedBfs.Hash = setTo;
        }

        public static void setOnlyStubs(bool setTo)
        {
            ModifiedBfs.OnlyNonStubs = setTo;
        }

        public static UInt32 estimateRunTime(MiniDestination miniDestination, GlobalState globalState)
        {
            if (!globalState.S[miniDestination.destination])
            {
                //this destination is off no workers need to be launched.
                return 0;
            }
            Destination d = new Destination(miniDestination);
            bool[] copyOfS = (bool[])globalState.S.Clone();  //shallow copy 

            /** update paths and compute utility based on current global state */
            d.UpdatePaths(copyOfS);
           
            UInt32 estimatedRunTime = 0;
            for (int i = 0; i < globalState.nonStubs.Count; i++)
            {
                UInt32 currASN = globalState.nonStubs[i];
                if (hasSecPThroughParent(ref d, currASN)&&(!justProgress || (justProgress&&!globalState.S[currASN])))//only compute on people with secP through a parent.
                {
                    estimatedRunTime++;
                }
            }
            return estimatedRunTime;
        }

        /// <summary>
        /// PHILLIPA ADDED: miniDWeight boolean parameter. If it is true it will use the Weight from the miniD object
        /// (ie., the pairwise decreasing weighting on pathlen).
        /// otherwise it will use the weight from the globalstate object.
        /// </summary>
        /// <param name="miniDestination"></param>
        /// <param name="globalState"></param>
        /// <param name="useMiniDWeight"></param>
        /// <returns></returns>
        public static Message ComputeOnDestination(MiniDestination miniDestination, GlobalState globalState)
        {
            Message result = new Message();
            Destination d = new Destination(miniDestination);

            //added by Phillipa for per destination weighting that 
            //accounts for sourced traffic weight.
            UInt16[] WToUse = new UInt16[globalState.W.Length];

            if (useMiniDWeight)
            {
                for (int i = 0; i < WToUse.Length; i++)
                {
                    WToUse[i] = (UInt16)Math.Round(globalState.W[i] * miniDestination.W[i]);

                }
            }
            else
            {
                for (int i = 0; i < WToUse.Length; i++)
                    WToUse[i] = globalState.W[i];
            }

            for (int i = 0; i < 9; i++)
                Console.WriteLine("AS: " + i + " W " + WToUse[i]);
            bool[] copyOfS = (bool[])globalState.S.Clone();  //shallow copy 
            int numberOfworkers = 0;

            /** update paths and compute utility based on current global state */
            d.UpdatePaths(copyOfS);


            //ADDED BY PHILLIPA:
            //use the W vector chosen using per destination switch.
            d.ComputeU(WToUse);
           

            result.UBefore = d.U;

            if (!globalState.S[d.destination])
            {
                //this destination is off, utilities stay the same.
                result.UAfter = result.UBefore;
                return result;
            }
            result.UAfter = new Int64[result.UBefore.Length];

            Worker w = new Worker();
            for (int i = 0; i < globalState.nonStubs.Count; i++)
            {
                UInt32 currASN = globalState.nonStubs[i];
                if (hasSecPThroughParent(ref d, currASN) && (!justProgress || (justProgress && !globalState.S[currASN])))//only compute on people with secP through a parent.
                //only compute on people with secP through a parent.
                {

                    //ADDED BY PHILLIPA:
                    //Switch on minidestination or globalstate W vectors.
                    //does not overflow because this is PER DESTINATION!
                   
                      Int32  U_tilda_n = w.ComputeUtility(d.BucketTable, d.Best, d.ChosenParent, d.SecP, copyOfS, currASN, d.L[currASN], d.BestRelation[currASN], WToUse);
                   
                    result.UAfter[currASN] = U_tilda_n;
                    numberOfworkers++;
                }
                else
                    result.UAfter[currASN] = result.UBefore[currASN];//no path to d for this ASN; utility does not change.
            }

            //performance debugging on dryad
            Console.WriteLine(DateTime.Now + ": Finished Dest" + d.destination + " after running " + numberOfworkers + " workers. ");

            return result;
        }

        /// <summary>
        /// given a destination and all its routing information
        /// tell us if the given node has a secP available through any of its parents.
        /// </summary>
        /// <param name="miniDestination"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool hasSecPThroughParent(ref Destination d, UInt32 node)
        {
            if (d.Best[node] == null)
                return false;
          
            foreach (UInt32 parent in d.Best[node])
            {
                if (d.SecP[parent])//parent has a secP to d (note this check is only valid after the update paths computation for d given an S)
                    return true;    
            }
            return false; 
        }

        public static NetworkGraph getNetworkGraphFromFile(string filename)
        {
            if (!File.Exists(filename))
                return null;
            NetworkGraph g = new NetworkGraph();
            InputFileReader iFR = new InputFileReader(filename, g);
            iFR.ProcessFile();
            return g;
        }

        public static List<UInt32> getNodeListFromFile(string filename)
        {
            List<UInt32> toreturn = new List<UInt32>();

            if (!File.Exists(filename))
                return toreturn;

            StreamReader input = new StreamReader(filename);

            while (!input.EndOfStream)
            {
                string line = input.ReadLine();

                if (line.Length>0 && line[0] != '#')
                    toreturn.Add(UInt32.Parse(line));
            }


            input.Close();

            return toreturn;

        }

        /// <summary>
        /// writes out 3 files for this graph
        /// 1 containing the list of stubs, 1 contianing list of nonstubs 
        /// 1 containing allnodes
        /// 
        /// it will pad your directory to be matlab compatible.
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="g"></param>
        public static void writeNodeListsToFile(string outputDirectory, NetworkGraph g)
        {
            string paddedDirectoryName = "";
            string paddedOutputPath=generateDirectoryName(outputDirectory,ref paddedDirectoryName);

            string AllNodesFileName = paddedDirectoryName + "." + Constants._AllNodesFileName;
            string NonStubsFileName = paddedDirectoryName + "." + Constants._NonStubsFileName;
            string StubsFileName = paddedDirectoryName + "." + Constants._StubsFileName;

            try
            {
                if (!Directory.Exists(paddedOutputPath))
                    Directory.CreateDirectory(paddedOutputPath);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating directory: " + paddedOutputPath + " Error: " + e.Message);
                return;
            }
            //make sure we have a \\ on our filepath
            if (paddedOutputPath[paddedOutputPath.Length - 1] != '\\')
                paddedOutputPath = paddedOutputPath + "\\";

            StreamWriter AllNodesOutput = new StreamWriter(AllNodesFileName);
            StreamWriter StubsOutput = new StreamWriter(StubsFileName);
            StreamWriter NonStubsOutput = new StreamWriter(NonStubsFileName);

            var allNodes = g.GetAllNodes().ToArray<AsNode>();
            for (int i = 0; i < allNodes.Length; i++)
                AllNodesOutput.WriteLine(allNodes[i].NodeNum);
            List<UInt32> stubs = g.getStubs();
            for (int i = 0; i < stubs.Count; i++)
                StubsOutput.WriteLine(stubs[i]);
            List<UInt32> nonstubs = g.getNonStubs();
            for (int i = 0; i < nonstubs.Count; i++)
                NonStubsOutput.WriteLine(nonstubs[i]);

            AllNodesOutput.Close();
            StubsOutput.Close();
            NonStubsOutput.Close();


        }

        public static void writeNodeListsToFile(string outputDirectory, string graphDir)
        {
            string paddedDirectoryName =outputDirectory+"\\"+ graphDir;

                
            string AllNodesFileName = paddedDirectoryName + "." + Constants._AllNodesFileName;
            string NonStubsFileName = paddedDirectoryName + "." + Constants._NonStubsFileName;
            string StubsFileName = paddedDirectoryName + "." + Constants._StubsFileName;

            try
            {
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating directory: " + outputDirectory + " Error: " + e.Message);
                return;
            }
            //make sure we have a \\ on our filepath
            if (outputDirectory[outputDirectory.Length - 1] != '\\')
                outputDirectory = outputDirectory + "\\";

            StreamWriter AllNodesOutput = new StreamWriter(AllNodesFileName);
            StreamWriter StubsOutput = new StreamWriter(StubsFileName);
            StreamWriter NonStubsOutput = new StreamWriter(NonStubsFileName);

            NetworkGraph g = new NetworkGraph();
            InputFileReader ifr = new InputFileReader(outputDirectory + graphDir, g);
            ifr.ProcessFile();

            var allNodes = g.GetAllNodes().ToArray<AsNode>();
            for (int i = 0; i < allNodes.Length; i++)
                AllNodesOutput.WriteLine(allNodes[i].NodeNum);
            List<UInt32> stubs = g.getStubs();
            for (int i = 0; i < stubs.Count; i++)
                StubsOutput.WriteLine(stubs[i]);
            List<UInt32> nonstubs = g.getNonStubs();
            for (int i = 0; i < nonstubs.Count; i++)
                NonStubsOutput.WriteLine(nonstubs[i]);

            AllNodesOutput.Close();
            StubsOutput.Close();
            NonStubsOutput.Close();


        }

        public static MiniDestination initMiniDestination(NetworkGraph g, UInt32 destination, bool readOnly)
        {
            //Console.WriteLine("Start initalizing miniDest " + destination + "--------------------");//zhiying

            MiniDestination miniDestination = new MiniDestination();
            miniDestination.destination = destination;
            miniDestination.Best = new List<UInt32>[Constants._numASNs];
            miniDestination.BestNew = new List<UInt32>[Constants._numASNs];

            miniDestination.BucketTable = new List<UInt32>[Constants._maxPathLen][];
            for(int i =0;i<miniDestination.BucketTable.Length;i++)
            miniDestination.BucketTable[i]=new List<UInt32>[Constants._numRelationshipTypes];

            miniDestination.L = new byte[Constants._numASNs];
            miniDestination.BestRelation = new byte[Constants._numASNs];

            //ADDED BY PHILLIPA Weight per destination.
            miniDestination.W=new float[Constants._numASNs];

            /** extra things that get computed but don't need to get put into minidestination **/
            List<UInt32>[] ChosenPath = new List<UInt32>[Constants._numASNs];
            UInt32[] ChosenParent = new UInt32[Constants._numASNs];
            g.ReinitializeBfsState();
            if (!readOnly)
            {
                //run the BFS
                ModifiedBfs.RoutingTreeAlg(g, destination, ref miniDestination.Best, ref miniDestination.BestNew, ref miniDestination.BucketTable, ref ChosenPath, ref ChosenParent, ref miniDestination.L, ref miniDestination.BestRelation);
            }
            else
            {
                NetworkGraph myCopy = new NetworkGraph(g);
                //run the BFS
                ModifiedBfs.RoutingTreeAlg(myCopy, destination, ref miniDestination.Best, ref miniDestination.BestNew, ref miniDestination.BucketTable, ref ChosenPath, ref ChosenParent, ref miniDestination.L, ref miniDestination.BestRelation);
           

            }
            //sort best i's by their hash value.
            for(UInt32 i =0;i<miniDestination.Best.Length;i++)
            {
                if (miniDestination.Best[i] != null)
                    miniDestination.Best[i] = sortByHashValue(miniDestination.Best[i], i);
            }

            //ADDED BY SHARON
            //converts the best i list of a stub node to the single element that wins the tiebreak without security
            //captures the idea that stubs need not break ties based on security (because they don't verify)
            if (stubsDontCare)
            {
                for (UInt32 i = 0; i < miniDestination.Best.Length; i++)
                {
                    var node = g.GetNode(i);
                    //if hes a stub not in the big5 with non-null best i set
                    if (node != null 
                            && node.isStub() 
                            && !big5List.Contains(i) 
                            && miniDestination.Best[i] != null) 
                        miniDestination.Best[i] = new List<UInt32> { miniDestination.Best[i].First() } ;  //his best i becomes just one element
                }

            }

            //ADDED BY PHILLIPA: within the miniD give each
            //AS a weight (W) based on their level in the buckettable
            //(i.e., further down the bucket table they are, the lower
            //their weight should be wrt this destination)

            //Weight of the destination is 0 (it will send no traffic to itself. 
            miniDestination.W[miniDestination.BucketTable[0][0][0]] = 0;
            for (int row = 1; row < miniDestination.BucketTable.GetLength(0); row++)
            {
                for (int col = 0; col < 3;col++ )//0=customer, 1=peer, 2=provider columns.
                {
                    if (miniDestination.BucketTable[row][col] != null)
                    {
                        foreach (UInt32 i in miniDestination.BucketTable[row][col])
                        {
                            //weight is 1/row, but multiplied out to convert it back to a UInt16
                            //i.e., instead of fraction between 0 and 1 we have values between 0 and 500
                            miniDestination.W[i] =computeWeight(row);
                            //Console.WriteLine("setting W for " + i + " to destination "+miniDestination.BucketTable[0][0][0] +" to " + miniDestination.W[i]);
                        }
                    }
                }
            }

           //Console.WriteLine("Done initalizing miniDest " + destination + "--------------------");
            return miniDestination;

        }

        //Added by Phillipa more modular computeWeight function.
        private static float computeWeight(int row)
        {
            //if we do not want the less harsh weight just 10/row.
            if(!useLessHarshWeight)
            return 10 / row;

            //less harsh, group together people with short paths.
            if (row - 2 <= 0)
                return 10;
            //give everyone else harsh weighting.
            return 10 / (row - 2);
        }

        public static MiniDestination initMiniDestinationSP(NetworkGraph g, UInt32 destination, bool readOnly)
        {
            Console.WriteLine("Start initalizing miniDestSP " + destination);

            MiniDestination miniDestination = new MiniDestination();
            miniDestination.destination = destination;
            miniDestination.Best = new List<UInt32>[Constants._numASNs];

            miniDestination.BucketTable = new List<UInt32>[Constants._maxPathLen][];
            for (int i = 0; i < miniDestination.BucketTable.Length; i++)
                miniDestination.BucketTable[i] = new List<UInt32>[Constants._numRelationshipTypes];

            miniDestination.L = new byte[Constants._numASNs];
            miniDestination.BestRelation = new byte[Constants._numASNs];

            /** extra things that get computed but don't need to get put into minidestination **/
            List<UInt32>[] ChosenPath = new List<UInt32>[Constants._numASNs];
            UInt32[] ChosenParent = new UInt32[Constants._numASNs];
            g.ReinitializeBfsState();
            if (!readOnly)
            {
                //run the BFS
                ModifiedBfs.ShortestRoutingTreeAlg(g, destination, ref miniDestination.Best, ref miniDestination.BucketTable, ref ChosenPath, ref ChosenParent, ref miniDestination.L, ref miniDestination.BestRelation);
            }
            else
            {
                NetworkGraph myCopy = new NetworkGraph(g);
                //run the BFS
                ModifiedBfs.ShortestRoutingTreeAlg(myCopy, destination, ref miniDestination.Best, ref miniDestination.BucketTable, ref ChosenPath, ref ChosenParent, ref miniDestination.L, ref miniDestination.BestRelation);


            }
            //sort best i's by their hash value.
            for (UInt32 i = 0; i < miniDestination.Best.Length; i++)
            {
                if (miniDestination.Best[i] != null)
                    miniDestination.Best[i] = sortByHashValue(miniDestination.Best[i], i);
            }

            // Console.WriteLine("Done initalizing miniDest " + destination);
            return miniDestination;

        }

        private static List<UInt32> sortByHashValue(List<UInt32> Best,UInt32 i)
        {
             List<UInt32> toreturn = new List<UInt32>();
             if (ModifiedBfs.Hash)
             {
                 Hashtable hashToVal = new Hashtable();
                 List<UInt32> hashValues = new List<UInt32>();
                 foreach (var parent in Best)
                 {
                     UInt32 hashI = ModifiedBfs.hash6432shift(i, parent);
                     hashToVal.Add(hashI, parent);
                     hashValues.Add(hashI);
                 }
                 hashValues.Sort();

                 for (int j = 0; j < hashValues.Count; j++)
                     toreturn.Add((UInt32)hashToVal[hashValues[j]]);
             }
             else
             {Best.Sort();
             toreturn = Best;
             }
            return toreturn;
            
        }

        /// <summary>
        /// for now this just sets all the weights to 1
        /// turns on the stubs and destinations
        /// and populates the nonstubs list using g
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
   /**     public static GlobalState initGlobalState(NetworkGraph g, List<UInt32> earlyAdopters)
        {
            GlobalState globalState = new GlobalState();

            //init nonstubs list.
            globalState.nonStubs = g.getNonStubs();

            //turn on stubs and destinations
            globalState.S = new bool[Constants._numASNs];
            List<UInt32> stubs = g.getStubs();
            for (int i = 0; i < globalState.S.Length; i++)
                globalState.S[i] = false;
            foreach (UInt32 AS in stubs)
                globalState.S[AS] = true;
            foreach (UInt32 AS in earlyAdopters)
                globalState.S[AS] = true;

           //init W with 1's
            globalState.W = new short[Constants._numASNs];
            for (int i = 0; i < globalState.W.Length; i++)
                globalState.W[i] = 1;

            return globalState;

        }*/

        /// <summary>
        /// this function assigns weight to the topX nodes and remaining nodes
        /// according to the constant K that is passed in. It will weight the
        /// topX nodes as (N-n) and the small nodes as K*n where n is the 
        /// number of nodes in the topX set and N is all nodes in the graph.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="earlyAdopters"></param>
        /// <param name="topX"></param>
        /// <param name="K">weight of small nodes will be K*n</param>
        /// <returns></returns>
        public static GlobalState initGlobalState(NetworkGraph g, List<UInt32> earlyAdopters,
            List<UInt32> weightedNodes,short K)
        {
            //K=0, we revert to original function
            if (K == 0)
            {
                return initGlobalState(g, earlyAdopters, weightedNodes);
            }
            GlobalState globalState = new GlobalState();

            //init nonstubs list.
            globalState.nonStubs = g.getNonStubs();

            //turn on stubs and destinations
            globalState.S = new bool[Constants._numASNs];
            List<UInt32> stubs = g.getStubs();
            for (int i = 0; i < globalState.S.Length; i++)
                globalState.S[i] = false;
            foreach (UInt32 AS in stubs)
                if (!big5List.Contains(AS))
                    globalState.S[AS] = true;  //don't accidentally turn on a big5 node
            foreach (UInt32 AS in earlyAdopters)
                globalState.S[AS] = true;


            //init W with 1's
            globalState.W = new UInt16[Constants._numASNs];
            for (int i = 0; i < globalState.W.Length; i++)
                globalState.W[i] = 0;//init array to all 0s
                //only give weights to real ASNs

            IEnumerable<AsNode> allNodes = g.GetAllNodes();

            UInt16 topXWeight = (UInt16)(allNodes.Count() - weightedNodes.Count);
             UInt16 everyoneElseWeight = (UInt16)(K * weightedNodes.Count);
            foreach (AsNode node in allNodes)
            {
                if (weightedNodes.Contains(node.NodeNum))
                {
                    //this AS is in the top X
                    globalState.W[node.NodeNum] = topXWeight;
                }
                else
                    globalState.W[node.NodeNum] = everyoneElseWeight;
            }

            return globalState;

        }

        public static void updateGlobalState(ref GlobalState currentState, List<Message> results, float percent, ref Int64[] Before, ref Int64[] After)
        {
            numberFlipped = 0;
            numberOn = 0;

            if (results.Count == 0)
                return;


            //   if(After.Length != results[0].UAfter.Length)
            After = new Int64[results[0].UAfter.Length];
            //  if(Before.Length != results[0].UBefore.Length)
            Before = new Int64[results[0].UBefore.Length];
            for (int currASN = 0; currASN < Before.Length; currASN++)
            {
                for (int d = 0; d < results.Count; d++)
                {
                    //add delta utility for this ASN and this destination to the delta array

                    After[currASN] += results[d].UAfter[currASN];
                    Before[currASN] += results[d].UBefore[currASN];

                }
                if(currASN<9)
                    Console.WriteLine("AS " + currASN + " Before: " + Before[currASN] + " After: " + After[currASN]);
                //now have aggregated Before and After results for currASN across all d's . decide if we will flip its state.
               if (After[currASN] > Before[currASN] * (1 + percent))
                {
                     currentState.S[currASN] = !currentState.S[currASN];//flip their state.
                    numberFlipped++;
                }
                if (currentState.S[currASN])
                    numberOn++;

            }



        }

        /// <summary>
        /// given a graph writes out a cyclops formatted file. note that for re-reading the code 
        /// would be happiest if the filename starts with "Cyclops"
        /// </summary>
        /// <param name="g"></param>
        /// <param name="filename"></param>
        public static void writeGraph(NetworkGraph g, string filename)
        {
            IEnumerable<AsNode> allNodes = g.GetAllNodes();
            HashSet<string> outputtedEdges = new HashSet<string>();
            StreamWriter output = new StreamWriter(filename);
            foreach (AsNode node in allNodes)
            {
                IEnumerable<AsNode> customers = node.GetNeighborsByType(RelationshipType.ProviderTo);
                IEnumerable<AsNode> peers = node.GetNeighborsByType(RelationshipType.PeerOf);
                IEnumerable<AsNode> providers = node.GetNeighborsByType(RelationshipType.CustomerOf);

                foreach (AsNode customer in customers)
                {
                    string currEdge = makeEdge(node.NodeNum, customer.NodeNum);
                    if (!outputtedEdges.Contains(currEdge))
                    {
                        outputtedEdges.Add(currEdge);
                        output.WriteLine(node.NodeNum + "\t" + customer.NodeNum + "\tp2c");
                    }
                }
                foreach (AsNode peer in peers)
                {
                    string currEdge = makeEdge(node.NodeNum, peer.NodeNum);
                    if (!outputtedEdges.Contains(currEdge))
                    {
                        outputtedEdges.Add(currEdge);
                        output.WriteLine(node.NodeNum + "\t" + peer.NodeNum + "\tp2p");
                    }
                }
                foreach (AsNode provider in providers)
                {
                    string currEdge = makeEdge(node.NodeNum, provider.NodeNum);
                    if (!outputtedEdges.Contains(currEdge))
                    {
                        outputtedEdges.Add(currEdge);
                        output.WriteLine(node.NodeNum + "\t" + provider.NodeNum + "\tc2p");
                    }
                }
            }
            output.Close();
        }

        private static string makeEdge(UInt32 AS1, UInt32 AS2)
        {
            if (AS1 < AS2)
                return AS1 + "-" + AS2;
            return AS2 + "-" + AS1;


        }

        /// <summary>
        /// will add edges to the graph until the AS is connected to the specified fraction of nodes 
        /// *NOTE: if you specify onlyNonStubs it will connect you to the specified frac
        /// </summary>
        /// <param name="g"></param>
        /// <param name="AS"></param>
        /// <param name="fractionOfNodes"></param>
        /// <param name="onlyNonStubs"></param>
        public static void addEdges(ref NetworkGraph g, UInt32 AS, double fractionOfNodes, bool fractionOfAll,bool onlyNonStubs)
        {
            double numEdges;
            if (fractionOfAll)
                numEdges = g.NodeCount * fractionOfNodes;
            else
                numEdges = g.getNonStubs().Count * fractionOfNodes;

            addEdges(ref g, AS, (int)Math.Round(numEdges), onlyNonStubs);

        }

        public static void addEdgesToTopN(ref NetworkGraph g, UInt32 AS, int topN)
        {
            AsNode asn = g.GetNode(AS);
            if (asn == null)
            {
                Console.WriteLine("error: " + AS + " was not in the graph.");
                return;
            }
            var topAsNodes = g.GetAllNodes().OrderBy(node => node.GetAllNeighbors().Count()).Reverse().ToArray();
            var neighbors = asn.GetAllNeighbors();
            for (int i = 0; i < topN; i++)
            {
                if (!neighbors.Contains(topAsNodes[i]))
                {
                    g.AddEdge(asn.NodeNum, topAsNodes[i].NodeNum, RelationshipType.PeerOf);
                    g.AddEdge(topAsNodes[i].NodeNum, asn.NodeNum, RelationshipType.PeerOf);
                }
            }

        }

        /// <summary>
        /// read a file that has the IXP ases 1 per line.
        /// </summary>
        /// <param name="IXPFilename"></param>
        /// <returns></returns>
        public static List<UInt32> getIXPASes(string IXPFilename)
        {
            List<UInt32> ASes = new List<UInt32>();
            StreamReader input = new StreamReader(IXPFilename);
            while (!input.EndOfStream)
            {
                UInt32 AS;
                if (UInt32.TryParse(input.ReadLine(), out AS))
                    ASes.Add(AS);
            }
            input.Close();
            return ASes;


        }

      


        /// <summary>
        /// adds edges to the specified fraction of ASes in the IXPASes list
        /// for the AS that is given as input.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="AS"></param>
        /// <param name="fraction"></param>
        /// <param name="IXPASes"></param>
        public static void addEdges(ref NetworkGraph g, UInt32 AS, double fraction, List<UInt32> IXPASes)
        {
            AsNode asn = g.GetNode(AS);
            if (asn == null)
            {
                Console.WriteLine("error: " + AS + " was not in the graph.");
                return;
            }

            var neighbors = asn.GetAllNeighbors();
            int currentConnectivity = 0;//how many of the IXP ASes do we already connect to
            foreach (var neighbor in neighbors)
            {
                if (IXPASes.Contains(neighbor.NodeNum))
                    currentConnectivity++;

            }

            int idealConnectivity = (int)Math.Round(fraction * IXPASes.Count);//how many edges we should have to these guys.
            if (idealConnectivity <= currentConnectivity)
            {
                Console.WriteLine("not adding any edges for AS: " + AS);
                return;
            }
            Random Random = new Random();
            while (currentConnectivity < idealConnectivity)
            {

                UInt32 potentialNeighborAS = IXPASes[Random.Next(IXPASes.Count)];
                AsNode potentialNeighbor = g.GetNode(potentialNeighborAS);
                if (potentialNeighbor != null && !neighbors.Contains(potentialNeighbor))
                {
                    g.AddEdge(asn.NodeNum, potentialNeighbor.NodeNum, RelationshipType.PeerOf);
                    g.AddEdge(potentialNeighbor.NodeNum, asn.NodeNum, RelationshipType.PeerOf);
                    currentConnectivity++;
                }
            }

        }
        /// <summary>
        /// given a graph g, if the AS has fewer than num edges add random edges for them
        /// until they have numEdges edges. Connections only to non-stubs if onlyStubs=true
        /// </summary>
        /// <param name="g"></param>
        /// <param name="AS"></param>
        /// <param name="numEdges"></param>
        /// <param name="onlyStubs"></param>
        public static void addEdges(ref NetworkGraph g, UInt32 AS, int numEdges, bool onlyNonStubs)
        {
            AsNode asn=g.GetNode(AS);
            if (asn == null)
            {
                Console.WriteLine("error: " + AS + " was not in the graph.");
                return;
            }
            var Neighbors = asn.GetAllNeighbors();
            if (Neighbors.Count() >= numEdges)
            {
                Console.WriteLine("not adding any edges for AS: " + AS);
                return;
            }

            List<UInt32> nonStubs = g.getNonStubs();
            Random Random = new Random();
            while (Neighbors.Count() < numEdges && nonStubs.Count > 0)
            {
                AsNode potentialNeighbor = null;
                if (onlyNonStubs)
                    potentialNeighbor = g.GetNode(nonStubs[Random.Next(nonStubs.Count)]);
                else
                    potentialNeighbor = g.GetRandomNode();

                if (potentialNeighbor!= null && !Neighbors.Contains(potentialNeighbor))
                {
                    g.AddEdge(asn.NodeNum, potentialNeighbor.NodeNum, RelationshipType.PeerOf);
                    g.AddEdge(potentialNeighbor.NodeNum, asn.NodeNum, RelationshipType.PeerOf);
                    if(onlyNonStubs)
                    nonStubs.Remove(potentialNeighbor.NodeNum);//so we don't try to add them again.
                }
            }

            if (nonStubs.Count == 0)
                Console.WriteLine("you may have ran out of non-stubs");
            
        }

        /// <summary>
        /// Given a graph 'g' remove the children of the ASes specified in the big AS list.
        /// </summary>
        /// <param name="bigAS"></param>
        /// <param name="g"></param>
        public static void killChildren(List<UInt32> bigAS, ref NetworkGraph g)
        {
           

            foreach (UInt32 asn in bigAS)
            {
                var children = g.GetNode(asn).GetNeighborsByType(RelationshipType.ProviderTo).ToArray();
                foreach (AsNode child in children)
                {
                    g.RemoveNode(child.NodeNum);

                }
            }
        }

        //takes the contents of "warmstart" directory, copies them to output directory
        //and retrieves the last state vector from the warmstart directory.
        public static bool[] getLastStateAndCopyFilesToDir(string outputDirectory, string descriptiveName)
        {
            //grab the files from the warmstart folder
            var outputFileNames = new List<string>();
            foreach (var f in Directory.GetFiles("warmstart"))
            {
                Console.WriteLine(f);
                outputFileNames.Add(f);
            }

            //check that we have a matching statefile in here
            string paddedDescriptiveName = "";
            paddedDescriptiveName = generateDirectoryName(descriptiveName, ref paddedDescriptiveName);
            string stateFileName = paddedDescriptiveName + "." + Constants._SFileName;
            if (!outputFileNames.Contains("warmstart\\" + stateFileName ))
            {
                Console.WriteLine("Warmstart failed.  No valid statefile matching " + stateFileName + " in the warmstart folder.");
                return null;
            }

                     
            //first copy data from the warmstart directory to the new directory
            Directory.CreateDirectory(outputDirectory);
            foreach (var f in outputFileNames)
                File.Copy(f, outputDirectory + "\\" + f.Split('\\')[1], true);

            //now read out the last state
            StreamReader input = new StreamReader("warmstart\\" + stateFileName);
            string[] statesAsString = null;
            while (!input.EndOfStream)
                statesAsString = input.ReadLine().Split(','); //get the last state

            if (statesAsString == null)
            {
                Console.WriteLine("Warmstart failed.  StateFile "+ stateFileName + " is empty.");
                return null;
            }
            
            //finally we can convert it to a bool.
            bool[] S = new bool[statesAsString.Length];
            for (int i = 0; i < statesAsString.Length; i++) 
                S[i] = statesAsString[i] == "1" ? true : false;
            return S;  
        }



       /// <summary>
       /// 
       /// </summary>
       /// <param name="outputDirectory">this is the directory name with the time stamp in it it can be whatever you want</param>
       /// <param name="descriptiveName">this will get chopped or padded to be 20 chars long and prepended to the state/utility filenames</param>
       /// <param name="currentIteration">array of results to output for thisiteration.</param>
        public static void writeIterationResultsToFiles(string outputDirectory, string descriptiveName,nodeStateUtility[] currentIteration)
        {
            string paddedDescriptiveName = "";

             paddedDescriptiveName = generateDirectoryName(descriptiveName, ref paddedDescriptiveName);

             string UBeforeFileName = paddedDescriptiveName + "." + Constants._UBeforeFileName;
             string UAfterFileName = paddedDescriptiveName + "." + Constants._UAfterFileName;
             string SFileName = paddedDescriptiveName + "." + Constants._SFileName;
           
            try
            {
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating directory: "+outputDirectory + " Error: "+e.Message);
                return;
            }
            //make sure we have a \\ on our filepath
            if (outputDirectory[outputDirectory.Length - 1] != '\\')
                outputDirectory = outputDirectory + "\\";

            StreamWriter beforeOutput = new StreamWriter(new FileStream(outputDirectory + UBeforeFileName, FileMode.Append, FileAccess.Write));
            StreamWriter afterOutput = new StreamWriter(new FileStream(outputDirectory + UAfterFileName, FileMode.Append, FileAccess.Write));
            StreamWriter sOutput = new StreamWriter(new FileStream(outputDirectory + SFileName, FileMode.Append, FileAccess.Write));
            //writing to the files is 
            string Before, After, S;
            Before = After = S = "";
            iterationResultsToStrings(currentIteration, ref Before, ref After, ref S);

            beforeOutput.WriteLine(Before);
            afterOutput.WriteLine(After);
            sOutput.WriteLine(S);

            beforeOutput.Close();
            afterOutput.Close();
            sOutput.Close();

        }

        public static void copyParametersFile(string outputDirectory, string parameterFileName)
        {
            StreamReader input = new StreamReader(parameterFileName);

            if(outputDirectory[outputDirectory.Length -1]!='\\')
                outputDirectory= outputDirectory+"\\";
            string[] parameterFileNamePieces = parameterFileName.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            StreamWriter output = new StreamWriter(outputDirectory + parameterFileNamePieces[parameterFileNamePieces.Length-1]);
            output.Write(input.ReadToEnd());

            input.Close();
            output.Close();
        }

        /// <summary>
        ///returns a string that is the directory path with the final directoryname padded out to 20 chars
        ///also returns this padded out value in the directoryName variable
        ///e.g., 
        ///c:\\users\\results 
        ///gets returned as
        ///c:\\users\\results0000000000000
        ///and the directory name string will contain results0000000000000
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filename"></param>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        public static string generateDirectoryName(string directoryPath, ref string directoryName)
        {
            //retrieve the last part of the file path
            char[] pathseparator = { '\\' };
            string[] directoryPieces = directoryPath.Split(pathseparator, StringSplitOptions.RemoveEmptyEntries);
             directoryName = directoryPieces[directoryPieces.Length - 1];

            //crop to 20 chars
            if (directoryName.Length > 20)
                directoryName = directoryName.Substring(0, 20);
            //pad out with 0s
            if (directoryName.Length < 20)
            {
                while (directoryName.Length < 20)
                    directoryName = directoryName + "x";
            }
            string pathToReturn = "";
            for (int i = 0; i < directoryPieces.Length - 1; i++)
                pathToReturn = pathToReturn + directoryPieces[i] + "\\";
            pathToReturn = pathToReturn + directoryName;
            return pathToReturn;
          
        }

        private static void iterationResultsToStrings(nodeStateUtility[] iterationResults, ref string Before, ref string After, ref string S)
        {
            Before = After = S = "";
            for (int i = 0; i < iterationResults.Length; i++)
            {
                Before = Before + iterationResults[i].before+",";
                After = After + iterationResults[i].after + ",";
                if (iterationResults[i].state)
                    S = S + "1,";
                else
                    S = S + "0,";
            }
        
        
        }


        private static GlobalState initGSHelper(NetworkGraph g, List<UInt32> earlyAdopters)
        {
            GlobalState globalState = new GlobalState();

            //init nonstubs list.
            globalState.nonStubs = g.getNonStubs();

            //turn on stubs and destinations
            globalState.S = new bool[Constants._numASNs];
            List<UInt32> stubs = g.getStubs();
            for (int i = 0; i < globalState.S.Length; i++)
                globalState.S[i] = false;
            foreach (UInt32 AS in stubs)
                if (!big5List.Contains(AS))  //don't accidentally turn on a big5 node
                    globalState.S[AS] = true;
            if (earlyAdopters != null)
            {
                foreach (UInt32 AS in earlyAdopters)
                    globalState.S[AS] = true;
            }

            return globalState;
        }

        /// <summary>
        /// for now this just sets all the weights to 1
        /// turns on the stubs and destinations
        /// and populates the nonstubs list using g
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static GlobalState initGlobalState(NetworkGraph g,List<UInt32> earlyAdopters)
        {
            GlobalState globalState = initGSHelper(g, earlyAdopters);

           //init W with 1's
            globalState.W = new UInt16[Constants._numASNs];
            for (int i = 0; i < globalState.W.Length; i++)
                globalState.W[i] = 1;

            return globalState;
        }

        /// <summary>
        /// OVERLOAD FUNCTION ADDED BY SHARON
        /// turns on the stubs and destinations
        /// and populates the nonstubs list using g
        /// weights the nodes in weightedOne with weight 1, all others with weight 0
        /// DO NOT USE THIS FUNCTION WITH A NULL weightedOne List!!!
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static GlobalState initGlobalState(NetworkGraph g, List<UInt32> earlyAdopters, List<UInt32> weightedOne)
        {
            GlobalState globalState = initGSHelper(g, earlyAdopters);

            //init W with 0's, except the nodes weighted 1:
            globalState.W = new UInt16[Constants._numASNs];
            for (int i = 0; i < globalState.W.Length; i++)
                globalState.W[i] = 0;

            foreach (var node in weightedOne)
                globalState.W[node] = 1;

            return globalState;
        }


        /// <summary>
        /// ADDED BY SHARON!  Creates a clean copy of the global state.
        /// </summary>
        public static GlobalState copyGlobalState(GlobalState otherGS)
        {
            GlobalState globalState = new GlobalState();

            globalState.nonStubs = new List<UInt32>();
            foreach (var nonStub in otherGS.nonStubs)
                globalState.nonStubs.Add(nonStub);

            globalState.S = new bool[Constants._numASNs];
            globalState.W = new UInt16[Constants._numASNs];
            for (int i = 0; i < globalState.W.Length; i++)
            {
                globalState.W[i] = otherGS.W[i];
                globalState.S[i] = otherGS.S[i];
            }

            return globalState;

        }

        //takes the contents of "warmstart" directory, copies them to output directory
        //and retrieves the last state vector from the warmstart directory.
        public static bool[] getLastStateAndCopyFilesToDir(string outputDirectory, string descriptiveName, ref int warmStartIteration)
        {
            warmStartIteration = 0;
            Console.WriteLine("\n\n****************\n WARMSTART ");

            //grab the files from the warmstart folder
            var outputFileNames = new List<string>();
            foreach (var f in Directory.GetFiles("warmstart"))
            {
                Console.WriteLine(f);
                outputFileNames.Add(f);
            }

            //check that we have a matching statefile in here
            string paddedDescriptiveName = "";
            paddedDescriptiveName = generateDirectoryName(descriptiveName, ref paddedDescriptiveName);
            string stateFileName = paddedDescriptiveName + "." + Constants._SFileName;
            if (!outputFileNames.Contains("warmstart\\" + stateFileName))
            {
                Console.WriteLine("Warmstart failed.  No valid statefile matching " + stateFileName + " in the warmstart folder.");
                return null;
            }


            //first copy data from the warmstart directory to the new directory
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            foreach (var f in outputFileNames)
            {
                if (f.Contains("params"))
                    File.Copy(f, outputDirectory + "\\" + f.Split('\\')[1] + ".warmStart", true);
                else
                    File.Copy(f, outputDirectory + "\\" + f.Split('\\')[1], true);   //overwrite
            }
            //now read out the last state
            StreamReader input = new StreamReader("warmstart\\" + stateFileName);
            string[] statesAsString = null;

            while (!input.EndOfStream)
            {
                statesAsString = input.ReadLine().Split(','); //get the last state
                warmStartIteration++;
            }
            if (statesAsString == null)
            {
                Console.WriteLine("Warmstart failed.  StateFile " + stateFileName + " is empty.");
                return null;
            }

            //finally we can convert it to a bool.
            bool[] S = new bool[Constants._numASNs];
            for (int i = 0; i < Constants._numASNs; i++)
                S[i] = statesAsString[i] == "1" ? true : false;
            Console.WriteLine("Successfully retrieved "+warmStartIteration+ " iterations and state from warmstart folder.");
            return S;


        }




    }
}
