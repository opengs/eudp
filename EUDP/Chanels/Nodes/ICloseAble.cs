using System;
namespace EUDP.Chanels.Nodes{
    /// <summary>
    /// Specifies that node must perform specific operations before closing connection. 
    /// </summary>
    public interface ICloseAble{
        void Close();
    }
}
