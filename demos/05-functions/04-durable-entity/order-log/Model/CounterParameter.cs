namespace DurableCounter.Model
{
    /// <summary>
    /// Class to send input to orchestration. 
    /// </summary>
    public class CounterParameter
    {
        public string OperationName { get; set; }
    }
}
