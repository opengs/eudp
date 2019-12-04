using System;
namespace EUDP.Chanels.Nodes{
    /// <summary>
    /// Specifies, that node can operateable. That means it need manual updates.
    /// Usefull for nodes, that shoul be run only for secified thread and in specified time.
    /// </summary>
    public interface IOperateAble{
        void Operate();
    }
}
