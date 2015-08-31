namespace Graphing.Entities
{
    public partial class DataLogModel 
    {
        partial class ScoresDataTable
        {
            public System.Threading.ReaderWriterLockSlim Lock = new System.Threading.ReaderWriterLockSlim();
        }
    
        partial class ImpactsDataTable
        {
            public System.Threading.ReaderWriterLockSlim Lock = new System.Threading.ReaderWriterLockSlim();
        }
    }
}
