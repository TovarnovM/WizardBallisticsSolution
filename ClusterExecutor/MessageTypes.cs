namespace ClusterExecutor {
    public class MessageTypes {
        public const int Terminate = 0; //tells the prime worker to stop
        public const int Start = 1; //initialize the prime workers
        public const int ReplyTask = 2; //the main worker sends a batch of numbers
        public const int RequestTask = 3; //the prime worker requests a new batch
        public const int Result = 4; //the prime worker sends the count back
        public const int RequestStatus = 5;
        public const int ReplyStatus = 6;
        public const int Pause = 7;
        public const int Run = 8;
        public const int StartNewTask = 9;
        public const int ReadyAgain = 10;
        public const int Buisy = 11;
        public const int ResultError = 12;
        public const int Initilised = 13;
    }
}
