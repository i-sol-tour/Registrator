
namespace KitCashProtocol
{
    class DoubleValue
    {
        public byte[] Value { get; set; }
	    public byte Size { get; set; }

        public DoubleValue()
        {
            Value = new byte[128];
        }
    }
}